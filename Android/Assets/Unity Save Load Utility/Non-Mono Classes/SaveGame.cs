using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class SaveGame {
	
	public string savegameName = "New SaveGame";
	public string saveDate;
	public List<SceneObject> sceneObjects = new List<SceneObject>();

	public SaveGame() {

	}

	public SaveGame(string s, string d, List<SceneObject> list) {
		savegameName = s;
		saveDate = d;
		sceneObjects = list;
	}
}
