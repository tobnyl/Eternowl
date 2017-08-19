using UnityEngine;
using System.Collections;

public class CameraMovement : MonoBehaviour 
{
    #region Fields/Properties

    public GameObject Player;
    public GameObject Level;

    private Bounds _bounds;
    private Camera _camera;
    private float _height;
    private float _cameraBottom;
    private bool _isAtBottom;

	#endregion
	#region Events
	
	void Awake()
	{
        _camera = GetComponent<Camera>();
	    _height = 2f * _camera.orthographicSize;
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

        //Debug.LogFormat("Camera: {0} | Bottom: {1}", transform.position.y - (_height / 2f), _bounds.min.y);
    }
	
	#endregion
	#region Methods
	
	
	
	#endregion
}
