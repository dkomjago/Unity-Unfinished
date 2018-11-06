using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Behaviour_Cover : MonoBehaviour {

    private bool inUse = false;
    public bool InUse
    {
        get { return inUse; }
    }
    private new Collider2D collider2D;
    private Behaviour_Container containerScript;
    [SerializeField] private float rotateAngle;

    private void Awake()
    {
        collider2D = GetComponent<Collider2D>();
    }
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Pull()
    {
        containerScript = GetComponent<Behaviour_Container>();
        containerScript.enabled = false;
        collider2D.isTrigger = false;
        inUse = true;
        StartCoroutine(Shadow(true));
    }

    public void Push()
    {
        if(containerScript != null)
            containerScript.enabled = true;
        collider2D.isTrigger = true;
        inUse = false;
        StartCoroutine(Shadow(false));
    }

    private IEnumerator Shadow(bool add)
    {
        var obstacle = GetComponentInChildren<Light2D.LightObstacleSprite>();
        for(int i=0;i<25;i++)
        {
            if (add)
                obstacle.AdditiveColor.a += 0.04f;
            else
                obstacle.AdditiveColor.a -= 0.04f;
            yield return new WaitForSecondsRealtime(0.01f);
        }
    }
}
