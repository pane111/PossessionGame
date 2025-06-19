using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bouncer : MonoBehaviour
{
    Rigidbody2D rb;
    public Vector2 initBoost;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.AddForce(initBoost);
    }

}
