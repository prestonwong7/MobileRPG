using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine.SceneManagement;

public class SaveAndLoad : MonoBehaviour {
    
    public static PlayerStats thePlayerStats;
    public static CameraController theCameras;
 
    public static PlayerStartPoint thePSP;
    public static PlayerController thePC;

    public void Start()
    {
        thePlayerStats = FindObjectOfType<PlayerStats>();
        theCameras = FindObjectOfType<CameraController>();
        thePSP = FindObjectOfType<PlayerStartPoint>();
        thePC = FindObjectOfType<PlayerController>();
    }

    public void Save()
    {
        BinaryFormatter bf = new BinaryFormatter(); // Turn file into binary
        FileStream file = File.Create(Application.persistentDataPath + "/playerInfo.dat"); // Create file

        PlayerData data = new PlayerData();
        print(thePlayerStats.currentHp);
        data.currentHp = thePlayerStats.currentHp;
        data.currentAttack = thePlayerStats.currentAttack;
        data.currentDefense = thePlayerStats.currentDefense;
        data.currentScene = SceneManager.GetActiveScene().name;

        data.playerStartPoint = thePC.startPoint;
        data.pointName = thePSP.pointName;
        
        //data.theCamera = theCameras.theCamera;
        //data.thePlayer = thePlayer;

        bf.Serialize(file, data);
        file.Close();
        Debug.Log("Success save!");
    }

    public void OnSave()
    {
        //SaveLoad.objectIdentifierDict();
    }
    public void Load()
    {
        if (File.Exists(Application.persistentDataPath + "/playerInfo.dat"))
        {
            BinaryFormatter bf = new BinaryFormatter(); // Turn file into binary so people can't modify it
            FileStream file = File.Open(Application.persistentDataPath + "/playerInfo.dat", FileMode.Open); // Make the file
            PlayerData data = (PlayerData)bf.Deserialize(file); // Uses playerData class below
            file.Close();

            SceneManager.LoadScene(data.currentScene);
            thePlayerStats.currentAttack = data.currentAttack; // When loading, set currentAttack into the serializable attribute
            thePlayerStats.currentHp = data.currentHp;
            thePlayerStats.currentDefense = data.currentDefense;
            thePC.startPoint = data.playerStartPoint;
            thePSP.pointName = data.pointName;
            //theCameras.theCamera = data.theCamera;
            //thePlayer = data.thePlayer;
        }
    }
}


[Serializable]
class PlayerData
{
    public int currentHp;
    public int currentAttack;
    public int currentDefense;

    public string playerStartPoint;
    public string pointName;

    //[NonSerialized]
    public string currentScene;
    //public Camera theCamera;
    //public GameObject thePlayer;
}