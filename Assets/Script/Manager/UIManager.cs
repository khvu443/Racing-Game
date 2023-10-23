using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [SerializeField] GameObject pnlPause;
    [SerializeField] GameObject pnlFinish;

    public enum Ui
    {
        FinishMenu,
        PauseMenu
    }

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    // active menu
    public void ActiveMenu(Ui name)
    {
        switch (name)
        {
            case Ui.FinishMenu:
                pnlFinish.SetActive(true);
                break;

            case Ui.PauseMenu:
                pnlPause.SetActive(true);
                // Time scale= 0 => almost every thing except audio
                Time.timeScale = 0f;
                //set pause flag
                GameManager.isPause = true;
                //making audio is pause -> car audio except bgm
                AudioListener.pause = true;
                //set main menu bgm to audio index 1
                SoundManager.Instance.PlayMusic(SoundManager.Instance.MainMenu, 1);
                // pause game bgm and play main menu
                SoundManager.Instance.swapBGM(0, 1);
                break;
        }
    }

    // deactive menu
    public void DeActiveMenu(Ui name)
    {
        switch (name)
        {
            case Ui.FinishMenu:
                pnlFinish.SetActive(false);
                break;

            case Ui.PauseMenu:
                pnlPause.SetActive(false);
                // Time scale= 1 => unpause
                Time.timeScale = 1f;
                // make audio unpause
                AudioListener.pause = false;
                //set pause flag
                GameManager.isPause = false;
                //set game bgm to audio index 0
                SoundManager.Instance.PlayMusic(SoundManager.Instance.background, 0);
                // pause main menu bgm and play game bgm
                SoundManager.Instance.swapBGM(1, 0);
                break;
        }

    }

}
