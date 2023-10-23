using Assets.Script.Configuration;
using Assets.Script.Data;
using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Lap : MonoBehaviour
{
    [SerializeField] TMP_Text Lap_Txt;


    private void Start()
    {
        EventManager.Instance.IncreaseLap += increaseLap;
        EventManager.Instance.FinishLap += finishLap;
    }

    void OnTriggerEnter(Collider other)
    {
        switch (other.tag)
        {
            case "Player":
                EventManager.Instance.IncreaseLapHandle(other.transform.GetComponentInParent<Car>());
                break;

            case "AiDriver":
                EventManager.Instance.IncreaseLapHandle(other.transform.GetComponentInParent<AI_Controller_Car>());
                break;
        }
        EventManager.Instance.FinishLapHandle(other.transform.parent.gameObject);
    }

    void increaseLap(Vehicle obj)
    {

        if (obj.lap != 3)
        {
            obj.lap++;

            if (Lap_Txt != null)
                Lap_Txt.text = "Lap: " + obj.lap + "/3";
        }
    }

    void finishLap(GameObject obj)
    {
        if (obj.GetComponent<Vehicle>().lap == 3 && obj.CompareTag("Player"))
        {
            SoundManager.Instance.PlaySFX(SoundManager.Instance.FinishLapSFX);
            // find the camera virtual
            GameObject camera = GameObject.FindGameObjectWithTag("Virtual Camera");
            // change view mode
            camera.GetComponent<CinemachineVirtualCamera>().GetCinemachineComponent<CinemachineTransposer>().m_BindingMode = CinemachineTransposer.BindingMode.WorldSpace;
            // set game is finish -> stop some function like how long to finish the time, etc
            GameManager.isFinish = true;
            //show finish menu
            UIManager.Instance.ActiveMenu(UIManager.Ui.FinishMenu);
        }
    }
}

