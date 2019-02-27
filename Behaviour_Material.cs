using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Behaviour_Material : MonoBehaviour {

    [SerializeField] private GameObject sparkPrefab;
    [SerializeField] private const int shotDistance = 10; //Shot distance after peneteration

    public void Damage(Info_Shot shotInfo)
    {
        StartCoroutine(Peneterate(shotInfo));
        GameObject particles = Instantiate(sparkPrefab, shotInfo.Point, Quaternion.identity);
        Destroy(particles, 0.2f);
    }

    IEnumerator Peneterate(Info_Shot shotInfo)
    {
        yield return new WaitForSecondsRealtime(0.1f);
        RaycastHit2D[] hit = Physics2D.RaycastAll(shotInfo.Point, shotInfo.Direction, shotDistance);
        if (hit.Length > 1 && hit[1].collider)
        {
            hit[1].collider.SendMessage("Damage", new Info_Shot(hit[1].point, shotInfo), SendMessageOptions.DontRequireReceiver);
        }
    }
}
