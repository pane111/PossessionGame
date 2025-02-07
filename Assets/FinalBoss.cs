using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinalBoss : BossParentScript
{
    public Color bgColor;
    Transform player;
    public Animator clockAnim;
    private void Start()
    {
        player = GameObject.Find("Player").transform;
        CurHealth = maxHealth;
        TakeDamage(0);
        Camera.main.backgroundColor = bgColor;
        bgEffect.SetActive(true);
        StartCoroutine(TimeManip());
    }

    IEnumerator TimeManip()
    {
        float r = Random.Range(0, 100);
        if (r <= 50)
        {
            Time.timeScale = 2f;
            Time.fixedDeltaTime = 0.02F * Time.timeScale;
        }
        else
        {
            Time.timeScale = 0.65f;
            Time.fixedDeltaTime = 0.02F * Time.timeScale;
        }
        clockAnim.SetTrigger("Trigger");
        yield return new WaitForSecondsRealtime(8);
        Time.timeScale = 1;
        Time.fixedDeltaTime = 0.02F * Time.timeScale;
        clockAnim.SetTrigger("Trigger");
        yield return new WaitForSecondsRealtime(Random.Range(5,8));
        StartCoroutine(TimeManip());
        yield return null;
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
