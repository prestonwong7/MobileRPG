using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TestScript : MonoBehaviour {

	[DontSaveMember]public int ignoredVariable = -1;//This field will not be saved due to the DontSaveMember attribute
	[DontSaveMember]public int myInt = 12;
	[DontSaveMember]public string myString = "Test123";
	[DontSaveMember]public Vector3 myVector3 = new Vector3(12.5f, -54.23f, 4f);
	[DontSaveMember]public TestClass myTestClass = new TestClass();
	[DontSaveMember]public GameObject myGameObject;
	[DontSaveMember]public Transform myTransform;
	[DontSaveMember]public TestScript myTestScript;

	[DontSaveMember]public int[] myIntArray = new int[] {2,-6,87};
	[DontSaveMember]public string[] myStringArray = new string[] {"Test123", "Hello World!"};
	[DontSaveMember]public Vector3[] myVector3Array = new Vector3[]{Vector3.up, Vector3.left};
	[DontSaveMember]public TestClass[] myTestClassArray = new TestClass[] {new TestClass(), new TestClass()};
	[DontSaveMember]public GameObject[] myGameObjectArray = new GameObject[0];
	[DontSaveMember]public Transform[] myTransformArray = new Transform[0];
	[DontSaveMember]public TestScript[] myTestScriptArray = new TestScript[0];

	[DontSaveMember]public List<int> myIntList = new List<int>() {1,-8,47};
	[DontSaveMember]public List<string> myStringList = new List<string>() {"Test123", "Hello World!"};
	[DontSaveMember]public List<Vector3> myVector3List = new List<Vector3>() {Vector3.up, Vector3.left};
	[DontSaveMember]public List<TestClass> myTestClassList = new List<TestClass>(){new TestClass(), new TestClass()};
	[DontSaveMember]public List<GameObject> myGameObjectList = new List<GameObject>();
	[DontSaveMember]public List<Transform> myTransformList = new List<Transform>();
	[DontSaveMember]public List<TestScript> myTestScriptList = new List<TestScript>();
	public List<List<Transform>> myIntListList = new List<List<Transform>>();

	void Start() {

	}

	void Update() {
		/*
		if(Input.GetKeyDown(KeyCode.Space)) {
			List<Transform> trList = new List<Transform>() {transform};
			if(myIntListList.Contains(trList) == false) {
				myIntListList.Add(trList);
			}
			Debug.Log("added");
		}
		if(Input.GetKeyDown(KeyCode.Return)) {
			int i = 0;
			Debug.Log("myIntListList.Count: " + myIntListList.Count);
			foreach(List<Transform> list in myIntListList) {
				Debug.Log("list.Count: " + list.Count);
				int j = 0;
				foreach(Transform tr in list) {
					Debug.Log(i + "," + j + ": " + tr);
					j++;
				}
				i++;
			}
		}
		*/
	}

	void OnEnable() {
		Debug.Log("Awake: " + name);
	}

	void OnDestroy() {
		Debug.Log("Destroyed: " + name);
	}

	public void OnSave() {
        //Use SaveLoad.objectIdentifierDict to access all ObjectIdentifers that were found in the scene when saving the game. Key: id, value: ObjectIdentifier
        //SaveLoad.objectIdentifierDict()
    }

	public void OnLoad() {
		//Use SaveLoad.objectIdentifierDict to access all ObjectIdentifers that were reconstructed when loading the game. Key: id, value: ObjectIdentifier

	}

}

