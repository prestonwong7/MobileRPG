using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;//for Type class
using System.Reflection;
using System.Linq;

public class SaveLoadUtility : MonoBehaviour {

	public bool usePersistentDataPath = true;//if true, savegames are saved to and loaded from a public folder.
	//Application.DataPath: http://docs.unity3d.com/ScriptReference/Application-dataPath.html
	//Application.persistentDataPath: http://docs.unity3d.com/ScriptReference/Application-persistentDataPath.html

	public string saveGamePath;//the path where savegames are saved to and loaded from. Overwritten by persistentDataPath if usePersistentDataPath is set to true.
	//You may define any path you like, such as "c:/Saved Games"
	//remember to use slashes instead of backslashes! ("/" instead of "\")

	//you can leave this in most cases, it just determines what your Quicksave save file will be called.
	public string quickSaveName = "QuickSave";

	public DebugController debugController;	//This is used to enable or disable certain console messages
	private Dictionary<string,GameObject> prefabDictionary;//holds references to all prefabs in the Resource folder hierarchy, accessed by the prefab's name, which must be unique. Filled in Start().

	private Dictionary<RefReconnecter,string> refDict;//used to reconnect fields of Type GameObject and those inheriting from Component to their respective reference instances again after loading.
	[HideInInspector]private List<string> surrogateTypes = new List<string>() {
		"Vector2",
		"Vector3",
		"Vector4",
		"Quaternion",
		"Color",
		"Color32"
	};

	[HideInInspector]private List<PropertySelector> propertySelectors = new List<PropertySelector>() {
		new PropertySelector("Transform", new List<string>() {
			"position",
			"localPosition",
			"rotation",
			"localScale"
		})
	};

	[System.Serializable]
	public class RefReconnecter {
		public object baseInstance;
		public FieldInfo field;
		public PropertyInfo property;
		public CollectionType collectionType = CollectionType.None;
		public object loadedValue;
	}

	public enum CollectionType {
		None,
		Array,
		List,
		Dictionary
	};

	[System.Serializable]
	public class PropertySelector {
		public PropertySelector() {

		}

		public PropertySelector(string s, List<string> list) {
			type = s;
			exceptions = list;
		}

		public string type;
		public List<string> exceptions = new List<string>();
	}

	[System.Serializable]
	public class ConvertedDictionary {
		public Dictionary<string,object> keys = new Dictionary<string, object>();
		public Dictionary<string,object> values = new Dictionary<string, object>();

		public ConvertedDictionary() {

		}

		public ConvertedDictionary(Dictionary<string,object> k, Dictionary<string,object> v) {
			keys = k;
			values = v;
		}
	}

	//This class is used to enable or disable certain console messages
	//Alternatively, the Debug.Log lines in the code itself can be commented out or in.
	[System.Serializable]
	public class DebugController {
		public bool gameSaved = true;
		public bool gameLoaded = true;
		public bool loadPrefab;
		public bool oiIsSetToDontSave;
		public bool currentRefRec;
		public bool isPersistent;
		public bool destroyingGameObject;
		public bool packGameObject;
		public bool packComponent;
		public bool checkCompSaveMode;
		public bool notSavingComp;
		public bool getValuesType;
		public bool getValuesField;
		public bool checkPropertySelector;
		public bool fieldsLength;
		public bool writeFieldType;
		public bool writeSurrogateFound;
		public bool writeFieldInfo;
		public bool instantiate;
		public bool unpackComponent;
		public bool readFieldInfo;
		public bool addFieldToDict;
		public bool setValuesType;
		public bool setValuesField;
		public bool readFieldType;
		public bool readFromDict;
	}

	// Use this for initialization
	void Start () {

		//Load all ObjectIdentifier components in the Resources folder hierarchy and fill the prefabDictionary with their GameObjects.
		//This allows the utility to instantiate GameObjects that were saved after loading.
		//Note that all prefabs that should be affected by saving and loading must have an ObjectIdentifier component attached.
		prefabDictionary = new Dictionary<string, GameObject>();
		ObjectIdentifier[] prefabs_oi = Resources.LoadAll<ObjectIdentifier>("");
		foreach(ObjectIdentifier oi in prefabs_oi) {
			prefabDictionary.Add (oi.gameObject.name,oi.gameObject);
			if(debugController.loadPrefab) {
				Debug.Log("Added GameObject to prefabDictionary: " + oi.gameObject.name);
			}
		}
	}

	// Update is called once per frame
	void Update () {


	}

	//use this one for specifying a filename
	public void SaveGame(string saveGameName) {

		if(string.IsNullOrEmpty(saveGameName)) {
			Debug.Log ("SaveGameName is null or empty!");
			return;
		}

		//Declare the path where the SaveGame file will be saved to.
		string pathToUse = saveGamePath;
		if(usePersistentDataPath == true) {
			pathToUse = Application.persistentDataPath + "/Saved Games/";
		}
		if(string.IsNullOrEmpty(pathToUse)) {
			Debug.Log ("SaveGame path is null or empty!");
			return;
		}

		//Create a new instance of SaveGame. This will hold all the data that should be saved in our scene.
		SaveGame newSaveGame = new SaveGame();
		newSaveGame.savegameName = saveGameName;
		newSaveGame.saveDate = DateTime.Now.ToString();

		//Clear the SaveLoad.objectIdentifierDict
		SaveLoad.objectIdentifierDict = new Dictionary<string, ObjectIdentifier>();

		//Find all ObjectIdentifier components in the scene.
		//Since we can access the gameObject to which each one belongs with .gameObject, we thereby get all GameObject in the scene which should be saved!
		ObjectIdentifier[] OIsToSerialize = FindObjectsOfType(typeof(ObjectIdentifier)) as ObjectIdentifier[];
		//Go through the "raw" collection of components
		if(true) {
			foreach (ObjectIdentifier oi in OIsToSerialize) {
				//if the gameObject shouldn't be saved, for whatever reason (maybe it's a temporary ParticleSystem that will be destroyed anyway), ignore it
				if(oi.dontSave == true) {
					if(debugController.oiIsSetToDontSave) {
						Debug.Log("GameObject " + oi.gameObject.name + " is set to dontSave = true, continuing loop.");
					}
					continue;
				}

				//First, we will set the ID of the GameObject if it doesn't already have one.
				if(string.IsNullOrEmpty(oi.id) == true) {
					oi.SetID();
				}
				//then, we will add the OI to the SaveLoad.objectIdentifierDict with the id as key
				//this Dictionary isn't used directly but you may want to access it in your script's OnSave functions so you don't have to find all OIs in the scene again and again.
				if(SaveLoad.objectIdentifierDict.ContainsKey(oi.id) == false) {
					SaveLoad.objectIdentifierDict.Add(oi.id, oi);
				}
			}
		}

		//Go through the OIsToSerialize array and for each GameObject, call the OnSave method on each (MonoBehavior) component (if one exsists).
		//This is a good time to call any functions on the GameObject that should be called before it gets saved and serialized as part of a SaveGame.
		if(true) {
			foreach (ObjectIdentifier oi in OIsToSerialize) {
				oi.gameObject.SendMessage("OnSave",SendMessageOptions.DontRequireReceiver);
			}
		}

		//Go through the OIsToSerialize array again to pack the GameObjects into serializable form, and add the packed data to the sceneObjects list of the new SaveGame instance.
		if(true) {
			foreach (ObjectIdentifier oi in OIsToSerialize) {
				//Convert the GameObject's data into a form that can be serialized (an instance of SceneObject),
				//and add it to the SaveGame instance's list of SceneObjects.
				newSaveGame.sceneObjects.Add(PackGameObject(oi.gameObject, oi));
			}
		}

		//Call the static method that serializes our SaveGame instance and writes the data to a file.
		SaveLoad.SaveScene(newSaveGame, pathToUse);
		if(debugController.gameSaved) {
			Debug.Log("Game Saved: " + newSaveGame.savegameName + " (" + newSaveGame.saveDate + ").");
		}
	}

	//use this one for loading a SaveGame with a specific filename
	public void LoadGame(string saveGameName) {

		if(string.IsNullOrEmpty(saveGameName)) {
			Debug.Log ("[LoadGame] " + "SaveGameName is null or empty!");
			return;
		}

		//First, we will destroy all objects in the scene which are not tagged with "DontDestroy" (such as Cameras, Managers of any type, and so on... things that should persist)
		ClearScene();

		//Clear the refDict, which we will need later to reconnect GameObject and Component variables with their referenced objects
		refDict = new Dictionary<RefReconnecter,string>();

		//Declare the path where the SaveGame file will be loaded from. Of course, this shouldn't change between saving and loading the same file.
		string pathToUse = saveGamePath;
		if(usePersistentDataPath == true) {
			pathToUse = Application.persistentDataPath + "/Saved Games/";
		}
		if(string.IsNullOrEmpty(pathToUse)) {
			Debug.Log ("[LoadGame] " + "SaveGame path is null or empty!");
			return;
		}

		//Call the static method that will attempt to load the specified file and deserialize it's data into a form that we can use
		SaveGame loadedGame = SaveLoad.LoadScene(saveGameName, pathToUse);
		if(loadedGame == null) {
			Debug.Log("[LoadGame] " + "saveGameName " + saveGameName + " couldn't be found!");
			return;
		}

		//clear the SaveLoad.objectIdentifierDict which will hold all the ObjectIdentifiers whose GameObjects will be created anew from the deserialized data, with their IDs as the keys.
		//we need this dictionary later to reconnect parents with their children (or vice versa), 
		//and you might want to access it in your scripts' OnLoad function so you don't ahve to load all Ois again and again.
		SaveLoad.objectIdentifierDict = new Dictionary<string, ObjectIdentifier>();

		//iterate through the loaded SaveGame's sceneObjects list to access each stored object's data and reconstruct/unpack it with all it's components
		//Simultaneously, add the loaded GameObject's ObjectIdentifier to the SaveLoad.objectIdentifierDict.
		foreach(SceneObject loadedObject in loadedGame.sceneObjects) {
			GameObject go = UnpackGameObject(loadedObject, null);

			if(go != null) {
				//Add the reconstructed GameObject to the list we created earlier.
				ObjectIdentifier oi = go.GetComponent<ObjectIdentifier>();
				SaveLoad.objectIdentifierDict.Add(oi.id, oi);
			}
		}

		//Go through the dictionary of reconstructed GameObjects and try to reassign any missing children, and reset the localPosition
		if(true) {
			foreach(KeyValuePair<string,ObjectIdentifier> pair in SaveLoad.objectIdentifierDict) {
				ObjectIdentifier oi = pair.Value;
				if(string.IsNullOrEmpty(oi.idParent) == false) {
					if(SaveLoad.objectIdentifierDict.ContainsKey(oi.idParent)) {
						Vector3 pos = oi.transform.position;
						oi.transform.parent = SaveLoad.objectIdentifierDict[oi.idParent].transform;
						oi.transform.localPosition = pos;
					}
				}
			}
		}

		//Now comes a quite ugly part. It will only come into play if you have loaded any members (field or properties) that held references to GameObjects or Components.
		//Basically, when we went through each member (field/property) and it was a GameObject or Component reference, 
		//this field along with the referenced object's id (and some other information) is added to refDict.
		//We now go through each key of refDict and try to find to referenced object (GameObject or Component) so we can add that value to the field or property.
		if(true) {
			foreach(KeyValuePair<RefReconnecter, string> pair in refDict) {


				RefReconnecter refRec = pair.Key;
				object valueToSet = new object();
				Type fieldType = refRec.field.FieldType;

				if(debugController.currentRefRec) {
					Debug.Log("[LoadGame] " + "Current RefReconnecter: " + refRec.baseInstance.GetType().Name + "/" + refRec.field.Name + " (" + fieldType + ")");
				}

				if(refRec.collectionType == CollectionType.None) {
					ObjectIdentifier oi = SaveLoad.objectIdentifierDict[pair.Value];

					if(fieldType == typeof(GameObject)) {
						valueToSet = oi.gameObject;
					}
					else {
						Component component = oi.GetComponent(fieldType.Name.ToString());
						if(component != null) {
							valueToSet = component;
						}
					}
				}
				else {
					Type elementType = TypeSystem.GetElementType(fieldType);
					Dictionary<string,object> fieldValueDict = refRec.loadedValue as Dictionary<string,object>;

					if(refRec.collectionType == CollectionType.Array) {
						Array array = Array.CreateInstance(elementType, fieldValueDict.Count);
						foreach(KeyValuePair<string,object> pair_fvd in fieldValueDict) {
							if(pair_fvd.Value == null) {
								continue;
							}
							if(SaveLoad.objectIdentifierDict.ContainsKey(pair_fvd.Value.ToString())) {
								ObjectIdentifier oi = SaveLoad.objectIdentifierDict[pair_fvd.Value.ToString()];
								if(elementType == typeof(GameObject)) {
									array.SetValue(oi.gameObject,Int32.Parse(pair_fvd.Key));
								}
								else {
									array.SetValue(oi.GetComponent(elementType),Int32.Parse(pair_fvd.Key));
								}
							}
						}
						valueToSet = array;
					}
					else if(refRec.collectionType == CollectionType.List) {
						object list = Activator.CreateInstance(fieldType);
						MethodInfo listAddMethod = fieldType.GetMethod( "Add" );

						foreach(KeyValuePair<string,object> pair_fvd in fieldValueDict) {
							if(pair_fvd.Value == null) {
								listAddMethod.Invoke( list, new object[] {null} );
								continue;
							}
							if(SaveLoad.objectIdentifierDict.ContainsKey(pair_fvd.Value.ToString())) {
								ObjectIdentifier oi = SaveLoad.objectIdentifierDict[pair_fvd.Value.ToString()];
								if(elementType == typeof(GameObject)) {
									listAddMethod.Invoke( list, new object[] {oi.gameObject} );
								}
								else {
									listAddMethod.Invoke( list, new object[] {oi.GetComponent(elementType)} );
								}
							}
						}

						valueToSet = list;
					}
					else if(refRec.collectionType == CollectionType.Dictionary) {

						Type keyType = fieldType.GetGenericArguments()[0];
						Type valueType = fieldType.GetGenericArguments()[1];

						bool inheritsFromComponent_keyType = SaveLoad.InheritsFrom(keyType, typeof(Component));
						bool inheritsFromComponent_valueType = SaveLoad.InheritsFrom(valueType, typeof(Component));

						object dictionary = Activator.CreateInstance(fieldType);
						MethodInfo dictionaryAddMethod = fieldType.GetMethod("Add", new[] {keyType, valueType});

						ConvertedDictionary convDict = refRec.loadedValue as ConvertedDictionary;

						for(int i = 0; i < convDict.keys.Count; i++) {

							//var newKey = Activator.CreateInstance(keyType);//Can't be used since String has no such initializer
							object newKey = new object();

							if(keyType.Namespace == "System" || surrogateTypes.Contains(keyType.Name)) {
								newKey = convDict.keys[i.ToString()];
							}
							else if(keyType == typeof(GameObject) || inheritsFromComponent_keyType == true) {
								string refID = convDict.keys[i.ToString()].ToString();
								if(SaveLoad.objectIdentifierDict.ContainsKey(refID)) {
									ObjectIdentifier oi = SaveLoad.objectIdentifierDict[refID];
									if(keyType == typeof(GameObject)) {
										newKey = oi.gameObject;
									}
									else {
										if(oi.GetComponent(keyType) != null) {
											newKey = oi.GetComponent(keyType.Name);
										}
										else {
											Debug.Log("[LoadGame] " + "oi.GetComponent(" + keyType + ") == null");
										}
									}
								}
							}
							else {
								Dictionary<string,object> keyDict = convDict.keys[i.ToString()] as Dictionary<string,object>;
								SetValues(ref newKey, keyDict);
							}

							//var newValue = Activator.CreateInstance(valueType);
							object newValue = new object();

							if(valueType.Namespace == "System" || surrogateTypes.Contains(valueType.Name)) {
								newValue = convDict.values[i.ToString()];
							}
							else if(valueType == typeof(GameObject) || inheritsFromComponent_valueType == true) {
								string refID =  convDict.values[i.ToString()].ToString();
								if(SaveLoad.objectIdentifierDict.ContainsKey(refID)) {
									ObjectIdentifier oi = SaveLoad.objectIdentifierDict[refID];
									if(valueType == typeof(GameObject)) {
										newValue = oi.gameObject;
									}
									else {
										if(oi.GetComponent(valueType.Name) != null) {
											newValue = oi.GetComponent(valueType);
										}
										else {
											Debug.Log("[LoadGame] " + "oi.GetComponent(" + valueType + ") == null");
										}
									}
								}
							}
							else {
								Dictionary<string,object> valueDict = convDict.values[i.ToString()] as Dictionary<string,object>;
								SetValues(ref newValue, valueDict);
							}

							dictionaryAddMethod.Invoke(dictionary, new object[] {newKey, newValue} );
						}
						valueToSet = dictionary;
					}
				}
				if(refRec.field != null) {
					refRec.field.SetValue(refRec.baseInstance, valueToSet);
				}
				else if(refRec.property != null) {
					refRec.property.SetValue(refRec.baseInstance, valueToSet, null);
				}
			}
		}

		//This is when you might want to call any functions that should be called when a gameobject is loaded.
		//Remember that you can access the static SaveLoad.objectIdentifierDict from anywhere to access all the ObjectIdentifiers that were reconstructed after loading the game.
		if(true) {
			foreach(KeyValuePair<string,ObjectIdentifier> pair in SaveLoad.objectIdentifierDict) {
				pair.Value.gameObject.SendMessage("OnLoad",SendMessageOptions.DontRequireReceiver);
			}
		}

		if(debugController.gameLoaded) {
			Debug.Log("Game Loaded: " + loadedGame.savegameName + " (" + loadedGame.saveDate + ").");
		}
	}

	//Clear the scene of all non-persistent GameObjects so we have a clean slate
	public void ClearScene() {
		object[] obj = GameObject.FindObjectsOfType(typeof (GameObject));
		foreach (object o in obj) {
			GameObject go = (GameObject) o;

			//if the GameObject has a PersistentGameObject component, ignore it. (Cameras, Managers, etc. which should survive loading)
			//these kind of GO's shouldn't have an ObjectIdentifier component! You can save and load single components with the PackComponent and UnpackComponent methods.
			//An empty component is used to mark a GameObject as persistent so the tag is still available for other uses
			if(go.GetComponent<PersistenceMarker>() == true) {
				if(debugController.isPersistent) {
					Debug.Log("[ClearScene] " + "Keeping peristent GameObject: " + go.name);
				}
				continue;
			}
			if(debugController.destroyingGameObject) {
				Debug.Log("[ClearScene] " + "Destroying GameObject: " + go.name);
			}
			Destroy(go);
		}
	}

	public SceneObject PackGameObject(GameObject go, ObjectIdentifier oi) {

		if(debugController.packGameObject) {
			Debug.Log("[PackGameObject] " + "Converting GameObject " + go.name + " into serialiazable form.");
		}

		//Now, we create a new instance of SceneObject, which will hold all the GO's data, including it's components.
		//Note that GameObjects that should be saved should never be inactive as a whole, as they can't be found by Unity! Instead, disable relevant components like MeshRenderers and scripts.
		SceneObject sceneObject = new SceneObject();
		sceneObject.prefabName = oi.prefabName;
		sceneObject.id = oi.id;
		sceneObject.active = go.activeSelf;
		sceneObject.layer = go.layer;
		sceneObject.tag = go.tag;

		if(go.transform.parent != null) {
			ObjectIdentifier oi_parent = go.transform.parent.GetComponent<ObjectIdentifier>();
			if(oi_parent != null) {
				sceneObject.idParent = oi_parent.id;
			}
		}
		else {
			sceneObject.idParent = null;
		}

		//Get all the components attaches the the GameObject, and cycle through them
		object[] components = go.GetComponents<Component>() as object[];
		foreach(object component in components) {
			Type compType = component.GetType();

			if(debugController.checkCompSaveMode) {
				Debug.Log("[PackGameObject] " + "Checking ComponentSaveMode for Component " + compType.Name + " of GameObject " + go.name);
			}

			//ObjectIdentifier.ComponentSaveMode controls which components will be saved.
			//Normally, the Transform component as well as all MonoBehavior scripts (except ObjectIdentifier) will be saved.
			if(oi.componentSaveMode == ObjectIdentifier.ComponentSaveMode.None) {
				if(debugController.notSavingComp) {
					Debug.Log("[PackGameObject] " + "Component " + compType.Name + "of GameObject " + go.name + " will not be saved due to ComponentSaveMode " + oi.componentSaveMode.ToString());
				}
				continue;
			}
			if(compType == typeof(ObjectIdentifier)) {
				continue;
			}
			if(compType == typeof(Transform) || compType.BaseType == typeof(MonoBehaviour)) {
				if(oi.componentSaveMode == ObjectIdentifier.ComponentSaveMode.Mono 
					|| oi.componentSaveMode == ObjectIdentifier.ComponentSaveMode.MonoListInclusive) {

					sceneObject.objectComponents.Add(PackComponent(component));
					continue;
				}
			}

			if(oi.componentSaveMode == ObjectIdentifier.ComponentSaveMode.All
				|| oi.componentSaveMode == ObjectIdentifier.ComponentSaveMode.ListExclusive && oi.componentTypesToSave.Contains(compType.Name) == false
					|| oi.componentSaveMode == ObjectIdentifier.ComponentSaveMode.MonoListInclusive && oi.componentTypesToSave.Contains(compType.Name) == true
				|| oi.componentSaveMode == ObjectIdentifier.ComponentSaveMode.ListInclusive && oi.componentTypesToSave.Contains(compType.Name) == true) {

				sceneObject.objectComponents.Add(PackComponent(component));
				continue;
			}
		}
		return sceneObject;
	}

	public ObjectComponent PackComponent(object component) {

		//This will go through all the fields of a component, check each one if it is serializable, and it it should be stored,
		//and puts it into the fields dictionary of a new instance of ObjectComponent,
		//with the field's name as key and the field's value as (object)value
		//for example, if a script has the field "float myFloat = 12.5f", then the key would be "myFloat" and the value "12.5f", tha latter stored as an object.
		//If the field's value's Type.Namespace is not "System" and there is no ISerializationSurrogate, then a new Dictionary is added as the value and the cycle is repeated.

		ObjectComponent newObjectComponent = new ObjectComponent();
		newObjectComponent.fields = new Dictionary<string, object>();

		Type typeToSave = component.GetType();
		newObjectComponent.componentName = typeToSave.Name;

		if(debugController.packComponent) {
			Debug.Log("[PackComponent] " + "Attempting to convert component: " + typeToSave.Name + " into serializable form");
		}

		//SaveLoad.InheritsFrom() checks if a Type inherits from MonoBehavior. 
		//If not, that means that we need to check if there is a PropertySelector in propertySelectors for the current Component Type.
		//If there is, then only those fields and properties will be saved. 
		//This is done because Unity Components like Transform can't have their members marked with custom attributes,
		//amd they have dozens of irrelevant properties but we only need a few of them, like position and rotation.
		//MonoBehaviors in turn will have all their fields and properties saved (depending on any custom member attributes, of course).
		bool checkForPropertySelector = !SaveLoad.InheritsFrom(typeToSave, typeof(MonoBehaviour));
		GetValues(component, ref newObjectComponent.fields, checkForPropertySelector);//true if it's not a MonoBehavior component
		return newObjectComponent;
	}

	//baseInstance is the object we want to save, baseDict is the Dictionary that will hold this object's data.
	private void GetValues(object baseInstance, ref Dictionary<string,object> baseDict) {
		GetValues(baseInstance, ref baseDict, false);
	}

	private void GetValues(object baseInstance, ref Dictionary<string,object> baseDict, bool checkForPropertySelector) {

		if(baseInstance == null) {
			return;
		}

		Type baseInstanceType = baseInstance.GetType();

		if(debugController.getValuesType) {
			Debug.Log("[GetValues] " + "baseInstanceType: " + baseInstanceType.Name);
		}

		//If we have to check for a PropertySelector, then get it if availabble so we know which properties to save
		//if(propertyHandlers.Any(p => p.type == baseInstanceType.Name)) {//Kept just in case the syntax is needed in a later version
		int index = propertySelectors.FindIndex(item => item.type == baseInstanceType.Name);
		PropertySelector propertySelector = null;
		if (checkForPropertySelector == true) {
			if(index < 0) {
				if(debugController.checkPropertySelector) {
					Debug.Log("[GetValues] " + "No PropertySelector found for Type " + baseInstanceType.Name + ".");

				}
				return;
			}
			else {
				propertySelector = propertySelectors[index];
				if(debugController.checkPropertySelector) {
					Debug.Log("[GetValues] " + "PropertySelector found for Type " + baseInstanceType.Name + ".");
				}
			}
		}

		//Determine if the class of the instance has a SaveNoMembers attribute, which means that only those members with the SaveMember attribute will be saved.
		//if ther is no SaveNoMembers attribute, then all members will automatically be saved except if they have the DontSaveMember attribute.
		//What this means is that if you only want to save a few members, you give the class the SaveNoMembers attribute and the members that should be saved the SaveMember attribute,
		//and if you want to save all or almost all members, give the class no USLU attribute and give those members you don't wnat to save the DontSaveMember attribute.
		bool saveNoMembers = false;
		object[] attributes_base = baseInstanceType.GetCustomAttributes(typeof(USLUAttribute), true);
		foreach(Attribute attribute_base in attributes_base) {
			if(attribute_base.GetType() == typeof(SaveNoMembers)) {
				saveNoMembers = true;
			}
		}

		//Get all fields, and check if they should be included or excluded because of attributes or a PropertySelector
		const BindingFlags flags = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetField;
		FieldInfo[] fields = baseInstanceType.GetFields(flags);
		if(debugController.fieldsLength) {
			Debug.Log("[GetValues] " + "fields.Length: " + fields.Length);
		}
		foreach(FieldInfo field in fields) {
			Type fieldType = field.FieldType;
			string fieldName = field.Name;

			if(debugController.getValuesField) {
				Debug.Log("[GetValues] " + "field.Name: " + fieldName + " (" + fieldType.Name + ")");
			}

			if(checkForPropertySelector == true && propertySelector.exceptions.Contains(fieldName) == false) {
				continue;
			}

			object[] attributes_field = field.GetCustomAttributes(typeof(USLUAttribute), true);
			bool stop = false;

			if(saveNoMembers == true) {
				stop = true;
			}		

			foreach(Attribute attribute_field in attributes_field) {
				if(saveNoMembers == true) {
					if(attribute_field.GetType() == typeof(SaveMember)) {
						stop = false;
						break;
					}
				}
				else {
					if(attribute_field.GetType() == typeof(DontSaveMember)) {
						stop = true;
						break;
					}
				}
			}
			if(saveNoMembers == true && stop == true || saveNoMembers == false && stop == true) {
				continue;
			}

			object fieldValue = field.GetValue(baseInstance);

			//The next part is commented out because it should just show how you can directly determine how certain Types should be saved, in this case Vector3.
			/*
			if(fieldType.Name == "Vector3") {
				Dictionary<string,object> dict = new Dictionary<string, object>();
				Vector3 vector3 = (Vector3)fieldValue;
				dict.Add("x", vector3.x);
				dict.Add("y", vector3.y);
				dict.Add("z", vector3.z);
				baseDict.Add(fieldName, dict);
				continue;
			}
			*/

			//If the field made it through the gauntlet of inclusion checks, finally pass it on to the function that will save it's data for real
			WriteToDictionary(fieldType, fieldName, fieldValue, ref baseDict, true);
		}

		if(checkForPropertySelector == true) {
			PropertyInfo[] properties = baseInstanceType.GetProperties(flags);
			if(debugController.fieldsLength) {
				Debug.Log("[GetValues] " + "properties.Length: " + properties.Length);
			}
			//Do the same for properties as you did for fields. This will only be done if the instance is a component of any sorts which is not a MonoBehavior.
			foreach(PropertyInfo property in properties) {
				if(property.CanRead == true) {
					Type propertyType = property.PropertyType;
					string propertyName = property.Name;

					if(debugController.getValuesField) {
						Debug.Log("[GetValues] " + "property.Name: " + propertyName + " (" + propertyType.Name + ")");
					}

					if(checkForPropertySelector == true && propertySelector.exceptions.Contains(propertyName) == false) {
						continue;
					}

					object[] attributes_property = property.GetCustomAttributes(typeof(USLUAttribute), true);
					bool stop = false;

					if(saveNoMembers == true) {
						stop = true;
					}		

					foreach(Attribute attribute_property in attributes_property) {
						if(saveNoMembers == true) {
							if(attribute_property.GetType() == typeof(SaveMember)) {
								stop = false;
								break;
							}
						}
						else {
							if(attribute_property.GetType() == typeof(DontSaveMember)) {
								stop = true;
								break;
							}
						}
					}
					if(saveNoMembers == true && stop == true || saveNoMembers == false && stop == true) {
						continue;
					}

					object propertyValue = property.GetValue(baseInstance, null);

					/*
					if(propertyType.Name == "Vector3") {
						Dictionary<string,object> dict = new Dictionary<string, object>();
						Vector3 vector3 = (Vector3)propertyValue;
						dict.Add("x", vector3.x);
						dict.Add("y", vector3.y);
						dict.Add("z", vector3.z);
						baseDict.Add(propertyName, dict);
						continue;
					}
					*/

					WriteToDictionary(propertyType, propertyName, propertyValue, ref baseDict, false);
				}
			}
		}
	}

	//GetCollectionElement is a special function which repeats a WriteToDictionary cycle.
	//It converts the index of a collection element to a string to serve as the fieldName and passes it on to the WriteDictionary function again
	private void GetCollectionElement(Type fieldType, int index, object fieldValue, ref Dictionary<string,object> baseDict, bool isField) {
		WriteToDictionary(fieldType, index.ToString(), fieldValue, ref baseDict, isField);
	}

	private void WriteToDictionary(Type fieldType, string fieldName, object fieldValue, ref Dictionary<string,object> baseDict, bool isField) {

		if(debugController.writeFieldInfo) {
			Debug.Log("[WriteToDictionary] " + "field.Name: " + fieldName + " (" + fieldType.Name + ")" + "( Namespace: " + fieldType.Namespace + ")");
		}

		//also works for arrays like int[] or string[], but not for lists
		if(fieldType.Namespace == "System") {
			if(debugController.writeFieldType) {
				Debug.Log("[WriteToDictionary] " + fieldName + " (" + fieldType.Name + ")" + "(Namespace System).");
			}
			baseDict.Add(fieldName, fieldValue);
			if(debugController.addFieldToDict) {
				Debug.Log("[WriteToDictionary] " + "Added to baseDict: " + fieldName + " -> " + baseDict[fieldName]);
			}
			return;
		}

		//No longer needed since System Types are taken as a whole instead of their various sub-variables.
		/*
		if(field.Name.Contains("m_value")) {
			Debug.Log("[WriteToDictionary] " + fieldName + " (" + fieldType.Name + ")" + ": ignoring" + "(m_value).");
			continue;
		}
		*/

		//If a ISerializationSurrogate exists (which we have to keep track of in the surrogates list), then add the fieldValue directly.
		if(surrogateTypes.Contains(fieldType.Name)) {
			if(debugController.writeSurrogateFound) {
				Debug.Log("[WriteToDictionary] " + "Surrogate found for Type " + fieldType.Name + "(fieldName: " + fieldName + ")");
			}
			baseDict.Add(fieldName, fieldValue);
			if(debugController.addFieldToDict) {
				Debug.Log("[WriteToDictionary] " + "Added to baseDict: " + fieldName + " -> " + baseDict[fieldName]);
			}
			return;
		}

		//Check if it is a Transform first, since Transform supports Enumerators and thus will be caught later when we check for Collections. 
		//Since the check for GameObject is similar, we do it here as well.

		//If the member Type is GameObject or Component, then we will hold the id of the (hopefully) accompanying ObjectIdentifier component so we can reconstruct that reference after loading.
		if(fieldType == typeof(GameObject)) {
			GameObject go = (GameObject)fieldValue;
			if(debugController.writeFieldType) {
				Debug.Log("[WriteToDictionary] " + fieldName + " (" + fieldType.Name + ")");
			}
			if(go != null) {
				ObjectIdentifier oi = go.GetComponent<ObjectIdentifier>();
				if(oi != null) {
					baseDict.Add(fieldName, oi.id);
					if(debugController.addFieldToDict) {
						Debug.Log("[WriteToDictionary] " + "Added to baseDict: " + fieldName + " -> " + oi.id);
					}
					return;
				}
			}
			baseDict.Add(fieldName,null);
			if(debugController.addFieldToDict) {
				Debug.Log("[WriteToDictionary] " + "Added to baseDict: " + fieldName + " -> " + "null");
			}			
			return;

		}

		bool inheritsFromComponent = SaveLoad.InheritsFrom(fieldType, typeof(Component));
		if(inheritsFromComponent == true) {
			if(debugController.writeFieldType) {
				Debug.Log("[WriteToDictionary] " + fieldName + " (" + fieldType.Name + ")");
			}			

			if(fieldValue != null) {
				Component comp = (Component)fieldValue;
				if(comp != null) {
					ObjectIdentifier oi = comp.GetComponent<ObjectIdentifier>();
					if(oi != null) {
						baseDict.Add(fieldName, oi.id);
						if(debugController.addFieldToDict) {
							Debug.Log("[WriteToDictionary] " + "Added to baseDict: " + fieldName + " -> " + oi.id);
						}
					}
				}
			}
			return;
		}

		//Collections (here: Array, List and Dictionary) require special attention so the following part is a bit longer and more convoluted
		if(TypeSystem.IsEnumerableType(fieldType) == true || TypeSystem.IsCollectionType(fieldType) == true) {
			
			Type elementType = TypeSystem.GetElementType(fieldType);

			//arrays get ignored because collection of Namespace System are automatically found above. Lists are found here.
			if(elementType.Namespace == "System") {
				if(debugController.writeFieldType) {
					Debug.Log("[WriteToDictionary] " + fieldName + " is a collection with elements of Type " + elementType + "." + " (Namespace System).");
				}
				baseDict.Add(fieldName, fieldValue);
				if(debugController.addFieldToDict) {
					Debug.Log("[WriteToDictionary] " + "Added to baseDict: " + fieldName + " -> " + baseDict[fieldName]);
				}				
				return;
			}

			if(surrogateTypes.Contains(elementType.Name)) {
				if(debugController.writeSurrogateFound) {
					Debug.Log("[WriteToDictionary] " + "Surrogate found for Type " + fieldType.Name + "(fieldName: " + fieldName + ")");
				}
				baseDict.Add(fieldName, fieldValue);
				return;
			}

			if(fieldType.IsArray) {
				if(debugController.writeFieldType) {
					Debug.Log("[WriteToDictionary] " + fieldName + " is an array of Type " + elementType + "[].");
				}
				baseDict.Add(fieldName, new Dictionary<string,object>());
				Dictionary<string,object> dict = baseDict[fieldName] as Dictionary<string,object>;

				Array a = (Array)fieldValue;
				int i = 0;
				foreach(object o in a) {
					if(o != null) {
						GetCollectionElement(o.GetType(), i, o, ref dict, isField);
					}
					else {
						dict.Add(i.ToString(),null);
						if(debugController.addFieldToDict) {
							Debug.Log("[WriteToDictionary] " + "Added to baseDict: " + fieldName + " -> " + baseDict[fieldName]);
						}
					}
					i++;
				}
				return;
			}
			else if(fieldValue is IList) {
				if(debugController.writeFieldType) {
					Debug.Log("[WriteToDictionary] " + fieldName + " is a Generic List<" + elementType + ">.");
				}
				baseDict.Add(fieldName, new Dictionary<string,object>());
				Dictionary<string,object> dict = baseDict[fieldName] as Dictionary<string,object>;

				var collection = (IEnumerable) fieldValue;
				int i = 0;
				foreach(object o in collection) {
					if(o != null) {
						GetCollectionElement(o.GetType(), i, o, ref dict, isField);
					}
					else {
						dict.Add(i.ToString(),null);
						if(debugController.addFieldToDict) {
							Debug.Log("[WriteToDictionary] " + "Added to baseDict: " + fieldName + " -> " + baseDict[fieldName]);
						}
					}
					i++;
				}
				return;
			}
			else if(fieldType.IsGenericType && fieldType.GetGenericTypeDefinition() == typeof(Dictionary<,>)){
				Type keyType = fieldType.GetGenericArguments()[0];
				Type valueType = fieldType.GetGenericArguments()[1];
				if(debugController.writeFieldType) {
					Debug.Log("[WriteToDictionary] " + fieldName + " is a Dictionary<" + keyType.Name + "," + valueType.Name + ">.");
				}

				if(keyType.Namespace == "System" && valueType.Namespace == "System"
					|| keyType.Namespace == "System" && surrogateTypes.Contains(valueType.Name)
					|| surrogateTypes.Contains(keyType.Name) && valueType.Namespace == "System"
				){
					baseDict.Add(fieldName, fieldValue);
					if(debugController.addFieldToDict) {
						Debug.Log("[WriteToDictionary] " + "Added to baseDict: " + fieldName + " -> " + baseDict[fieldName]);
					}					
					return;
				}

				var keys_raw = fieldType.GetProperty("Keys", BindingFlags.Instance | BindingFlags.Public).GetValue(fieldValue, null) as IEnumerable;
				if (keys_raw == null) {
					throw new ArgumentException("Dictionary with no keys?");
				}
				object[] keys = keys_raw.OfType<object>().ToArray();

				var values_raw = fieldType.GetProperty("Values", BindingFlags.Instance | BindingFlags.Public).GetValue(fieldValue, null) as IEnumerable;
				object[] values = values_raw.OfType<object>().ToArray();

				ConvertedDictionary convDict = new ConvertedDictionary();

				for(int i = 0; i < keys.Length; i++) {
					GetCollectionElement(keyType, i, keys[i], ref convDict.keys, isField);
					GetCollectionElement(valueType, i, values[i], ref convDict.values, isField);
				}
				baseDict.Add(fieldName, convDict);
				if(debugController.addFieldToDict) {
					Debug.Log("[WriteToDictionary] " + "Added to baseDict: " + fieldName + " -> " + baseDict[fieldName]);
				}				
				return;
			}
			return;
		}

		//Any member Types that could cause trouble when serializing will pop up here.
		//These will mainly be those UnityEngine-specific Types which have no ISerializationSurrogate and hence will cause a SerializationException.
		/*
		if(fieldType.IsSerializable == false) {
			if(fieldType.Namespace == "UnityEngine") {
				Debug.Log("[WriteToDictionary] " + fieldType.Name + " (Type: " + fieldType + ") is not marked as serializable and is from UnityEngine Namespace. Continue loop.");
				return;
			}
		}
		*/

		//if the member's value can be divided further, repeat the cycle by adding a new Dictionary<string,object> to baseDict and passing those on to the GetValues function again.
		if(true) {
			baseDict.Add(fieldName, new Dictionary<string,object>());
			if(debugController.addFieldToDict) {
				Debug.Log("[WriteToDictionary] " + "Added to baseDict: " + fieldName + " -> " + baseDict[fieldName]);
			}				
			Dictionary<string,object> dict = baseDict[fieldName] as Dictionary<string,object>;
			GetValues(fieldValue, ref dict);
		}
	}

	//This method unpacks a loaded SceneObject instance into a GameObject. Note that this method can be used in two ways:
	//1. Pass a GameObject (parameter "go") to fill an existing GameObject in the scene with data from a saved one
	//2. Don't pass a GameObject (parameter "go" = null) which makes the method attempt to instantiate a new GameObject.
	public GameObject UnpackGameObject(SceneObject sceneObject, GameObject go) {

		//instantiate the gameObject if needed
		if(go == null) {
			//This is where our prefabDictionary above comes in. Each GameObject that was saved needs to be reconstucted, so we need a Prefab,
			//and we know which prefab it is because we stored the GameObject's prefab name in it's ObjectIdentifier/SceneObject script/class.
			//Theoretically, we could even reconstruct GO's that have no prefab by instatiating an empty GO and filling it with the required components... I'lll leave that to you.
			if(prefabDictionary.ContainsKey(sceneObject.prefabName) == false) {
				Debug.Log("[UnpackGameObject] " + "Can't find key " + sceneObject.prefabName + " in SaveLoadUtility.prefabDictionary!");
				return null;
			}
			go = Instantiate(prefabDictionary[sceneObject.prefabName], Vector3.zero, Quaternion.identity) as GameObject;

			if(debugController.instantiate) {
				Debug.Log("[UnpackGameObject] " + "Instantiated GameObject " + go.name + "(prefabName: " + sceneObject.prefabName + ")");
			}
		}

		//Reassign values
		//Note that GameObjects that should be saved should never be inactive as a whole, as they can't be found by Unity! Instead, disable relevant components like MeshRenderers and scripts.
		go.name = sceneObject.prefabName;
		go.layer = sceneObject.layer;
		go.tag = sceneObject.tag;
		go.SetActive (sceneObject.active);


		//Go through the stored object's component list and reassign all values in each component
		foreach(ObjectComponent obc in sceneObject.objectComponents) {
			UnpackComponent(ref go, obc);
		}

		ObjectIdentifier oi = go.GetComponent<ObjectIdentifier>();
		if(oi != null) {
			oi.id = sceneObject.id;
			oi.idParent = sceneObject.idParent;

			//Destroy any children (which have an ObjectIdentifier component) that were not referenced as having a parent
			ObjectIdentifier[] oi_children = go.GetComponentsInChildren<ObjectIdentifier>();
			foreach(ObjectIdentifier oi_child in oi_children) {
				if(oi_child.gameObject != go) {
					if(string.IsNullOrEmpty(oi_child.id) == true) {
						Destroy (oi_child.gameObject);
					}
				}
			}
		}

		return go;
	}

	//Rconstruct a component from it's loaded ObjectComponentData, passing it on to an existing GameObject. 
	//If the GameObject has no such component, then one will be added.
	public void UnpackComponent(ref GameObject go, ObjectComponent obc) {

		if(debugController.unpackComponent) {
			Debug.Log("[UnpackComponent] " + "Unpacking Component: " + obc.componentName + " on GameObject " + go.name);
		}

		//add components that are missing
		if(go.GetComponent(obc.componentName) == null) {
			Type componentType = Type.GetType(obc.componentName);
			go.AddComponent(componentType);
		}

		//Unpack the component
		object obj = go.GetComponent(obc.componentName) as object;
		SetValues(ref obj, obc.fields);
	}

	private void SetValues(ref object baseInstance, Dictionary<string,object> baseDict) {

		Type baseInstanceType = baseInstance.GetType();

		if(debugController.setValuesType) {
			Debug.Log("[SetValues] " + "baseInstanceType: " + baseInstanceType);
		}

		foreach(KeyValuePair<string,object> pair in baseDict) {

			FieldInfo field = baseInstanceType.GetField(pair.Key,BindingFlags.Instance 
				| BindingFlags.Public 
				| BindingFlags.NonPublic 
				| BindingFlags.SetField);
			if(field != null) {
				Type fieldType = field.FieldType;

				if(debugController.setValuesField) {
					Debug.Log("[SetValues] " + baseInstanceType.Name + "/" + field.Name + " (" + fieldType.Name + ")");
				}

				//The next part is commented out because it should just show how you can directly determine how certain Types should be loaded, in this case Vector3.
				/*
				if(fieldType.Name == "Vector3") {
					Dictionary<string,object> dict = pair.Value as Dictionary<string,object>;
					field.SetValue(baseInstance, new Vector3((float)dict["x"], (float)dict["y"], (float)dict["z"]));
				}
				*/

				ReadFromDictionary(fieldType, pair.Key, pair.Value, field, null,  baseInstance);
			}
			else {							
				PropertyInfo property = baseInstanceType.GetProperty(pair.Key);
				if(property != null) {
					if(property.CanWrite == true) {
						Type propertyType = property.PropertyType;

						if(debugController.setValuesField) {
							Debug.Log("[SetValues] " + baseInstanceType.Name + "/" + property.Name + " (" + propertyType.Name + ")");
						}
						/*
						if(fieldType.Name == "Vector3") {
							Dictionary<string,object> dict = pair.Value as Dictionary<string,object>;
							property.SetValue(baseInstance, new Vector3((float)dict["x"], (float)dict["y"], (float)dict["z"]), null);
						}
						*/

						string propertyName = property.Name;
						object propertyValue = property.GetValue(baseInstance, null);
						ReadFromDictionary(propertyType, pair.Key, pair.Value, null, property, baseInstance);
					}
				}
			}
		}
	}

	//Assign values to fields and properties from a Dictionary
	private void ReadFromDictionary(Type fieldType, string fieldName, object fieldValue, FieldInfo baseInstanceField, PropertyInfo baseInstanceProperty, object baseInstance) {

		object baseInstanceValue = null;
		if(baseInstanceField != null) {
			baseInstanceValue = baseInstanceField.GetValue(baseInstance);
		}
		else if(baseInstanceProperty != null) {
			baseInstanceValue = baseInstanceProperty.GetValue(baseInstance, null);
		}

		if(fieldValue == null) {
			if(baseInstanceField != null) {
				baseInstanceField.SetValue(baseInstance, null);
			}
			else if(baseInstanceProperty != null){
				baseInstanceProperty.SetValue(baseInstance, null, null);
			}

			if(debugController.readFromDict) {
				Debug.Log("[ReadFromDictionary] " + "Read from baseDict: " + fieldName + " -> " + "null");
			}
			return;
		}
		if(debugController.readFieldInfo) {
			Debug.Log("[ReadFromDictionary] " + fieldName + " -> " + fieldValue.ToString() + "(" + fieldType.Name + ")" + "(" + fieldType.Namespace + ")");
		}

		//also works for arrays like int[] or string[], but not for lists
		if(fieldType.Namespace == "System") {
			if(debugController.readFieldType) {
				Debug.Log("[ReadFromDictionary] " + fieldName + " (" + fieldType.Name + ")" + "(Namespace System).");
			}
			if(baseInstanceField != null) {
				baseInstanceField.SetValue(baseInstance, fieldValue);
			}
			else if(baseInstanceProperty != null){
				baseInstanceProperty.SetValue(baseInstance, fieldValue, null);
			}

			if(debugController.readFromDict) {
				Debug.Log("[ReadFromDictionary] " + "Read from baseDict: " + fieldName + " -> " + fieldValue.ToString() + "(" + fieldType.Name + ")" + "(" + fieldType.Namespace + ")");
			}
			return;
		}

		//No longer needed since System Types are taken as a whole instead of their various sub-variables.
		/*
		if(field.Name.Contains("m_value")) {
			Debug.Log(fieldName + " (" + fieldType.Name + ")" + ": ignoring" + "(m_value).");
			continue;
		}
		*/

		Type baseInstanceType = baseInstance.GetType();
		Dictionary<string,object> fieldValueDict = fieldValue as Dictionary<string,object>;

		if(surrogateTypes.Contains(fieldType.Name)) {
			if(debugController.writeSurrogateFound) {
				Debug.Log("[WriteToDictionary] " + "Surrogate found for Type " + fieldType.Name + "(fieldName: " + fieldName + ")");
			}
			if(baseInstanceField != null) {
				baseInstanceField.SetValue(baseInstance, fieldValue);
			}
			else if(baseInstanceProperty != null) {
				baseInstanceProperty.SetValue(baseInstance, fieldValue, null);
			}

			if(debugController.readFromDict) {
				Debug.Log("[ReadFromDictionary] " + "Read from baseDict: " + fieldName + " -> " + fieldValue.ToString() + "(" + fieldType.Name + ")" + "(" + fieldType.Namespace + ")");
			}
			return;
		}

		bool inheritsFromComponent = SaveLoad.InheritsFrom(fieldType, typeof(Component));
		if(fieldType == typeof(GameObject) || inheritsFromComponent == true) {
			if(fieldValue == null) {
				if(baseInstanceField != null) {
					baseInstanceField.SetValue(baseInstance, null);
				}
				else if(baseInstanceProperty != null){
					baseInstanceProperty.SetValue(baseInstance, null, null);
				}

				if(debugController.readFromDict) {
					Debug.Log("[ReadFromDictionary] " + "Read from baseDict: " + fieldName + " -> " + "null");
				}
				return;
			}
			RefReconnecter refReconnecter = new RefReconnecter() {baseInstance = baseInstance, field = baseInstanceField, property = baseInstanceProperty};
			if(refDict.ContainsKey(refReconnecter) == false) {
				refDict.Add(refReconnecter, fieldValue.ToString());

				if(debugController.readFromDict) {
					Debug.Log("[ReadFromDictionary] " + "Read from baseDict: " + fieldName + " -> " + fieldValue.ToString() + "(" + fieldType.Name + ")" + "(" + fieldType.Namespace + ")");
				}
			}
			return;
		}

		if(TypeSystem.IsEnumerableType(fieldType) == true || TypeSystem.IsCollectionType(fieldType) == true) {
			Type elementType = TypeSystem.GetElementType(fieldType);

			if(debugController.readFieldType) {
				Debug.Log("[ReadFromDictionary] " + fieldName + " (" + fieldType.Name + ")" + "is a collection.");
			}

			if(surrogateTypes.Contains(elementType.Name)) {
				if(baseInstanceField != null) {
					baseInstanceField.SetValue(baseInstance, fieldValue);
				}
				else if(baseInstanceProperty != null) {
					baseInstanceProperty.SetValue(baseInstance, fieldValue, null);
				}

				if(debugController.readFromDict) {
					Debug.Log("[ReadFromDictionary] " + "Read from baseDict: " + fieldName + " -> " + fieldValue.ToString() + "(" + fieldType.Name + ")" + "(" + fieldType.Namespace + ")");
				}
				return;
			}

			//arrays get ignored because collection of Namespace System are automatically found above. Lists are found here.
			if(elementType.Namespace == "System") {
				if(baseInstanceField != null) {
					baseInstanceField.SetValue(baseInstance, fieldValue);
				}
				else if(baseInstanceProperty != null){
					baseInstanceProperty.SetValue(baseInstance, fieldValue, null);
				}

				if(debugController.readFromDict) {
					Debug.Log("[ReadFromDictionary] " + "Read from baseDict: " + fieldName + " -> " + fieldValue.ToString() + "(" + fieldType.Name + ")" + "(" + fieldType.Namespace + ")");
				}
				return;
			}

			bool inheritsFromComponent_element = SaveLoad.InheritsFrom(elementType, typeof(Component));

			if(fieldType.IsArray) {

				if(debugController.readFieldType) {
					Debug.Log("[ReadFromDictionary] " + fieldName + " is an array of Type " + elementType + "[]");
				}

				if(elementType == typeof(GameObject) || inheritsFromComponent_element == true) {
					RefReconnecter refReconnecter = new RefReconnecter() {baseInstance = baseInstance, field = baseInstanceField, property = baseInstanceProperty, collectionType = CollectionType.Array, loadedValue = fieldValue};
					refDict.Add(refReconnecter, null);
					return;
				}

				Array array = Array.CreateInstance(elementType, fieldValueDict.Count);
				for(int i = 0; i < fieldValueDict.Count; i++) {
					var newElement = Activator.CreateInstance(elementType);
					array.SetValue(newElement,i);
				}

				foreach(KeyValuePair<string,object> pair in fieldValueDict) {
					Dictionary<string,object> elementDict = pair.Value as Dictionary<string, object>;
					if(elementDict == null) {
						array.SetValue(pair.Value,Int32.Parse(pair.Key));
					}
					else {
						var e = array.GetValue(Int32.Parse(pair.Key));
						SetValues(ref e, elementDict);
						array.SetValue(e,Int32.Parse(pair.Key));
					}
				}
				if(baseInstanceField != null) {
					baseInstanceField.SetValue(baseInstance, array);
				}
				else if(baseInstanceProperty != null){
					baseInstanceProperty.SetValue(baseInstance, array, null);
				}

				if(debugController.readFromDict) {
					Debug.Log("[ReadFromDictionary] " + "Read from baseDict: " + fieldName + " -> " + fieldValue.ToString() + "(" + fieldType.Name + ")" + "(" + fieldType.Namespace + ")");
				}
				return;
			}
			else if(baseInstanceValue is IList) {
				//else if(fieldType.IsGenericType && fieldType.GetGenericTypeDefinition() == typeof(List<>)) {

				if(debugController.readFieldType) {
					Debug.Log("[ReadFromDictionary] " + fieldName + " is a Generic List<" + elementType + ">.");
				}

				if(elementType == typeof(GameObject) || inheritsFromComponent_element == true) {
					RefReconnecter refReconnecter = new RefReconnecter() {baseInstance = baseInstance, field = baseInstanceField, property = baseInstanceProperty, collectionType = CollectionType.List, loadedValue = fieldValue};
					refDict.Add(refReconnecter, null);
					return;
				}

				object list = Activator.CreateInstance( fieldType );
				MethodInfo listAddMethod = fieldType.GetMethod( "Add" );

				foreach(KeyValuePair<string,object> pair in fieldValueDict) {

					Dictionary<string,object> elementDict = pair.Value as Dictionary<string, object>;

					if(elementDict == null) {
						listAddMethod.Invoke(list, new object[] {pair.Value} );
					}
					else {
						var newElement = Activator.CreateInstance(elementType);
						SetValues(ref newElement, elementDict);
						listAddMethod.Invoke(list, new object[] {newElement} );
					}
				}
				if(baseInstanceField != null) {
					baseInstanceField.SetValue(baseInstance, list);
				}
				else if(baseInstanceProperty != null){
					baseInstanceProperty.SetValue(baseInstance, list, null);
				}

				if(debugController.readFromDict) {
					Debug.Log("[ReadFromDictionary] " + "Read from baseDict: " + fieldName + " -> " + fieldValue.ToString() + "(" + fieldType.Name + ")" + "(" + fieldType.Namespace + ")");
				}

				return;
			}
			else if(fieldType.IsGenericType && fieldType.GetGenericTypeDefinition() == typeof(Dictionary<,>)){

				//IDictionary collection = (IDictionary) baseInstanceValue;

				Type keyType = fieldType.GetGenericArguments()[0];
				Type valueType = fieldType.GetGenericArguments()[1];

				if(debugController.readFieldType) {
					Debug.Log("[ReadFromDictionary] " + fieldName + " is a Dictionary <" + keyType.Name + "," + valueType.Name + ">.");
				}

				if(keyType.Namespace == "System" && valueType.Namespace == "System"
					|| keyType.Namespace == "System" && surrogateTypes.Contains(valueType.Name)
					|| surrogateTypes.Contains(keyType.Name) && valueType.Namespace == "System"
					) {
					baseInstanceField.SetValue(baseInstance, fieldValue);

					if(debugController.readFromDict) {
						Debug.Log("[ReadFromDictionary] " + "Read from baseDict: " + fieldName + " -> " + fieldValue.ToString() + "(" + fieldType.Name + ")" + "(" + fieldType.Namespace + ")");
					}					
					return;
				}

				bool inheritsFromComponent_keyType = SaveLoad.InheritsFrom(keyType, typeof(Component));
				bool inheritsFromComponent_valueType = SaveLoad.InheritsFrom(valueType, typeof(Component));

				object dictionary = Activator.CreateInstance(fieldType);
				MethodInfo dictionaryAddMethod = fieldType.GetMethod("Add", new[] {keyType, valueType});

				if(keyType == typeof(GameObject) || inheritsFromComponent_keyType == true || valueType == typeof(GameObject) || inheritsFromComponent_valueType == true) {
					RefReconnecter refReconnecter = new RefReconnecter() {baseInstance = baseInstance, field = baseInstanceField, property = baseInstanceProperty, collectionType = CollectionType.Dictionary, loadedValue = fieldValue};
					refDict.Add(refReconnecter, null);

					if(debugController.readFromDict) {
						Debug.Log("[ReadFromDictionary] " + "Read from baseDict: " + fieldName + " -> " + fieldValue.ToString() + "(" + fieldType.Name + ")" + "(" + fieldType.Namespace + ")");
					}
					return;
				}
				ConvertedDictionary convDict = fieldValue as ConvertedDictionary;
				for(int i = 0; i < convDict.keys.Count; i++) {

					//var newKey = Activator.CreateInstance(keyType);
					object newKey = new object();

					if(keyType == typeof(string)) {
						newKey = (string)"";
					}
					else {
						newKey = Activator.CreateInstance(keyType);
					}

					if(keyType.Namespace == "System") {
						newKey = convDict.keys[i.ToString()];
					}
					else {
						Dictionary<string,object> keyDict = convDict.keys[i.ToString()] as Dictionary<string,object>;
						SetValues(ref newKey, keyDict);
					}

					var newValue = Activator.CreateInstance(valueType);
					if(valueType.Namespace == "System") {
						newValue = convDict.values[i.ToString()];
					}
					else {
						Dictionary<string,object> valueDict = convDict.values[i.ToString()]  as Dictionary<string,object>;
						SetValues(ref newValue, valueDict);
					}

					dictionaryAddMethod.Invoke(dictionary, new object[] {newKey, newValue} );
				}
				if(baseInstanceField != null) {
					baseInstanceField.SetValue(baseInstance, dictionary);
				}
				else if(baseInstanceProperty != null){
					baseInstanceProperty.SetValue(baseInstance, dictionary, null);
				}

				if(debugController.readFromDict) {
					Debug.Log("[ReadFromDictionary] " + "Read from baseDict: " + fieldName + " -> " + fieldValue.ToString() + "(" + fieldType.Name + ")" + "(" + fieldType.Namespace + ")");
				}
				return;
			}
			return;
		}

		if(true) {
			SetValues(ref baseInstanceValue, fieldValueDict);
		}
	}
}

