using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Lever : MonoBehaviour {

    [SerializeField] private Sprite leverGreen;
    [SerializeField] private Sprite leverRed;
    [SerializeField] private UnityEngine.UI.Image imgComponent;
    private static UI_Lever instance;

    private UI_Lever()
    {
        instance = this;
    }

    // Use this for initialization
    void Start ()
    {
        imgComponent.sprite = leverRed;
	}
    //Sets lever image
    public static void SetLeverStatus(bool green) 
    {
        instance.imgComponent.sprite = green ? instance.leverGreen : instance.leverRed;
    }
}
