using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CopyTransformPosition : MonoBehaviour
{
    [SerializeField] Transform from;
    [SerializeField] Transform to;

    [SerializeField] Rigidbody2D body;

    float offset;

    private void Awake()
    {
        offset = from.position.x - transform.position.x;
    }

    void Update()
    {
        to.position = from.position;
        //transform.position = new Vector3(from.position.x, transform.position.y, transform.position.z);
    }

    private void FixedUpdate()
    {
        //body.MoveRotation(Mathf.LerpAngle(body.rotation, 0, 100 * Time.deltaTime));
    }
}
