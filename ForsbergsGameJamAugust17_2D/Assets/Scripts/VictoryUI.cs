using UnityEngine;
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

    }

    void Update()
    {
        if (Input.GetButtonDown("Next"))
        {
            Debug.Log("Continue!!");
            GameManager.Instance.LoadNextScene();
        }
    }

    #endregion
    #region Methods



    #endregion
}
