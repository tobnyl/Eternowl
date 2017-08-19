using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class FinishedUI : MonoBehaviour
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

    }

    void Update()
    {
        if (Input.GetButtonDown("Next"))
        {
            #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
            #endif

            Application.Quit();
        }
    }

    #endregion
    #region Methods



    #endregion
}
