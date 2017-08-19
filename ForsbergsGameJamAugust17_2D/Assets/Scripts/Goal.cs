using UnityEngine;
using System.Collections;

public class Goal : MonoBehaviour 
{
    #region Fields/Properties

    public Sprite OpenDoor;

    private SpriteRenderer _spriteRenderer;

    public bool IsUnlocked { get; set; }

    #endregion
    #region Events

    void Awake()
	{
        _spriteRenderer = GetComponent<SpriteRenderer>();
	}
	
	void Start() 
	{
		
	}

	void Update() 
	{
		
	}
	
	#endregion
	#region Methods
	
	public void ChangeToOpenDoorSprite()
    {
        _spriteRenderer.sprite = OpenDoor;
    }
	
	#endregion
}
