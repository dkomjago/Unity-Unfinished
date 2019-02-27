using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Behaviour_Character : MonoBehaviour {

    [SerializeField]
    private GameObject bloodPrefab;

    public void Damage(Info_Shot shotInfo)
    {
        GameObject particles = Instantiate(bloodPrefab, shotInfo.Point, Quaternion.identity);
        Destroy(particles, 1);
    }
}
