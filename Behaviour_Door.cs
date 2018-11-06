using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Behaviour_Door : MonoBehaviour {

    private Animator animator;
    private new Collider2D collider ;
    [SerializeField] private int lockLevel;
    public bool IsBusy
    {
        get { return !animator.GetCurrentAnimatorStateInfo(0).IsName("Idle");}
    }
    // Use this for initialization
    void Start () {
        animator = GetComponent<Animator>();
        collider = GetComponent<Collider2D>();
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Interact()
    {
        animator.ResetTrigger("Operate");
        animator.SetTrigger("Operate");
    }

    public void FullClose()
    {
        if (!animator.GetBool("isClosed"))
            animator.SetBool("isClosed", true);
        else
            animator.SetBool("isClosed", false);
    }

    public int CheckLock()
    {
        return lockLevel;
    }

    public bool IsClosed()
    {
        return animator.GetBool("isClosed");
    }

    public void Open()
    {
        Interact();
    }

    public void Close()
    {
        Interact();
    }

    public void Lock()
    {
        lockLevel = 1;
    }

    public void Unlock()
    {
        if (Stats_PlayerGear.GetLockPickLevel() >= lockLevel)
            lockLevel = 0;
    }
}
