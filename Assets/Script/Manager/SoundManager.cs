using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static Unity.VisualScripting.Member;

public class SoundManager : MonoBehaviour
{

    [Header("Audio Source")]
    // list audio bgm source
    // [0] -> game bgm
    // [1] -> main menu bgm
    [SerializeField] AudioSource[] source;
    [SerializeField] AudioSource sfx;

    [Header("Audio Clip")]
    [SerializeField] public AudioClip background;
    [SerializeField] public AudioClip MainMenu;
    [SerializeField] public AudioClip FinishLapSFX;
    [SerializeField] public AudioClip CoundownSFX;
    [SerializeField] public AudioClip HitSFX;

    float delay = 0.8f;

    public static SoundManager Instance;

    void Awake()
    {

        if (Instance != null && Instance != this)
            Destroy(this.gameObject);
        else
        {
            Instance = this;
            DontDestroyOnLoad(Instance);
        }


    }

    private void Start()
    {
        //Set ingore listener true -> when set AudioListener pause so
        //only music background can play except car audio and sfx
        source[0].ignoreListenerPause = true;
        source[1].ignoreListenerPause = true;

        // Check if the scene active is main menu or track so that
        // it can have appropriate music
        if (SceneManager.GetSceneByBuildIndex(EventManager.Instance.currentSceneIndex).name == "MainMenu")
        {
            PlayMusic(MainMenu, 1);
            PlayMusic(background, 0);
            swapBGM(0,1);

        }
    }


    public void PlaySFX(AudioClip clip)
    {
        sfx.PlayOneShot(clip);
    }

    public void PlayMusic(AudioClip clip, int i)
    {
        source[i].clip = clip;
        source[i].volume = 0.15f;
        source[i].Play();
    }

    // When pause game, BGM of pause game is different
    // and When playing game, BGM of game is different
    // -> it pause the bgm when playing and playing the bgm of pause game
    public void swapBGM(int pause, int play)
    {
        if(SceneManager.GetActiveScene().buildIndex == EventManager.Instance.currentSceneIndex)
        {
            source[pause].Pause();
            source[play].UnPause();
        }

    }

    //Each car have hit sfx
    public void HitSfx(GameObject obj)
    {
        if (obj != null)
        {
            AudioSource src = obj.AddComponent<AudioSource>();
            src.volume = 1f;
            src.spatialBlend = 1;
            src.PlayOneShot(HitSFX);
            Destroy(src, delay);
        }
    }


}
