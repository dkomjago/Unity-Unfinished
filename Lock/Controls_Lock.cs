using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using System.Linq;

public class Controls_Lock : MonoBehaviour {

    #region Selected pin
    private static GameObject selectedPin; //currently controlled pin
    public static GameObject SelectedPin
    {
        get { return selectedPin; }
        set
        {
            selectedPin = value;
            SelectedAnim = selectedPin.GetComponent<Animator>();
        }
    }
    public static Animator SelectedAnim { get; private set; }
    private int currentPosition;//Current index of selected pin in pin list
    #endregion
    public static List<GameObject> HoldPins { get; private set; } //Already picked pins
    public static List<Behaviour_LockPin> HoldPinResidue { get; private set; }//Dropped lowerpins from already picked pins
    public static List<Animator> PrecedingAnims { get; private set; }//Animator list of pins before selected pin
    private List<GameObject> pinsLeft; //Unpicked pins

    private float lockAxis; //Vertical lock input
    private bool checkingPin; //Is pin being checked by CheckPin()
    private bool sweetSpotReached;//Is ready to be picked
    [SerializeField] private RectTransform pick;//Transform of Pick GameObject (set in Editor)

    // Use this for initialization
    void Start() {}

    private void OnEnable()
    {
        Controls_Character.movementDisabled = true;
        ReturnToDefault();
    }

    private void OnDisable()
    {
        Controls_Character.movementDisabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        lockAxis = CrossPlatformInputManager.GetAxis("GunVertical") % 1;
        if (CrossPlatformInputManager.GetButtonDown("Horizontal") && SelectedAnim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1)
        {
            ChangeSelectedPin(CrossPlatformInputManager.GetAxis("Horizontal"));
        }
        if (CrossPlatformInputManager.GetButton("Fire2") && lockAxis >= 0 && SelectedAnim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0)
        {
            SelectedAnim.SetFloat("Speed", -lockAxis);
            foreach (var anim in PrecedingAnims)
            {
                if ((anim.GetCurrentAnimatorStateInfo(0).normalizedTime - SelectedAnim.GetCurrentAnimatorStateInfo(0).normalizedTime) > 0.42f)
                    anim.SetFloat("Speed", -lockAxis);
                else if (anim.GetCurrentAnimatorStateInfo(0).normalizedTime <= 1)
                    anim.SetFloat("Speed", 1);
                else
                    anim.SetFloat("Speed", 0);
            }
        }
        else if (SelectedAnim.GetCurrentAnimatorStateInfo(0).normalizedTime <= 1)
        {
            SelectedAnim.SetFloat("Speed", 1);
            foreach (var anim in PrecedingAnims)
            {
                if (anim.GetCurrentAnimatorStateInfo(0).normalizedTime <= 1)
                    anim.SetFloat("Speed", 1);
                else
                    anim.SetFloat("Speed", 0);
            }
        }
        else
        {
            SelectedAnim.SetFloat("Speed", 0);
            PrecedingAnims.ForEach(x => x.SetFloat("Speed", 0));
        }
        if (!checkingPin && SelectedPin == UI_Lock.PinOrder[currentPosition])
        {
            StartCoroutine(CheckPin());
        }
        if (SelectedAnim.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.56f) //Change dropped lower pin position if pick has reached them
            HoldPinResidue.ForEach(x => x.droppedPosition = new Vector3(x.droppedPosition.x, pick.GetChild(0).position.y));
        if (sweetSpotReached)
        {
            UI_Lever.SetLeverStatus(true);
            if (CrossPlatformInputManager.GetButtonDown("Fire1"))
                HoldPin();
        }
        else
            UI_Lever.SetLeverStatus(false);
        if (CrossPlatformInputManager.GetButton("Cancel")) // If Cancel Pressed disable lock UI
            gameObject.SetActive(false);
    }

    //Return to default lock state
    private void ReturnToDefault()
    {
        pick.gameObject.SetActive(true);
        sweetSpotReached = false;
        pinsLeft = UI_Lock.ActivePins;
        HoldPins = new List<GameObject>();
        currentPosition = 0;
        UI_Lock.ActivePins.ForEach(x => x.GetComponent<Behaviour_LockPin>().holding = false);
        CollectPrecedingPins();
    }

    /* Change current pin 
     * argument(direction)  (-1;0) -> left (0;1) -> right */
    private void ChangeSelectedPin(float direction)
    {
        pinsLeft = new List<GameObject>();
        pinsLeft.AddRange(UI_Lock.ActivePins.Except(HoldPins));
        if (direction > 0 && SelectedPin != pinsLeft[pinsLeft.Count - 1])
        {
            SelectedPin = pinsLeft[pinsLeft.FindIndex(x => x == SelectedPin) + 1]; 
        }
        else if (direction < 0 && SelectedPin != pinsLeft[0])
        {
            SelectedPin = pinsLeft[pinsLeft.FindIndex(x => x == SelectedPin) - 1];
        }
        CollectPrecedingPins();
    }

    //Lock pin state and exclude from selectable pins
    private void HoldPin()
    {
        Behaviour_LockPin controls = SelectedPin.GetComponent<Behaviour_LockPin>();
        controls.dropped = true;
        controls.holding = true;
        pinsLeft = new List<GameObject>();
        pinsLeft.AddRange(UI_Lock.ActivePins.Except(HoldPins));
        HoldPins.Add(SelectedPin);
        var tempSelectedPin = SelectedPin;
        if (SelectedPin != pinsLeft[pinsLeft.Count - 1])
        {
            SelectedPin = pinsLeft[pinsLeft.FindIndex(x => x == SelectedPin) + 1];
        }
        else if (SelectedPin != pinsLeft[0])
        {
            SelectedPin = pinsLeft[pinsLeft.FindIndex(x => x == SelectedPin) - 1];
        }
        pinsLeft.Remove(tempSelectedPin);
        CollectPrecedingPins();
        currentPosition += 1;
        if (currentPosition >= UI_Lock.ActivePins.Count)
        {
            if (!UI_Lock.currentLock.unlocked)
                UI_Lock.currentLock.unlocked = true;
            else
                UI_Lock.currentLock.unlocked = false;
            SelectedAnim.Rebind();
            gameObject.SetActive(false);
        }
    }

    //Make pick affect preceding pins
    private void CollectPrecedingPins()
    {
            var children = SelectedPin.GetComponentsInChildren<Transform>();
            foreach (var child in children)
            {
                if (child.name == "LowerPinEnd")
                {
                    pick.SetParent(child);
                    pick.anchoredPosition = Vector2.zero;
                }
            }
        PrecedingAnims = new List<Animator>();
        HoldPinResidue = new List<Behaviour_LockPin>();
        var index = pinsLeft.FindIndex(x => x == SelectedPin) + 1;
        var precedingPins = pinsLeft.GetRange(index, pinsLeft.Count - index);
        precedingPins.ForEach(x => PrecedingAnims.Add(x.GetComponent<Animator>()));
        foreach (var holdPin in HoldPins)
        {
            if (UI_Lock.ActivePins.FindIndex(x => x == holdPin) > UI_Lock.ActivePins.FindIndex(x => x == SelectedPin))
                HoldPinResidue.Add(holdPin.GetComponent<Behaviour_LockPin>());
        }
    }

    //Check if pin is ready to be unlocked
    private IEnumerator CheckPin()
    {
        checkingPin = true;
        yield return new WaitForSecondsRealtime(0.1f);
        float currentState = SelectedAnim.GetCurrentAnimatorStateInfo(0).normalizedTime;
        float sweetSpot = SelectedPin.GetComponent<Behaviour_LockPin>().Fraction;
        if (Mathf.Abs(sweetSpot - currentState) < 0.01f)
            sweetSpotReached = true;
        else
            sweetSpotReached = false;
        checkingPin = false;
    }
}
