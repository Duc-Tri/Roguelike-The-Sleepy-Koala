using OdinSerializer;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    public static SaveManager instance;
    [SerializeField] private int currentFloor = 0;
    [SerializeField] private string saveFileName = "saveThe.koala";
    [SerializeField] private SaveData save = new SaveData();

    public int CurrentFloor { get => currentFloor; set { currentFloor = value; } }
    public SaveData Save { get => save; set { save = value; } }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);
    }

    public bool HasSaveAvailable()
    {
        return File.Exists(Path.Combine(Application.persistentDataPath, saveFileName));
    }

    public void SaveGame()
    {
        save.SaveFloor = currentFloor;
        bool hasScene = save.Scenes.Find(x => x.FloorNumber == currentFloor) is not null;
        if (hasScene)
        {
            UpdateScene(SaveState());
        }
        else
        {
            AddScene(SaveState());
        }
        string path = Path.Combine(Application.persistentDataPath, saveFileName);
        byte[] saveJson = SerializationUtility.SerializeValue(save, DataFormat.JSON); // serialize the state to JSON
        File.WriteAllBytes(path, saveJson);
    }

    private void AddScene(object v)
    {
        throw new NotImplementedException();
    }

    private void UpdateScene(object v)
    {
        throw new NotImplementedException();
    }

    private object SaveState()
    {
        throw new NotImplementedException();
    }
}

[System.Serializable]
public class SaveData
{
    [SerializeField] private int savedFloor;
    [SerializeField] private List<SceneState> scenes;

    public int SaveFloor { get; internal set; }
    public List<SceneState> Scenes { get => scenes; set { scenes = value; } }

}

public class SceneState
{
    public int FloorNumber { get; internal set; }
}