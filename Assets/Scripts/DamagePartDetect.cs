using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamagePartDetect : MonoBehaviour
{

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.collider.transform.root.GetComponent<Stickman>())
            return;

        var body = collision.rigidbody;
        if(body && collision.relativeVelocity.magnitude > 17)
        {
            var forceX = Mathf.Abs(collision.relativeVelocity.x) * transform.lossyScale.x;
            Vector2 force = new (forceX, collision.relativeVelocity.y);

            float crit = Random.Range(0, 10) > 5 ? 5 : 1;

            force.y /= crit;

            body.AddForce(30 * crit * force, ForceMode2D.Impulse);

            var stickman = body.transform.root.GetComponent<Stickman>();

            if (stickman.HP > 0)
            {
                var damage = (int)(collision.relativeVelocity.sqrMagnitude / 100);
                
                stickman.Damage(damage);
            }

            var pos = collision.contacts[0].point;
            SoundManager.Punch(pos);
            EffectManager.Punch(pos);
        }

        
    }
}
