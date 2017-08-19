using UnityEngine;
using System.Collections;

public class TimeScale : MonoBehaviour {

    [Range(0, 1)]
    public float timeScale = 1.0f;

	// Use this for initialization
	void Start ()
    {
        Time.timeScale = timeScale;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
