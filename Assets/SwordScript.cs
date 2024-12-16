using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordScript : MonoBehaviour
{
    public Transform playerChar;
    public Transform curTarget;
    public bool targeting;
    public Rigidbody2D rb;
    [Header("Values")]
    public float maxVel;
    public float curRange;
    [SerializeField]
    float rangeMod;
    [SerializeField]
    float maxVelMod;

    ContactFilter2D cf;
    Collider2D[] enemyColls;
    void Start()
    {
        cf.layerMask = 6;

        curTarget = playerChar;
    }

    void FixedUpdate()
    {
        Vector2 dir = curTarget.position - transform.position;

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

    void FindEnemy()
    {
        Physics2D.OverlapCircle(transform.position, curRange, cf, enemyColls);
        for (int i = 0; i < enemyColls.Length; i++)
        {

        }
    }

    public void Pull()
    {


    }
}
