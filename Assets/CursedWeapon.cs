using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursedWeapon : MonoBehaviour
{
    public float maxHealth;
    public float curHealth;
    public GameObject enemy;
    public Transform player;

    public float detectRange;
    public bool playerFound;

    private void Start()
    {
        player = GameObject.Find("Player").transform;
    }

    public virtual void TakeDamage(float amount)
    {
        curHealth -= amount;
        if (curHealth <= 0) {
            OnDeath();
        }
    }

    public virtual void FindPlayer()
    {
        float pDist = (player.position - transform.position).magnitude;
        if (pDist <= detectRange) { playerFound = true; }
    }

    public virtual void OnDeath()
    {
        enemy.GetComponent<Enemy>().Purify();
        Destroy(gameObject);
    }

    // Update is called once per frame
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<SwordScript>() != null)
        {
            if (collision.GetComponent<SwordScript>().isSlashing)
            {
                TakeDamage(3);
            }
            else
            {
                TakeDamage(1);
            }
        }
    }
}
