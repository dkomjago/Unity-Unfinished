﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controls_Character : MonoBehaviour {

    #region Movement
    [SerializeField] private float maxSpeed = 10f; // Max movement speed (while Sprinting)
    [Range(0, 1)] [SerializeField] private float walkSpeed = .36f;     // Amount of maxSpeed applied to crouching movement. 1 = 100%
    [Range(0, 1)] [SerializeField] private float sneakSpeed = .36f; // Amount of maxSpeed applied to sneak movement(change in Editor)
    public bool FacingRight
    {
        get { return facingRight; }
    }
    private bool facingRight = true;  // For determining which way the player is currently facing.
    public static bool movementDisabled = false;
    private bool isSprinting = false;

    #region Stairs
    private bool onStairs = false; // Is character on stairs
    public bool OnStairs
    {
        get { return onStairs; }
        set { onStairs = value; }
    }
    private bool wantOnStairsUp = false; // Cues to use the stairs
    public bool WantOnStairsUp
    {
        get { return wantOnStairsUp; }
    }
    private bool wantOnStairsDown = false;
    public bool WantOnStairsDown
    {
        get { return wantOnStairsDown; }
    }
    private float stairsAngle; //angle of stairs in degrees , if get -> radians
    public float StairsAngle
    {
        get { return Mathf.Deg2Rad * stairsAngle; }
        set
        {
            if (value != 0)
                stairsAngle = value;
        }
    }
    #endregion
    #endregion

    [SerializeField] private bool isArmed; //unused
    [SerializeField] private float stepLoudness; // future use for behaviour_sound
    [SerializeField] private int tension; // decides stress growth
    [SerializeField] private float stress; // used for shaking hands etc.
    public float Stress{get {return stress;} }

    private Animator animator; // Reference to the player's animator component.
    private new Rigidbody2D rigidbody2D;  // Rigidbody reference
    private new SpriteRenderer renderer; // Renderer reference
    private GameObject arms; // reference to character arms object

    private static bool isPaused; // used to stop movement and behaviour

    private void Awake()
    {
        Global_GameManager.OnPause += delegate { isPaused= true;};
        Global_GameManager.UnPause += delegate { isPaused = false;};
        renderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        rigidbody2D = GetComponent<Rigidbody2D>();
        arms = transform.Find("Arms").gameObject;
}

    private void Start()
    {
        tension = 1;
    }


    private void FixedUpdate()
    {
        if (!isPaused)
        {
            if (stress > 0)
                stress -= 0.2f;
        }
    }

    //Main move function - decides speed and direction
    public void Move(float move, bool sneak)
    {
        if (!isPaused && !movementDisabled)
        {
            animator.SetBool("isSneaking", sneak);
            move = (isSprinting ? move : sneak ? move * sneakSpeed : move * walkSpeed);
            animator.SetFloat("Speed", Mathf.Abs(move)); // The Speed animator parameter is set to the absolute value of the horizontal input.
            if (move != 0)
            {
                if (onStairs)
                {
                    rigidbody2D.gravityScale = 0;
                    rigidbody2D.velocity = new Vector2(move * maxSpeed * Mathf.Cos(StairsAngle), move * maxSpeed * Mathf.Sin(StairsAngle));
                }
                else
                {
                    rigidbody2D.gravityScale = 10;
                    rigidbody2D.velocity = new Vector2(move * maxSpeed, 0);
                }
            }
            else
                rigidbody2D.velocity = Vector2.zero;
            Behavior_Sound.MakeSound(stepLoudness, transform.position);
            //Flip sprite according to movement
            if (move > 0 && !facingRight)
            {
                Flip();
            }
            else if (move < 0 && facingRight)
            {
                Flip();
            }
        }
    }

    //Flip character sprite on x axis
    private void Flip()
    {
        facingRight = !facingRight;
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
        foreach (Light2D.LightSprite light in GetComponentsInChildren<Light2D.LightSprite>(true))
        {
            theScale = light.transform.localScale;
            theScale.x *= -1;
            light.transform.localScale = theScale;
        }
    }

    // Instant heart pounding and shaking hands
    public void AddCriticalStress() 
    {
        stress += 150;
    }

    public void AddStress()
    {
        stress += 0.2f*tension;
    }

    public void Tense()
    {
        tension = 2;
    }

    public void ToggleSprint()
    {
        if (!isPaused && !movementDisabled)
        {
            if (isSprinting)
            {
                arms.SetActive(true);
                isSprinting = false;
                animator.SetBool("isSprinting", false);
            }
            else
            {
                arms.SetActive(false);
                isSprinting = true;
                animator.SetBool("isSprinting", true);
            }
        }
    }

    //Limits stair accessibility
    public void TryStairs(float direction) 
    {
        if (direction>0)
        {
            wantOnStairsUp = true;
            wantOnStairsDown = false;
        }
        else if (direction<0)
        {
            wantOnStairsDown = true;
            wantOnStairsUp = false;
        }
        else
        {
            wantOnStairsDown = false;
            wantOnStairsUp = false;
        }
    }

    //Make list of nearby actions available to the character
    public List<Info_Action> Action()
    {
        Physics2D.queriesHitTriggers = true;
        Collider2D[] hits = Physics2D.OverlapAreaAll(transform.position + renderer.bounds.size,
            transform.position - renderer.bounds.size - new Vector3(-transform.localScale.x * 2, 0, 0));
        var actionList = new List<Info_Action>();
        foreach (Collider2D hit in hits)
        {
            MonoBehaviour script = hit.GetComponent<Behaviour_Door>();
            if (script != null)
            {
                Behaviour_Door newScript = (Behaviour_Door)script;
                if (!newScript.IsBusy)
                {
                    if (newScript.IsClosed())
                    {
                        actionList.Add(new Info_Action("Open", script));
                        if (!newScript.CheckLock())
                            actionList.Add(new Info_Action("Unlock", script));
                        else
                            actionList.Add(new Info_Action("Lock", script));
                    }
                    else
                        actionList.Add(new Info_Action("Close", script));
                }
                    continue;
            }
            script = hit.GetComponent<Behaviour_Switch>();
            if (script != null)
            {
                Behaviour_Switch newScript = (Behaviour_Switch)script;
                if (newScript.Coded)
                    actionList.Add(new Info_Action("Hack", script));
                else
                    actionList.Add(new Info_Action("Use", script));
            }

            script = hit.GetComponent<Behaviour_Container>();
            var isPlayer = hit.gameObject.tag == ("Player") ? true : false;
            if (script != null && !isPlayer)
            {
                Behaviour_Container container = (Behaviour_Container)script;
                actionList.Add(new Info_Action("Search", script));
            }
            script = hit.GetComponent<Behaviour_Cover>();
            if (script != null)
            {
                Behaviour_Cover cover = (Behaviour_Cover)script;
                if (cover.InUse)
                    actionList.Add(new Info_Action("Push back", script));
                else
                    actionList.Add(new Info_Action("Pull out", script));
            }
        }
        Physics2D.queriesHitTriggers = false;
        return actionList;
    }
}
