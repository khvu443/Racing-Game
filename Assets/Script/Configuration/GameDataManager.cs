using Assets.Script.Configuration;
using Assets.Script.Data;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class GameDataManager : MonoBehaviour
{
    [Header("File Store Config")]
    [SerializeField] string FileName;

    public static GameDataManager Instance;

    private GameData Data;
    private List<IGameDataManager> managers;
    private FileDataHandle fileDataHandle;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    private void Start()
    {
        this.fileDataHandle = new FileDataHandle(@"C:\", FileName); ;
        //
        this.managers = FindAllDataManager();
    }

    public void NewGame()
    {
        this.Data = new GameData();
        SaveGame();
    }


    public void LoadGame()
    {

        //load any saved data from a file using data handler
        this.Data = fileDataHandle.Load();

        //if no data can be loaded, initalize to a new game
        if (this.Data == null)
        {
            Debug.Log("No data");
            NewGame();
        }

        // push the loaded data to all other scripts that need it
        foreach (IGameDataManager gameDataManager in managers)
        {
            gameDataManager.LoadData(Data);
        }
    }

    public void SaveGame()
    {
        // pass the data to other scripts so they can update it
        foreach (IGameDataManager gameDataManager in managers)
        {
            gameDataManager.SaveData(ref Data);
        }

        // save that data to a file using the data handler
        fileDataHandle.Save(Data);
    }

    private List<IGameDataManager> FindAllDataManager()
    {
        // find every object have type IGameDataManager and return to a list
        IEnumerable<IGameDataManager> list = FindObjectsOfType<MonoBehaviour>().OfType<IGameDataManager>();
        return new List<IGameDataManager>(list);
    }

}
