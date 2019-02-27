using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Behavior_Sound {

    public static void MakeSound(float volume , Vector3 source)
    {
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(source, volume);
        int i = 0;
        while (i < hitColliders.Length)
        {
            if (volume > 100)
                hitColliders[i].SendMessage("AddCriticalStress", SendMessageOptions.DontRequireReceiver);
            else
                hitColliders[i].SendMessage("AddStress", SendMessageOptions.DontRequireReceiver);
            i++;
        }
    }
}
