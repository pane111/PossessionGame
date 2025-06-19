using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SharpHoming : MonoBehaviour
{
    Transform player;
    Rigidbody2D rb;
    void Start()
    {
        player = GameObject.Find("Player").transform;
        rb = GetComponent<Rigidbody2D>();
        Invoke("HomeIn", Random.Range(0.5f, 2));
    }

    public void HomeIn()
    {
        Vector2 dir = player.position - transform.position;
        rb.velocity = dir.normalized * 20;
    }
}
