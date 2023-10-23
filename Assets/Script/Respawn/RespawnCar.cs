using Assets.Script.Configuration;
using Assets.Script.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnCar : MonoBehaviour, IGameDataManager
{
    public static RespawnCar Instance;

    public RespawnPoint respawns;
    private List<Transform> point = new List<Transform>();
    [SerializeField] GameObject[] carPrefab;
    public static int length;
    GameObject car;

    private void Awake()
    {

    }

    private void Start()
    {
        //Set List Start Position
        length = carPrefab.Length;
        respawns = GameObject.FindGameObjectWithTag("Respawn").GetComponent<RespawnPoint>();
        point = respawns.respanwPoint;
        Debug.Log(carPrefab.Length);
        Debug.Log(respawns.respanwPoint.Count);

        //Set each car for different position
        for (int i = 0; i < length; i++)
        {
            car = Instantiate(carPrefab[i], point[i].position, Quaternion.Euler(0, -90, 0));

            //Set camera minimap following to Player
            if (car.CompareTag("Player"))
            {
                FollowingCamera.target = car.transform.GetChild(1);
                FollowingCamera.look = car.transform.GetChild(1);
            }
        }
    }
    public void LoadData(GameData data)
    {
        GameObject.FindGameObjectWithTag("Player").transform.GetChild(1).GetComponent<Car>().maxRpm = data.CarSetting.maxRpm;
        GameObject.FindGameObjectWithTag("Player").transform.GetChild(1).GetComponent<Car>().breakingForce = data.CarSetting.breakingForce;

        GameObject.FindGameObjectWithTag("Player").transform.GetChild(1).GetComponent<Car>().gears = data.CarSetting.gears;
        GameObject.FindGameObjectWithTag("Player").transform.GetChild(1).GetComponent<Car>().gears[0] = data.CarSetting.gears[0];
        GameObject.FindGameObjectWithTag("Player").transform.GetChild(1).GetComponent<Car>().gears[1] = data.CarSetting.gears[1];
        GameObject.FindGameObjectWithTag("Player").transform.GetChild(1).GetComponent<Car>().gears[2] = data.CarSetting.gears[2];
        GameObject.FindGameObjectWithTag("Player").transform.GetChild(1).GetComponent<Car>().gears[3] = data.CarSetting.gears[3];
        GameObject.FindGameObjectWithTag("Player").transform.GetChild(1).GetComponent<Car>().gears[4] = data.CarSetting.gears[4];

        GameObject.FindGameObjectWithTag("Player").transform.GetChild(1).GetComponent<Car>().gearNum = data.CarSetting.gearNum;
    }

    public void SaveData(ref GameData data)
    {

        data.CarSetting.maxRpm = GameObject.FindGameObjectWithTag("Player").transform.GetChild(1).GetComponent<Car>().maxRpm;
        data.CarSetting.breakingForce = GameObject.FindGameObjectWithTag("Player").transform.GetChild(1).GetComponent<Car>().breakingForce;

        data.CarSetting.gears = GameObject.FindGameObjectWithTag("Player").transform.GetChild(1).GetComponent<Car>().gears;
        data.CarSetting.gears[0] = GameObject.FindGameObjectWithTag("Player").transform.GetChild(1).GetComponent<Car>().gears[0];
        data.CarSetting.gears[1] = GameObject.FindGameObjectWithTag("Player").transform.GetChild(1).GetComponent<Car>().gears[1];
        data.CarSetting.gears[2] = GameObject.FindGameObjectWithTag("Player").transform.GetChild(1).GetComponent<Car>().gears[2];
        data.CarSetting.gears[3] = GameObject.FindGameObjectWithTag("Player").transform.GetChild(1).GetComponent<Car>().gears[3];
        data.CarSetting.gears[4] = GameObject.FindGameObjectWithTag("Player").transform.GetChild(1).GetComponent<Car>().gears[4];

        data.CarSetting.gearNum = GameObject.FindGameObjectWithTag("Player").transform.GetChild(1).GetComponent<Car>().gearNum;


    }
}
