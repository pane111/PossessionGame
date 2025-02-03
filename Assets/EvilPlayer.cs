using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EvilPlayer : MonoBehaviour
{
    public float moveSpeed;
    public float dashForce;
    public float minDashCd;
    public float maxDashCd;
    public float maintainedDistance;
    public float turnDeg;
    public float turnChance;
    Vector2 curDir;
    Transform player;
    public bool moving = true;
    public GameObject circleAttack;
    public ParticleSystem afterimage;
    public float stepDelay;
    public SpriteRenderer sr;
    Rigidbody2D rb;
    float turning=0;
    void Start()
    {
        player = GameObject.Find("Player").transform;
        player.GetComponent<Player>().sword.GetComponent<SwordScript>().curTarget = transform;
        DecideDirection();
        Invoke("StartDash", Random.Range(minDashCd, maxDashCd));
        rb = GetComponent<Rigidbody2D>();
        Step();
    }
    void Step()
    {
        sr.flipX = !sr.flipX;
        Invoke("Step", stepDelay);
    }

    // Update is called once per frame
    void Update()
    {
        if(moving)
        {
            rb.velocity = curDir.normalized * moveSpeed;
            curDir = Vector3.Lerp(curDir, Quaternion.AngleAxis(turning, Vector3.forward) * curDir, Time.deltaTime * 3);
        }
    }
    void DecideDirection()
    {
        float r = Random.Range(0, 100);

        


        if (r <= turnChance) {

            Vector3 dir = player.position-transform.position;
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
        if (r2<=50)
        {
            turning = Random.Range(-90, 90);
        }
        else
        {
            turning = 0;
        }

        Invoke("DecideDirection", Random.Range(0.2f, 1));
    }
    void StartDash()
    {
        StartCoroutine(dash());
    }
    IEnumerator dash()
    {
        moving = false;
        curDir.Normalize();
        rb.velocity = Vector2.zero;
        GameObject sp = Instantiate(circleAttack, transform.position, Quaternion.identity);
        sp.GetComponent<AttackSpawner>().dir = curDir*1.2f;
        sp.GetComponent<AttackSpawner>().repeatDelay = 0.05f;
        sp.GetComponent<AttackSpawner>().repeatTimes = 5;

        afterimage.Play();
        rb.AddForce(curDir * dashForce, ForceMode2D.Impulse);
        yield return new WaitForSeconds(0.1f);
        rb.velocity = Vector2.zero;
        moving = true;
        afterimage.Stop();
        Invoke("StartDash",Random.Range(minDashCd, maxDashCd));
        yield return null;
    }
}
