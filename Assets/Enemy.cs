using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float maxHealth = 100;
    public float curHealth = 100;
    public float speed;
    public float detectRange;
    public bool playerFound;
    public Rigidbody2D rb;

    public Transform player;
    void Start()
    {
        player = GameObject.Find("Player").transform;
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 pDir = player.position - transform.position;
        pDir.Normalize();
        RaycastHit2D hit;
        hit = Physics2D.Raycast(transform.position, pDir, detectRange);
        if (hit)
        {
            if (hit.transform == player)
            {
                playerFound = true;
                GetComponent<Animator>().enabled = true;
            }
            else
            {
                playerFound = false;
                GetComponent<Animator>().enabled = false;
            }
        }

        if (playerFound)
        {
            rb.velocity = pDir * speed;
        }
        else
        {
            rb.velocity = Vector2.zero;
        }
    }

    public void TakeDamage(float amount)
    {
        curHealth -= amount;
    }
}
