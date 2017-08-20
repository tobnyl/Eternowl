using UnityEngine;
using Supersonic;
using System.Collections;
using UnityEngine.SceneManagement;

public class TitleUI : MonoBehaviour
{
    #region Fields/Properties

    //public string SceneNameToLoad;

    #endregion
    #region Events

    void Awake()
    {
        Screen.fullScreen = true;
    }

    void Start()
    {
        AudioPlayer.Instance.PlayTrack(GameManager.Instance.Title);
    }

    void Update()
    {
        if (Input.anyKeyDown)
        {            
            GameManager.Instance.LoadNextScene();
        }
    }

    #endregion
    #region Methods



    #endregion
}
