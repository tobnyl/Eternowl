using UnityEngine;
using System.Collections;

public class CameraMovement : MonoBehaviour 
{
    #region Fields/Properties

    public Player Player;
    public GameObject Level;

    private Bounds _bounds;
    private Camera _camera;
    private float _height;
    private float _cameraBottom;
    private bool _isAtBottom;
    private Vector3 _cameraStartPosition;

	#endregion
	#region Events
	
	void Awake()
	{
        _camera = GetComponent<Camera>();
	    _height = 2f * _camera.orthographicSize;
        _cameraStartPosition = transform.position;
    }
	
	void Start() 
	{
        _bounds = new Bounds();

        var renderers = Level.GetComponentsInChildren<SpriteRenderer>();

        foreach (var spriteRenderer in renderers)
        {
            _bounds.Encapsulate(spriteRenderer.bounds);
        }

        Debug.LogFormat("Bounds: {0}", _bounds.extents);
    }

	void Update() 
	{
        if (!Player.IsDead)
        {
            _cameraBottom = transform.position.y - (_height / 2f);
            _isAtBottom = _cameraBottom <= _bounds.min.y;

            if (!_isAtBottom && Player.transform.position.y < transform.position.y)
            {
                var temp = transform.position;
                temp.y = Player.transform.position.y;
                transform.position = temp;
            }
            else if (_isAtBottom)
            {
                var temp = transform.position;
                temp.y = _bounds.min.y + _height / 2f;
                transform.position = temp;
            }
        }       
    }
	
	#endregion
	#region Methods
	
	public void ResetPosition()
    {
        transform.position = _cameraStartPosition;
    }
	
	#endregion
}
