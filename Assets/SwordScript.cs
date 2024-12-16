using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordScript : MonoBehaviour
{
    public Transform player;
    public Transform curTarget;
    public bool targeting;
    public Rigidbody2D rb;
    public float maxVel;
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector2 dir = player.position - transform.position;

        if (!targeting)
        {
            rb.AddForce(dir * Mathf.Pow(dir.magnitude, 2));
            transform.Rotate(0, 0, dir.magnitude);

            if (rb.velocity.magnitude > maxVel)
            {
                rb.velocity = Vector2.Lerp(rb.velocity, Vector3.zero, Time.deltaTime * 5);
            }
        }
        
    }
}
