using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class FleshCreature : MonoBehaviour
{
    SpreadShot sp;
    Animator anim;
    bool canMove=true;
    Rigidbody2D rb;
    Transform player;
    public float moveSpeed;
    public Vector3 curDir;
    public float turnChance;
    public float turning;
    public float maintainedDistance;
    public float turnDeg;
    public int health;
    public GameObject bloodSplat;
    void Start()
    {
        sp = GetComponent<SpreadShot>();   
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.Find("Player").transform;
        StartCoroutine(Attack());
        DecideDirection();
        Destroy(gameObject, 60);
    }

    // Update is called once per frame
    void Update()
    {
        
        if (canMove)
        {
            rb.velocity = curDir.normalized * moveSpeed;
            curDir = Vector3.Lerp(curDir, Quaternion.AngleAxis(turning, Vector3.forward) * curDir, Time.deltaTime * 3);
        }
    }
    void DecideDirection()
    {
        float r = Random.Range(0, 100);


        if (r <= turnChance)
        {

            Vector3 dir = player.position - transform.position;
            dir.Normalize();

            float deg = Random.Range(-turnDeg, turnDeg);

            dir = Quaternion.AngleAxis(deg, Vector3.forward) * dir;

            float dist = (player.position - transform.position).magnitude;
            if (dist < maintainedDistance)
            {
                curDir = -dir;
            }
            else
            {
                curDir = dir;
            }


        }
        else
        {
            Vector3 dir = curDir;
            float deg = Random.Range(-turnDeg, turnDeg);

            dir = Quaternion.AngleAxis(deg, Vector3.forward) * dir;

            curDir = dir;
        }

        float r2 = Random.Range(0, 100);
        if (r2 <= 50)
        {
            turning = Random.Range(-90, 90);
        }
        else
        {
            turning = 0;
        }

        Invoke("DecideDirection", Random.Range(0.1f, 0.4f));
    }

    IEnumerator Attack()
    {
        yield return new WaitForSeconds(2.5f);
        canMove = false;
        rb.velocity= Vector3.zero;
        anim.SetTrigger("Attack");
        yield return new WaitForSeconds(1);
        sp.OnShoot();
        sp.addedDeg += 45;
        canMove = true;
        StartCoroutine(Attack());
        yield return null;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<SwordScript>()!=null)
        {
            health--;
            StartCoroutine(DamageAnim());
            if (health < 0)
            {
                Instantiate(bloodSplat,transform.position, Quaternion.identity);
                Destroy(gameObject);
            }
        }
    }

    IEnumerator DamageAnim()
    {
        GetComponentInChildren<SpriteRenderer>().color = Color.red;
        yield return new WaitForSeconds(0.1f);
        GetComponentInChildren<SpriteRenderer>().color = Color.white;
        yield return null;
    }
}
