using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using Supersonic;

public class GameOverUI : MonoBehaviour
{
    #region Fields/Properties

    public string SceneNameToLoad;

    #endregion
    #region Events

    void Awake()
    {

    }

    void Start()
    {
        AudioPlayer.Instance.PlayTrack(GameManager.Instance.LevelFailed);
    }

    void Update()
    {
        if (Input.anyKeyDown)
        {
            Debug.Log("Continue!!");
            GameManager.Instance.LoadCurrentScene();

        }
    }

    #endregion
    #region Methods



    #endregion
}
