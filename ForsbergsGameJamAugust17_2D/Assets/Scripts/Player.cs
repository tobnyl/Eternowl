using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    #region Fields/Properties

    [Header("Force")]
    public float Force;

    [Header("Triggers")]
    public Transform LeftEdge;
    public Transform RightEdge;
    public float Offset;
    
    public CameraMovement Camera;
    public GameObject SpawnPosition;

    private Rigidbody _rigidBody;
    private bool _hasKey;

    public bool IsDead { get; set; }

    #endregion
    #region Events

    void Awake()
    {
        _rigidBody = GetComponent<Rigidbody>();
    }

    void Start()
    {
        transform.position = SpawnPosition.transform.position;
    }

    void Update()
    {

    }

    void FixedUpdate()
    {        
        _rigidBody.AddForce(new Vector3(Input.GetAxis("Horizontal"), 0, 0) * Force );
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "RightEdge")
        {
            var boxCollider = other.gameObject.GetComponent<BoxCollider>();

            transform.position = new Vector3(LeftEdge.position.x + boxCollider.size.x + Offset, transform.position.y, LeftEdge.position.z);
        }
        else if (other.gameObject.tag == "LeftEdge")
        {
            var boxCollider = other.gameObject.GetComponent<BoxCollider>();

            transform.position = new Vector3(RightEdge.position.x - boxCollider.size.x - Offset, transform.position.y, RightEdge.position.z);
        }
        else if (other.gameObject.tag == "PortalBottom")
        {
            transform.position = SpawnPosition.transform.position;
            Camera.ResetPosition();
        }
        else if (other.gameObject.tag == "Key")
        {            
            _hasKey = true;
            Destroy(other.gameObject);
        }
        else if (other.gameObject.tag == "Goal" && _hasKey)
        {
            Debug.Log("Goal!");
        }
        else if (other.gameObject.tag == "Spike")
        {
            IsDead = true;
            Destroy(gameObject);
        }
    }

    #endregion
    #region Methods



    #endregion
}
