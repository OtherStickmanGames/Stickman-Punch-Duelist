using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CollisionPartDetector : MonoBehaviour
{
    public Action<int> OnCollision;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.collider.gameObject.CompareTag("Obstacle"))
            return;

        var force = (int)collision.relativeVelocity.sqrMagnitude;

        if (force > 8000)
        {
            OnCollision?.Invoke(force);
        }
    }
}
