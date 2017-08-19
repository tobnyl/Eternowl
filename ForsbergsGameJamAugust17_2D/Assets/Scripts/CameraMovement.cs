using UnityEngine;
using System.Collections;

public class CameraMovement : MonoBehaviour 
{
    #region Fields/Properties

    public GameObject Player;
	
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
		if (Player.transform.position.y < transform.position.y)
        {
            var temp = transform.position;
            temp.y = Player.transform.position.y;
            transform.position = temp;
        }
	}
	
	#endregion
	#region Methods
	
	
	
	#endregion
}
