using Assets.Script.Configuration;
using Assets.Script.Data;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class GameManager : MonoBehaviour, IGameDataManager
{

    public static GameManager Instance;

    [SerializeField] TMP_Text lapTime;
    [SerializeField] TMP_Text CountDown;
    [SerializeField] Text BestTime;

    private float timer = 0f;
    private float bestTimeLap = 0f;
    private float count = 3f;

    // Flag
    public static bool isStart = false;
    public static bool isFinish = false;
    public static bool saving = false;
    public static bool isPause = false;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        saving = false;
        isStart = false;
        isFinish = false;
        isPause = false;

        // set best time to text when playing
        BestTime.text = Mathf.FloorToInt(bestTimeLap / 60).ToString("00") + ":" + Mathf.FloorToInt(bestTimeLap % 60).ToString("00");
        StartCoroutine(StartRace());
    }

    // Update is called once per frame
    void Update()
    {
        // when game start (count down to 0) -> start to count time lap
        if (isStart)
        {
            TimerCount();
        }
    }

    private void FixedUpdate()
    {
        //Count down to start
        StartRace();
    }

    void TimerCount()
    {
        // if not finish
        if (!isFinish)
        {
            timer += Time.deltaTime;
            //convert to minutes
            float minutes = Mathf.FloorToInt(timer / 60);
            // set to text 
            // ToString("00") -> show digit will have format like "02:02"
            lapTime.text = "Time: " + minutes.ToString("00") + ":" + Mathf.FloorToInt(timer % 60).ToString("00") +
                "\nBest Time: " + Mathf.FloorToInt(bestTimeLap / 60).ToString("00") + ":" + Mathf.FloorToInt(bestTimeLap % 60).ToString("00");
        }
        else
        {
            //after finish lap -> set timer (second) to best time variable
            bestTimeLap =  timer;
            // show in finish panel, how long to finish 3 laps
            BestTime.text = Mathf.FloorToInt(timer / 60).ToString("00") + ":" + Mathf.FloorToInt(timer % 60).ToString("00");
            // flag to saving one time
            if (!saving)
            {
                GameDataManager.Instance.SaveGame();
                saving = true;
            }
        }
    }
    
    //Count down to start
    IEnumerator StartRace()
    {
        yield return new WaitForSeconds(0.2f);
        SoundManager.Instance.PlaySFX(SoundManager.Instance.CoundownSFX);
        while (count > 0)
        {
            CountDown.text = count.ToString("0");
            yield return new WaitForSeconds(0.7f);
            count--;
        }
        yield return new WaitForSeconds(0.6f);
        CountDown.text = "Go";
        yield return new WaitForSeconds(1.0f);
        CountDown.gameObject.SetActive(false);
        isStart = true;
        yield return null;
    }

    public void LoadData(GameData data)
    {
        this.bestTimeLap = data.TimeFinish;
    }

    public void SaveData(ref GameData data)
    {
        //if the time finish is 0 -> mean it is new because no one can finish 3 laps in 0 second
        // set the first time finish
        if (data.TimeFinish == 0)
        {
            data.TimeFinish = bestTimeLap; 

        }
        // or else check min 
        else
        {
            data.TimeFinish = Mathf.Min(bestTimeLap, data.TimeFinish);
        }

    }
}
