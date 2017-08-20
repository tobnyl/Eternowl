using Supersonic;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public string[] Levels;

    [Header("Music")]
    public Track LevelBgm;

    [Header("Debug")]
    public int UseThisLevelIndexOnly;

    public int CurrentLevelIndex { get; set; }
    public bool FinishedGame { get { return CurrentLevelIndex >= Levels.Length;  } }

    private static GameManager _instance;
    public static GameManager Instance
    {
        get { return _instance; }
    }
    void Start()
    {
        if (_instance == null)
        {
            _instance = this;
        }

        AudioPlayer.Instance.PlayTrack(LevelBgm);

        //Keys = GameObject.FindGameObjectsWithTag("Key").Length;

        DontDestroyOnLoad(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.LogFormat("Keys: {0}", Keys);
    }

    public void LoadNextScene()
    {
        if (CurrentLevelIndex < Levels.Length)
        {
            SceneManager.LoadScene(Levels[CurrentLevelIndex]);
        }
    }

    public void LoadCurrentScene()
    {
        if (UseThisLevelIndexOnly == 0)
        { 
            SceneManager.LoadScene(Levels[CurrentLevelIndex]);
        }
        else
        {
            SceneManager.LoadScene(Levels[UseThisLevelIndexOnly]);
        }

    }
}
