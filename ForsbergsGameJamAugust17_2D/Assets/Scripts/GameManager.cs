using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;

    public static int Keys { get; set; }

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

        Keys = GameObject.FindGameObjectsWithTag("Key").Length;

        
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.LogFormat("Keys: {0}", Keys);
    }
}
