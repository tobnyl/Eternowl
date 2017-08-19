using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

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

    }

    void Update()
    {
        if (Input.GetButtonDown("Next"))
        {
            Debug.Log("Continue!!");
            SceneManager.LoadScene(SceneNameToLoad);
        }
    }

    #endregion
    #region Methods



    #endregion
}
