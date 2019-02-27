using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Behaviour_Door : MonoBehaviour {

    private Animator animator;
    private new Collider2D collider;
    public bool Unlocked { get; private set; }
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
	void Update () {}

    //Set off door animation
    public void Interact()
    {
        if (Unlocked)
        {
            animator.ResetTrigger("Operate");
            animator.SetTrigger("Operate");
        }
        else
            Debug.Log("Door is locked");
    }

    public void FullClose()
    {
        if (!animator.GetBool("isClosed"))
            animator.SetBool("isClosed", true);
        else
            animator.SetBool("isClosed", false);
    }

    public bool CheckLock()
    {
        return Unlocked;
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
        if (Stats_PlayerGear.GetLockPickLevel() > 0)
        {
            UI_Lock.currentLock = GetComponent<Behaviour_Lock>();
            UI_Lock.Instance.gameObject.SetActive(true);
            UI_Lock.currentLock.untouched = false;
        }
        else
            Debug.Log("Cant lock no lockpick");
    }

    public void Unlock()
    {
        if (Stats_PlayerGear.GetLockPickLevel() >= 0)
        {
            UI_Lock.currentLock = GetComponent<Behaviour_Lock>();
            UI_Lock.Instance.gameObject.SetActive(true);
            UI_Lock.currentLock.untouched = false;
        }
        else
            Debug.Log("Cant open no lockpick");
    }

    public void DisableLock()
    {
        Unlocked = true;
    }

    public void EnableLock()
    {
        Unlocked = false;
    }
}
