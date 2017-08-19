﻿using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class UiManager : MonoBehaviour 
{
    #region Fields/Properties

    public string SceneNameToLoad;

    private static UiManager _instance;

    public static int Keys { get; set; }

    public static UiManager Instance
    {
        get { return _instance; }
    }

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
