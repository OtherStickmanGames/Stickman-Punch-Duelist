using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour
{

    private void Start()
    {
        
    }

    //private void OnCollisionEnter2D(Collision2D collision)
    //{
    //    if (Random.Range(0, 10) > 5)
    //        return;

    //    if (collision.gameObject.CompareTag("Obstacle") && collision.gameObject.GetComponent<Rigidbody2D>().bodyType == RigidbodyType2D.Dynamic)
    //    {
    //        Physics2D.IgnoreCollision(gameObject.GetComponent<Collider2D>(), collision.gameObject.GetComponent<Collider2D>());
    //    }
    //}

    private void Update()
    {
        if(transform.position.y < -800)
        {
            Destroy(gameObject);
        }
    }
}
