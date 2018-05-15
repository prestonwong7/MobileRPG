using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public class SaveLoadMenu : MonoBehaviour {

	public SaveLoadUtility slu;
	public bool showMenu = true;
	public bool showSave = false;
	public bool showLoad = false;
	private string saveGameName;
	private int selectedSaveGameIndex = -99;
	public List<SaveGame> saveGames;
	private char[] newLine = "\n\r".ToCharArray();

	private Regex regularExpression = new Regex("^[a-zA-Z0-9_\"  *\"]*$");
	/*Regular expression, contains only upper and lowercase letters, numbers, and underscores.
 
          * ^ : start of string
         [ : beginning of character group
         a-z : any lowercase letter
         A-Z : any uppercase letter
         0-9 : any digit
         _ : underscore
         ] : end of character group
         * : zero or more of the given characters
         $ : end of string
 
     */


	void Start() {
		if(slu == null) {
			slu = GetComponent<SaveLoadUtility>();
			if(slu == null) {
				Debug.Log("[SaveLoadMenu] Start(): Warning! SaveLoadUtility not assigned!");
			}
		}
	}

	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(KeyCode.Escape)) {

			if(showLoad == true) {
				showLoad = false;
				showMenu = true;
				return;
			}

			if(showSave == true) {
				if(selectedSaveGameIndex != -99) {
					selectedSaveGameIndex = -99;
					Debug.Log("sdf");
				}
				else {
					showSave = false;
					showMenu = true;
				}
				return;
			}

			if(showMenu == true) {
				showMenu = false;
				return;
			}
			else {
				if(showLoad == false || showSave == false) {
					showMenu = true;
					return;
				}
			}
		}



		//The classic hotkeys for quicksaving and quickloading
		if(Input.GetKeyDown(KeyCode.F5)) {
			slu.SaveGame(slu.quickSaveName);//Use this for quicksaving, which is basically just using a constant savegame name.
		}

		if(Input.GetKeyDown(KeyCode.F9)) {
			slu.LoadGame(slu.quickSaveName);//Use this for quickloading, which is basically just using a constant savegame name.

		}
	}


	void OnGUI() {

		if(showMenu == false && showLoad == false && showSave == false) {
			if(GUILayout.Button("Menu")) {
				showMenu = true;
				return;
			}
		}

		if(showMenu == true) {
			GUILayout.BeginVertical(GUILayout.MinWidth(300)); 

			if(GUILayout.Button("Save")) {
				showMenu = false;
				showLoad = false;
				saveGames = SaveLoad.GetSaveGames(slu.saveGamePath, slu.usePersistentDataPath);
				showSave = true;

				return;
			}

			if(GUILayout.Button("Load")) {
				showSave = false;
				showMenu = false;
				saveGames = SaveLoad.GetSaveGames(slu.saveGamePath, slu.usePersistentDataPath);
				if(saveGames.Count >= 0) {
					showLoad = true;
				}
				else {
					showMenu = true;
				}
				return;
			}

			if(GUILayout.Button("Close")) {
				showSave = false;
				showMenu = false;
				showLoad = false;
				return;
			}

			if(GUILayout.Button("Exit to Windows")) {
				Application.Quit();
				return;
			}

			GUILayout.FlexibleSpace();
			GUILayout.EndVertical();
		}
		if(showLoad == true) {
			GUILayout.BeginVertical(GUILayout.MinWidth(300)); 

			foreach(SaveGame saveGame in saveGames) {
				if(GUILayout.Button(saveGame.savegameName + " (" + saveGame.saveDate + ")")) {
					slu.LoadGame(saveGame.savegameName);
					showLoad = false;
					return;
				}
			}

			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			if(GUILayout.Button("Back", GUILayout.MaxWidth(100))) {
				showLoad = false;
				showMenu = true;
			}
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();

			GUILayout.FlexibleSpace();
			GUILayout.EndVertical();
		}
		if(showSave == true) {

			GUILayout.BeginVertical(GUILayout.MinWidth(300));

			for(int i = -1; i < saveGames.Count; i++) {

				if(i == selectedSaveGameIndex) {

					GUILayout.BeginHorizontal(GUILayout.MinWidth(300));

					string str = GUILayout.TextField(saveGameName, GUILayout.MinWidth(200));

					if(regularExpression.IsMatch(str)){
						if(str.IndexOfAny(newLine) != -1) {
							//New Line detected
							if(i >= 0) {
								SaveLoad.DeleteFile(slu.saveGamePath, saveGames[i].savegameName);
							}
							slu.SaveGame(saveGameName);
							selectedSaveGameIndex = -99;
							return;
						}
						else {
							saveGameName = str; //All OK, copy
						}
					}
					else {
						Debug.Log("Irregular expression detected");
					}

					GUILayout.FlexibleSpace();
					if(GUILayout.Button("Save", GUILayout.MaxWidth(50))) {
						if(i >= 0) {
							SaveLoad.DeleteFile(slu.saveGamePath, saveGames[i].savegameName);
						}
						slu.SaveGame(saveGameName);
						selectedSaveGameIndex = -99;
						saveGames = SaveLoad.GetSaveGames(slu.saveGamePath, slu.usePersistentDataPath);
						return;
					}

					if(GUILayout.Button("Cancel", GUILayout.MaxWidth(50))) {
						selectedSaveGameIndex = -99;
						return;
					}
					GUILayout.EndHorizontal();
				}
				else {
					if(i == -1) {
						if(GUILayout.Button("(New)")) {
							selectedSaveGameIndex = i;
							saveGameName = "";
							return;
						}
					}
					else {
						if(GUILayout.Button(saveGames[i].savegameName + " (" + saveGames[i].saveDate + ")")) {
							selectedSaveGameIndex = i;
							saveGameName = saveGames[i].savegameName;
							return;
						}
					}
				}
			}

			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			if(GUILayout.Button("Back", GUILayout.MaxWidth(100))) {
				if(selectedSaveGameIndex != -99) {
					selectedSaveGameIndex = -99;
				}
				else {
					showSave = false;
					showMenu = true;
				}
			}
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();

			GUILayout.FlexibleSpace();
			GUILayout.EndVertical();

		}
	}
}

