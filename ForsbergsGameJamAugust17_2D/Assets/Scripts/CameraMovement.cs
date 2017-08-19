using UnityEngine;
using System.Collections;

public class CameraMovement : MonoBehaviour 
{
    #region Fields/Properties

    public GameObject Player;
    public GameObject Level;
	
	#endregion
	#region Events
	
	void Awake()
	{
		
	}
	
	void Start() 
	{
        Bounds bounds = new Bounds();

        var renderers = Level.GetComponentsInChildren<SpriteRenderer>();

        foreach (var spriteRenderer in renderers)
        {
            bounds.Encapsulate(spriteRenderer.bounds);
        }

        Debug.LogFormat("Bounds: {0}", bounds.extents);
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
