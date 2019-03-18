using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class Behaviour_LockPin : MonoBehaviour {

    public bool dropped;
    public bool holding;
    private Animator anim;
    public RectTransform upperPin; //Set pins in editor
    public RectTransform lowerPin;
    public float Fraction { get; private set; } //Upper/Lower pin divide
    public Vector3 droppedPosition;
    private Vector3 defaultDroppedPosition;
    private float stopPoint; //Animation stopping point
    public float pinRandomizer;

    // Set Animation and lower pin drop position to default state
    public void ResetState()
    {
        anim.Play("Operate", 0, 1);
        droppedPosition = lowerPin.parent.position;
    }

    private void OnEnable()
    {
        anim = GetComponent<Animator>(); //Get animator each time because of animator bug
        lowerPin.sizeDelta = new Vector2(lowerPin.sizeDelta.x, lowerPin.sizeDelta.y + pinRandomizer); //Set randomized divides in pin
        upperPin.sizeDelta = new Vector2(upperPin.sizeDelta.x, upperPin.sizeDelta.y - pinRandomizer);
        Fraction = lowerPin.sizeDelta.y / upperPin.sizeDelta.y;
        ResetState();
        defaultDroppedPosition = droppedPosition;
    }

    private void OnDisable()
    {
        ResetState();
        lowerPin.sizeDelta = new Vector2(lowerPin.sizeDelta.x, lowerPin.sizeDelta.y - pinRandomizer); //Set divides to default
        upperPin.sizeDelta = new Vector2(upperPin.sizeDelta.x, upperPin.sizeDelta.y + pinRandomizer);
        holding = false;
    }
    
    void Update()
    {
        if (dropped) //If pin is picked adjust lower pin position
            lowerPin.parent.position = droppedPosition;
        if (holding) //If pin picked lock position
            stopPoint = Fraction;
        else
        {
            stopPoint = 1;
            if (anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= stopPoint)
                dropped = false;
        }
        if (anim != Controls_Lock.SelectedAnim && !Controls_Lock.PrecedingAnims.Contains(anim)) //Return to closed state if not selected
        {
            if (anim.GetCurrentAnimatorStateInfo(0).normalizedTime <= stopPoint)
                anim.SetFloat("Speed", 1);
            else
                anim.SetFloat("Speed", 0);
        }
        if(Controls_Lock.HoldPins.Contains(gameObject)) //Hold state if pin is picked
            anim.SetFloat("Speed", 0);
    }
}
