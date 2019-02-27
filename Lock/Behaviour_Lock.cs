using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Behaviour_Door))]
public class Behaviour_Lock : MonoBehaviour {

    [SerializeField] private int level;
    private Behaviour_Door door; //Door script of this gameObject
    public bool untouched;//Has player interacted with this lock
    public bool unlocked;
    public List<GameObject> PinOrder { get; set; } //Order in which pins should be picked
    public List<float> PinDivides { get;  private set; } //Saved pin randomizers
    public int PinCount { get { return level+2; } } //Number of pins in this lock

    // Use this for initialization
    void Start ()
    {
        PinDivides = new List<float>();
        untouched = true;
        door = GetComponent<Behaviour_Door>();
        unlocked = door.Unlocked;
        for (int i = 0; i < PinCount; i++)
        {
            PinDivides.Add(Random.Range(0, 3.5f));
        }
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (unlocked)
            door.DisableLock();
        else
            door.EnableLock();
	}
}
