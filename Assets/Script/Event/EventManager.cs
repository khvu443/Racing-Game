using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.UIElements;
using static UIManager;

public class EventManager : MonoBehaviour
{
    public static EventManager Instance;

    [SerializeField] Text description;

    public event System.Action<Vehicle> IncreaseLap;
    public event System.Action<GameObject> FinishLap;

    public int currentSceneIndex;

    private void Awake()
    {
        if(Instance == null)
            Instance = this;
    }

    private void Start()
    {
        //Get current scene
        currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        if(description!=null)
            description.text = "";
    }

    //Handle event increase and finish
    public void IncreaseLapHandle(Vehicle obj)
    {
        if(IncreaseLap != null)
        {
            IncreaseLap(obj);
        }
    }
    public void FinishLapHandle(GameObject obj)
    {
        if (FinishLap != null)
        {
            FinishLap(obj);
        }
    }


    //When game is stop in editor or quit game -> data will be save
    private void OnApplicationQuit()
    {
        GameDataManager.Instance.SaveGame();
    }

    #region Button event

    //Close panel pause game by using event
    public void OnCloseClicked()
    {
        UIManager.Instance.DeActiveMenu(UIManager.Ui.PauseMenu);
    }

    //the data will reset to default when new game
    public void OnNewGameButtonClicked()
    {

        SceneManager.LoadScene(currentSceneIndex+1, LoadSceneMode.Single);

        SoundManager.Instance.PlayMusic(SoundManager.Instance.background, 0);
        // pause main menu bgm and play bgm game
        SoundManager.Instance.swapBGM(1, 0);
        //New data
        GameDataManager.Instance.NewGame();
    }

    //Reset game
    public void OnRetryButtonClicked()
    {
        SceneManager.LoadScene(currentSceneIndex);

        // when reset scene in pause
        // Time.timeScale stil = 0f -> everything is pause
        // sound is mute except bgm
        // -> unpause game
        UIManager.Instance.DeActiveMenu(Ui.PauseMenu);
    }

    //Go to main menu scene
    public void OnMainMenuButtonClicked()
    {
        SceneManager.LoadScene(currentSceneIndex - 1);
        SoundManager.Instance.PlayMusic(SoundManager.Instance.MainMenu, 1);
        // pause main menu bgm and play bgm game
        SoundManager.Instance.swapBGM(0, 1);
    }

    //Quit Game
    public void OnExitButtonClicked()
    {
        Application.Quit();
    }

    //Load data when clicked Load Game button
    public void OnLoadGameClicked()
    {

        SceneManager.LoadScene(currentSceneIndex+1, LoadSceneMode.Single);

        //Load data
        GameDataManager.Instance.LoadGame();
        SoundManager.Instance.PlayMusic(SoundManager.Instance.background, 0);
        // pause main menu bgm and play bgm game
        SoundManager.Instance.swapBGM(1, 0);
    }

    //Save data when clicked Save Game button
    public void OnSaveGameClicked()
    {
        GameDataManager.Instance.SaveGame();
    }
    #endregion

    //When hover button will description the button function
    #region Event Hover
    public void OnMouseHoverRetryEnter()
    {
        description.text = "Reset the game";
    }
    public void OnMouseHoverRetryExit()
    {
        description.text = "";
    }

    public  void OnMouseHoverMainMenuEnter()
    {
        description.text = "Return the main menu of game";
    }
    public void OnMouseHoverMainMenuExit()
    {
        description.text = "";
    }
    #endregion
}
