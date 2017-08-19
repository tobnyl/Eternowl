using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public string[] Levels;    

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
        SceneManager.LoadScene(Levels[CurrentLevelIndex]);

    }
}
