using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MonsterBoss : BossParentScript
{
    Transform player;
    public Rigidbody2D rb;
    public Vector3 centerCoords;
    bool moveFreely;
    bool canAttack;
    public float moveSpeed;
    public Animator anim;
    public GameObject leftSideAttack;
    public GameObject rightSideAttack;
    public GameObject summonedStaff;
    public GameObject leftCleave;
    public GameObject rightCleave;
    public SpreadShot rotatingBullets;
    public float shotRepeats;
    public float shotDeg;
    private void Start()
    {
        bgEffect.SetActive(true);
        player = GameObject.Find("Player").transform;
        rb = GetComponent<Rigidbody2D>();
        CurHealth = maxHealth;
        StartCoroutine(moveToMiddle());
        TakeDamage(0);
        Invoke("SummonStaff",Random.Range(2,5));
        Invoke("CleavePlayer",Random.Range(3,8));
        Invoke("PunchAttack",5);
        Invoke("Shoot", Random.Range(8, 16));
    }
    private void Update()
    {
        
    }
    void Shoot()
    {
        StartCoroutine(bullets());
    }

    IEnumerator MoveAround()
    {
        Vector2 offset = new Vector2(Random.Range(-18,18), Random.Range(-4,4));
        Vector2 dir = ((Vector2)centerCoords+offset) - (Vector2)transform.position;
        moveFreely = false;
        canAttack = false;
        rb.velocity = dir.normalized * moveSpeed;
        yield return new WaitForSeconds(dir.magnitude / moveSpeed);
        rb.velocity = Vector2.zero;
        moveFreely = true;
        yield return new WaitForSeconds(Random.Range(2, 5));
        Vector2 distv = centerCoords - transform.position;
        if (Mathf.Abs(distv.y) > 4 || Mathf.Abs(distv.x) > 18)
        {
            StartCoroutine(moveToMiddle());
        }
        else
        {
            StartCoroutine(MoveAround());
        }
        

        yield return null;
    }

    IEnumerator bullets()
    {
        rotatingBullets.OnShoot();
        for (int i=0;i<shotRepeats;i++)
        {
            rotatingBullets.transform.Rotate(0, 0, shotDeg);
            yield return new WaitForSeconds(0.2f);
            rotatingBullets.OnShoot();
        }
        rotatingBullets.transform.rotation = Quaternion.Euler(0, 0, 0);
        Invoke("Shoot", Random.Range(6, 12));
        yield return null;
    }
    public void CleavePlayer()
    {
        
        float lor = Random.Range(0, 100);
        if (lor < 50)
        {
            GameObject cleave = Instantiate(leftCleave, player.position - Vector3.right * 3, Quaternion.identity);
            Destroy(cleave, 5);
        }
        else
        {
            GameObject cleave = Instantiate(rightCleave, player.position + Vector3.right * 3, Quaternion.identity);
            Destroy(cleave, 5);
        }
        Invoke("CleavePlayer", 15);
    }
    public void SummonStaff()
    {
        float rX = Random.Range(-3, 3);
        float rY = Random.Range(-3, 3);
        Vector2 offset = new Vector2(rX, rY);
        GameObject s = Instantiate(summonedStaff,(Vector2)player.position + offset,Quaternion.identity);
        Destroy(s, 6);
        Invoke("SummonStaff", 12);
    }
    public void PunchAttack()
    {
        if (transform.position.x > player.position.x)
        {
            anim.SetTrigger("PunchLeft");
        }
        else
        {
            anim.SetTrigger("PunchRight");
        }
        Invoke("PunchAttack", Random.Range(6,10));
    }

    IEnumerator moveToMiddle()
    {
        Vector2 dir = centerCoords - transform.position;
        moveFreely = false;
        canAttack = false;
        rb.velocity = dir.normalized * moveSpeed;
        yield return new WaitForSeconds(dir.magnitude/moveSpeed);
        rb.velocity = Vector2.zero;
        moveFreely = true;

        yield return new WaitForSeconds(Random.Range(5, 15));
        StartCoroutine(MoveAround());

        yield return null;
    }
    public void SpawnLeftAttack()
    {
        GameObject la = Instantiate(leftSideAttack,transform.position,Quaternion.identity);
        Destroy(la, 6);
    }
    public void SpawnRightAttack()
    {
        GameObject la = Instantiate(rightSideAttack, transform.position, Quaternion.identity);
        Destroy(la, 6);
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
