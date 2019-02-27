using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Behaviour_Switch : MonoBehaviour {

    [SerializeField] private GameObject slave;
    [SerializeField] private bool isOn;
    [SerializeField] private bool coded;
    public bool Coded{ get { return coded; } }

    public void Use()
    {
        if (isOn)
        {
            slave.SetActive(false);
            isOn = false;
        }
        else
        {
            slave.SetActive(true);
            isOn = true;
        }
    }
}

//idea - fusebox