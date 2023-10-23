using Assets.Script.Configuration;
using Assets.Script.Data;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class Car : Vehicle
{
       
    protected override void Start()
    {

        GameDataManager.Instance.LoadGame();

        //set speedometer object
        Speedometer_Txt = GameObject.Find("Speedometer_txt").GetComponent<TextMeshProUGUI>();
        base.Start();
        // set type
        typeControll = TypeControll.Keyboard;

    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        if (GameManager.isFinish)
        {
            GameDataManager.Instance.SaveGame();
        }
    }

}
