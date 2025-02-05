using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MonsterBoss : BossParentScript
{
    Transform player;
    private void Start()
    {
        player = GameObject.Find("Player").transform;
        CurHealth = maxHealth;
        TakeDamage(0);
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<SwordScript>() != null)
        {

            SwordScript sword = collision.GetComponent<SwordScript>();
            if (sword.curTarget == this.gameObject.transform) { sword.attacksCount++; }
            Vector2 dir = transform.position - collision.transform.position;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            Quaternion lDir = Quaternion.AngleAxis(angle, Vector3.forward);
            GameObject bs = Instantiate(GameManager.Instance.bSplatter, transform.position, lDir);

            bs.transform.position = (Vector2)transform.position + dir.normalized;
            if (collision.GetComponent<SwordScript>().isSlashing)
            {
                if (CurHealth - 3 <= 0)
                {
                    collision.GetComponent<SwordScript>().curTarget = player;
                }
                TakeDamage(3);

            }
            else
            {
                if (CurHealth - 1 <= 0)
                {
                    collision.GetComponent<SwordScript>().curTarget = player;
                }
                TakeDamage(1);
                if (sword.curTarget == sword.playerChar || sword.curTarget == sword.playerChar.gameObject.GetComponent<Player>().orbiter) { sword.curTarget = this.gameObject.transform; sword.Idling = false; sword.SIforNPC(); }
            }

        }
    }
}
