using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Lock : MonoBehaviour {

    [SerializeField] private List<GameObject> pinEditor; //Pins in lock UI element (set in Editor)
    public static List<GameObject> Pins { get; private set; }
    public static List<GameObject> ActivePins { get; private set; }//List of enabled pin GameObjects
    public static List<GameObject> PinOrder { get; private set; }//Order in which pins should be picked
    public static UI_Lock Instance { get; private set; }

    public static Behaviour_Lock currentLock;//Lock script that is being modified

    // Use this for initialization
    private UI_Lock ()
    {
        Instance = this;
    }

    public void Awake()
    {
        Pins = pinEditor;
        RectTransform rect = GetComponent<RectTransform>();
        foreach (var pin in Pins)
        {
            pin.SetActive(false);
        }
    }

    private void OnEnable()
    {
        ActivePins = new List<GameObject>();
        for (int i = 1; i <= currentLock.PinCount; i++)
        {
            Pins[i-1].GetComponent<Behaviour_LockPin>().pinRandomizer = currentLock.PinDivides[i-1];
            ActivePins.Add(Pins[i - 1]);
            Pins[i-1].SetActive(true);
        }
        Controls_Lock.SelectedPin = Pins[0];
        if (currentLock.untouched)
        {
            PinOrder = new List<GameObject>();
            foreach (var pin in Pins)
            {
                if(pin.activeInHierarchy)
                PinOrder.Add(pin);
            }
            PinOrder.Shuffle();
            currentLock.PinOrder = PinOrder;
        }
        else
            PinOrder = currentLock.PinOrder;
    }

    private void OnDisable()
    {
        Pins.ForEach(x => x.SetActive(false));
    }
}
