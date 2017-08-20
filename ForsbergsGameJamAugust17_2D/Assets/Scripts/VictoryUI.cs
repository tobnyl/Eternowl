using UnityEngine;
using Supersonic;
using System.Collections;
using UnityEngine.SceneManagement;

public class VictoryUI : MonoBehaviour
{
    #region Fields/Properties

    //public string SceneNameToLoad;

    #endregion
    #region Events

    void Awake()
    {

    }

    void Start()
    {
        AudioPlayer.Instance.PlayTrack(GameManager.Instance.LevelComplete);
    }

    void Update()
    {
        if (Input.anyKeyDown)
        {
            Debug.Log("Continue!!");
            GameManager.Instance.LoadNextScene();
        }
    }

    #endregion
    #region Methods



    #endregion
}
