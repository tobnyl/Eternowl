using Supersonic;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour {

    #region Fields/Properties

    [Header("Force")]
    public float Force;
    public float TrampolineForce;

    [Header("Triggers")]
    public Transform LeftEdge;
    public Transform RightEdge;
    public float Offset;

    [Header("Death")]
    public float DeathSequenceTime;
    public float GoalSequenceTime;
    public float FinishedSequenceTime;

    public Goal GoalDoor;
    public CameraMovement Camera;
    public GameObject SpawnPosition;

    private Rigidbody _rigidBody;
    private bool _hasKey;
    private SpriteRenderer _spriteRenderer;

    public bool IsDead { get; set; }

    #endregion
    #region Events

    void Awake()
    {
        _rigidBody = GetComponent<Rigidbody>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
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
            GoalDoor.ChangeToOpenDoorSprite();
            Destroy(other.gameObject);
        }
        else if (other.gameObject.tag == "Goal" && _hasKey)
        {
            Debug.Log("Goal!");
            GoalSequence();
        }
        else if (other.gameObject.tag == "Spike")
        {
            AudioPlayer.Instance.PlaySoundEffect2D(GameManager.Instance.Spike);

            DeathSequence();
        }
        else if (other.gameObject.tag == "Trampoline")
        {
            var trampoline = other.gameObject.GetComponentInParent<Trampoline>();
            trampoline.PlayAnimation();

            Debug.Log("Trampoline");
            _rigidBody.AddForce(Vector3.up * TrampolineForce, ForceMode.Impulse);
        }
    }

    #endregion
    #region Methods

    private void DeathSequence()
    {
        IsDead = true;
        //_spriteRenderer.enabled = false;
        _rigidBody.isKinematic = true;

        AudioPlayer.Instance.PlayTrack(GameManager.Instance.OwlDeath);

        StartCoroutine(NewSceneCoroutine(DeathSequenceTime, "GameOver"));
    }

    private void GoalSequence()
    {
        _rigidBody.isKinematic = true;

        GameManager.Instance.CurrentLevelIndex++;

        if (!GameManager.Instance.FinishedGame)
        {
            StartCoroutine(NewSceneCoroutine(GoalSequenceTime, "Victory"));

        }
        else
        {
            StartCoroutine(NewSceneCoroutine(FinishedSequenceTime, "FinishedGame"));

        }
    }

    #endregion

    #region Coroutines

    //private IEnumerator GoalCoroutine()
    //{
    //    yield return new WaitForSeconds(GoalSequenceTime);

    //    SceneManager.LoadScene("Victory");
    //}

    private IEnumerator NewSceneCoroutine(float time, string sceneToLoad)
    {
        yield return new WaitForSeconds(time);

        SceneManager.LoadScene(sceneToLoad);
    }

    #endregion
}
