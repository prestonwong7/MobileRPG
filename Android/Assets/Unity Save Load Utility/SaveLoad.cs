using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Runtime.Serialization;
using System.Linq;

public static class SaveLoad {

	public static Dictionary<string,ObjectIdentifier> objectIdentifierDict;//holds references to all ObjectIdentifier components from GameObjects before saving and loaded GameObjects after loading a scene. key: id, value: ObjectIdentifier
	//also useful if you need access to all other OIs in one of your script's OnSave and OnLoad functions, so you don't have to use FindObjectOfType again and again.

	public static void SaveScene(SaveGame saveGame, string saveGamePath) {

		BinaryFormatter bf = new BinaryFormatter();
		
		// 1. Construct a SurrogateSelector object
		SurrogateSelector ss = new SurrogateSelector();
		// 2. Add the ISerializationSurrogates to our new SurrogateSelector
		AddSurrogates(ref ss);
		// 3. Have the formatter use our surrogate selector
		bf.SurrogateSelector = ss;
		
		//Application.persistentDataPath is a string, so if you wanted you can put that into debug.log if you want to know where save games are located
		//You can also use any path you like
		CheckDirectory(saveGamePath);
		saveGamePath = CheckPath(saveGamePath, false);

		FileStream file = File.Create (saveGamePath + saveGame.savegameName + ".sav"); //you can call it anything you want including the file extension
		bf.Serialize(file, saveGame);
		file.Close();
	}	
	
	public static SaveGame LoadScene(string gameToLoad, string saveGamePath) {

		saveGamePath = CheckPath(saveGamePath, false);

		if(File.Exists(saveGamePath + gameToLoad + ".sav")) {

			BinaryFormatter bf = new BinaryFormatter();
			// 1. Construct a SurrogateSelector object
			SurrogateSelector ss = new SurrogateSelector();
			// 2. Add the ISerializationSurrogates to our new SurrogateSelector
			AddSurrogates(ref ss);
			// 3. Have the formatter use our surrogate selector
			bf.SurrogateSelector = ss;
			
			FileStream file = File.Open(saveGamePath + gameToLoad + ".sav", FileMode.Open);
			SaveGame loadedGame = (SaveGame)bf.Deserialize(file);
			file.Close();
			return loadedGame;
		}
		else {
			Debug.Log(gameToLoad + " does not exist!");
			return null;
		}
	}

	public static void SaveGameObject(SceneObject gameObjectToSave, string path, string fileName) {

		BinaryFormatter bf = new BinaryFormatter();

		// 1. Construct a SurrogateSelector object
		SurrogateSelector ss = new SurrogateSelector();
		// 2. Add the ISerializationSurrogates to our new SurrogateSelector
		AddSurrogates(ref ss);
		// 3. Have the formatter use our surrogate selector
		bf.SurrogateSelector = ss;

		//Application.persistentDataPath is a string, so if you wanted you can put that into debug.log if you want to know where save games are located
		//You can also use any path you like
		CheckDirectory(path);

		FileStream file = File.Create (path + fileName + ".go"); //you can call it anything you want including the file extension
		bf.Serialize(file, gameObjectToSave);
		file.Close();
	}	

	public static SceneObject LoadGameObject(string fileName, string path) {
		if(File.Exists(path + fileName + ".go")) {

			BinaryFormatter bf = new BinaryFormatter();
			// 1. Construct a SurrogateSelector object
			SurrogateSelector ss = new SurrogateSelector();
			// 2. Add the ISerializationSurrogates to our new SurrogateSelector
			AddSurrogates(ref ss);
			// 3. Have the formatter use our surrogate selector
			bf.SurrogateSelector = ss;

			FileStream file = File.Open(path + fileName + ".go", FileMode.Open);
			SceneObject loadedSceneObject = (SceneObject)bf.Deserialize(file);
			file.Close();
			return loadedSceneObject;
		}
		else {
			Debug.Log(fileName + " does not exist!");
			return null;
		}
	}
	
	public static void AddSurrogates(ref SurrogateSelector ss) {

		Vector2Surrogate Vector2_SS = new Vector2Surrogate();
		ss.AddSurrogate(typeof(Vector2), 
			new StreamingContext(StreamingContextStates.All), 
			Vector2_SS);
		
		Vector3Surrogate Vector3_SS = new Vector3Surrogate();
		ss.AddSurrogate(typeof(Vector3), 
		                new StreamingContext(StreamingContextStates.All), 
		                Vector3_SS);
		
		Vector4Surrogate Vector4_SS = new Vector4Surrogate();
		ss.AddSurrogate(typeof(Vector4), 
			new StreamingContext(StreamingContextStates.All), 
			Vector4_SS);
		
		QuaternionSurrogate Quaternion_SS = new QuaternionSurrogate();
		ss.AddSurrogate(typeof(Quaternion), 
		                new StreamingContext(StreamingContextStates.All), 
		                Quaternion_SS);
		
		ColorSurrogate Color_SS = new ColorSurrogate();
		ss.AddSurrogate(typeof(Color), 
			new StreamingContext(StreamingContextStates.All), 
			Color_SS);
		
		Color32Surrogate Color32_SS = new Color32Surrogate();
		ss.AddSurrogate(typeof(Color32), 
			new StreamingContext(StreamingContextStates.All), 
			Color32_SS);
	}

	private static string CheckPath(string path, bool usePersistentDataPath) {

		if(usePersistentDataPath == true) {
			return Application.persistentDataPath + "/Saved Games/";
		}

		path = path.Replace("\\", "/");

		if(path.EndsWith("/") == false) {
			path += "/";
		}

		return path;
	}

	private static void CheckDirectory(string path) {
		try {
			// Determine whether the directory exists. 
			if (Directory.Exists(path)) {
				//Debug.Log("That path exists already.");
				return;
			}
			
			// Try to create the directory.
			DirectoryInfo dir = Directory.CreateDirectory(path);
			Debug.Log("The directory was created successfully at " + path);

		} 
		catch (Exception e) {
			Debug.Log("The process failed: " + e.ToString());
		} 
		finally {}
	}

	public static bool InheritsFrom(Type type, Type baseType)
	{
		// null does not have base type
		if (type == null)
		{
			return false;
		}

		// only interface can have null base type
		if (baseType == null)
		{
			return type.IsInterface;
		}

		// check implemented interfaces
		if (baseType.IsInterface)
		{
			return type.GetInterfaces().Contains(baseType);
		}

		// check all base types
		var currentType = type;
		int i  = 0;
		while (currentType != null)
		{
			//Debug.Log(i + ": " + currentType);
			if (currentType.BaseType == baseType)
			{
				return true;
			}

			currentType = currentType.BaseType;
			i++;
		}

		return false;
	}

	public static List<SaveGame> GetSaveGames(string path, bool usePersistentDataPath) {
		List<SaveGame> saveGames = new List<SaveGame>();

		SurrogateSelector ss = new SurrogateSelector();
		AddSurrogates(ref ss);

		path = CheckPath(path, usePersistentDataPath);

		if (Directory.Exists(path) == false) {
			Debug.Log("[GetSaveGames/GetSaveGames] " + "Path not found: " + path);
			return saveGames;
		}

		DirectoryInfo dir = new DirectoryInfo(path);
		List<FileInfo> info = dir.GetFiles("*.sav", SearchOption.AllDirectories).ToList();
		info.OrderBy(x => x.CreationTime);

		foreach (FileInfo f in info) {
			BinaryFormatter bf = new BinaryFormatter();
			bf.SurrogateSelector = ss;
			FileStream file = File.Open(path + f.Name, FileMode.Open);
			SaveGame loadedGame = (SaveGame)bf.Deserialize(file);
			file.Close();
			saveGames.Add(loadedGame);
		}
		return saveGames;
	}

	public static void DeleteFile(string path, string fileName) {
		if(File.Exists(path + fileName + ".sav")) {
			File.Delete(path + fileName + ".sav");
		}
	}
}
