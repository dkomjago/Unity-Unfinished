using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

[RequireComponent(typeof(Controls_Arm))]
public class Controls_Arm_Player : MonoBehaviour {

    // Use this for initialization
    Controls_Arm arm;

    private static bool isPaused;

    private void Awake()
    {
        Global_GameManager.OnPause += delegate { isPaused = true; };
        Global_GameManager.UnPause += delegate { isPaused = false; };
    }

    void Start () {
        arm = GetComponent<Controls_Arm>();		
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        if (!isPaused)
        {
            float direction = CrossPlatformInputManager.GetAxis("GunVertical");
            arm.Rotate(direction);
        }
	}
}
