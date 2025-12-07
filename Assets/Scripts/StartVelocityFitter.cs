using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartVelocityFitter : MonoBehaviour
{
    IEnumerator Start()
    {
        var bodies = GetComponentsInChildren<Rigidbody2D>();

        for (int i = 0; i < 30; i++)
        {
            foreach (var item in bodies)
            {
                item.velocity = Vector2.zero;
                item.angularVelocity = 0;
            }

            yield return null;
        }

         
    }
}
