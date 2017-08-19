using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    #region Fields/Properties

    public float Force;

    private Rigidbody _rigidBody;

    #endregion
    #region Events

    void Awake()
    {
        _rigidBody = GetComponent<Rigidbody>();
    }

    void Start()
    {

    }

    void Update()
    {

    }

    void FixedUpdate()
    {        
        _rigidBody.AddForce(new Vector3(Input.GetAxis("Horizontal"), 0, 0) * Force );
    }

    #endregion
    #region Methods



    #endregion
}
