using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Behaviour_Door))]
public class Behaviour_Door_Apartment : MonoBehaviour {

    static Object doorNumberPrefab;
    static int doorNumber = 0;
    TMPro.TMP_Text doorNumberText;

    // Use this for initialization
    void Awake () {
        doorNumber++;
        if(doorNumberPrefab == null)
        doorNumberPrefab = Resources.Load("Prefabs/LevelBuilder/DoorNumber");
        GameObject doorNumberControl = Instantiate(doorNumberPrefab,transform.position,Quaternion.identity) as GameObject;
        doorNumberText = doorNumberControl.GetComponent<TMPro.TMP_Text>();
        doorNumberText.text = doorNumber.ToString();
    }
	
	// Update is called once per frame
	void Update () {
        if (transform.localScale.x < 0)
            doorNumberText.alignment = TMPro.TextAlignmentOptions.MidlineLeft;
    }
}
