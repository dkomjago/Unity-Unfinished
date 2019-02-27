using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Behaviour_RoomLight : MonoBehaviour {

    bool discovered=true;
    GameObject player;

    enum LightStatus { off = 0 , on = 90, dim = 60 }
    private LightStatus rememberedStatus;
    private LightStatus realStatus = LightStatus.on;
    private LightStatus status;
    private new Light2D.LightSprite light;

    // Use this for initialization
    void Start ()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        light = GetComponent<Light2D.LightSprite>();
        if(discovered)
            rememberedStatus = LightStatus.dim;
        else
            rememberedStatus = LightStatus.off;
	}
	
	// Update is called once per frame
	void Update ()
    {
        light.Color.a = 0.01f * (int)status;
	}

    private void OnWillRenderObject()
    {
        RaycastHit2D hit = Physics2D.Linecast(transform.position, player.transform.position);
        if(hit.transform == player.transform)
        {
            status = realStatus;
            if(realStatus == LightStatus.on)
            rememberedStatus = LightStatus.dim;
        }
        else
            status = rememberedStatus;
    }
}
