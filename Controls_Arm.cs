using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controls_Arm : MonoBehaviour {

    [SerializeField]
    private bool isShaky;
    private Controls_Character characterControl;
    private static bool isPaused;

    private void Awake()
    {
        Global_GameManager.OnPause += delegate { isPaused = true;};
        Global_GameManager.UnPause += delegate { isPaused = false;};
    }

    void Start ()
    {
        characterControl = GetComponentInParent<Controls_Character>();
	}

    private void Update()
    {
        if (!isPaused)
        {
            if (characterControl.Stress > 100)
            {
                isShaky = true;
            }
            else
            {
                isShaky = false;
            }
        }
    }

    void FixedUpdate ()
    {
		if(isShaky)
        {
            Rotate(Random.value - 0.5f);
        }
	}

    public void Rotate(float direction)
    {
        transform.Rotate(0, 0, direction);
        float z;
        if (transform.localEulerAngles.z<=360 && transform.localEulerAngles.z >= 200)
           z = Mathf.Clamp(transform.localEulerAngles.z, 280, 360);
        else
        z = Mathf.Clamp(transform.localEulerAngles.z, 0, 45);
        transform.localEulerAngles = new Vector3(0,0,z);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.GetComponent<Behaviour_Material>() != null)
        {
            Rotate(-75);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Behaviour_Material>() != null)
        {
            Collider2D collider = GetComponent<Collider2D>();
            Rotate(Physics2D.Linecast(collision.transform.position, transform.TransformPoint(collider.offset)).point.y >
                collider.bounds.min.y ? -2 : 2);
        }
    }
}
