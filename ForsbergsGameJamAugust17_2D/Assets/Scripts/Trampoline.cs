using UnityEngine;
using System.Collections;

public class Trampoline : MonoBehaviour 
{
    #region Fields/Properties

    private Animator _animator;
    private int _isJumpingHash;
	
	#endregion
	#region Events
	
	void Awake()
	{
        _animator = GetComponent<Animator>();

        _isJumpingHash = Animator.StringToHash("IsJumping");
	}
	
	void Start() 
	{
		
	}

	void Update() 
	{
		
	}

    #endregion
    #region Methods

    public void PlayAnimation()
    {
        _animator.SetBool(_isJumpingHash, true);

        StartCoroutine(StopAnimation());
    }

    #endregion

    #region Coroutine

    IEnumerator StopAnimation()
    {
        yield return new WaitForSeconds(1);

        _animator.SetBool(_isJumpingHash, false);
    }

    #endregion
}
