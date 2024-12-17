using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordScript : MonoBehaviour
{
    public Transform playerChar;
    public Transform curTarget;
    private bool _idling;
    public bool Idling
    {
        get => _idling;
        set
        {
            if (value) { speedMod = speedModIdle; }
            else { speedMod = speedModAttack; }
            _idling = value;
        }
    }
    bool moveFreely;
    public Rigidbody2D rb;
    [Header("Values")]
    public float maxVel;
    public float speedModIdle;
    public float speedModAttack;
    private float speedMod;
    public float curRange;
    [SerializeField]
    float rangePossMod;
    [SerializeField]
    float maxVelPossMod;
    public float pullSpeed;
    public float maxVelocity; //Maximum velocity the sword can go at
    private Vector2 pullPos;
    public GameObject reticle;
    public ParticleSystem slashes;
    public float slashAttackTimer = 2;
    private float curSAT = 2;
    public bool isSlashing=false;
    public ContactFilter2D cf;
    List<Collider2D> enemyColls = new List<Collider2D>();
    void Start()
    {
        curTarget = playerChar;
        speedMod = speedModIdle;
        Idling = true;
        moveFreely = true;
    }

    void FixedUpdate()
    {
        Vector2 dir = curTarget.position - transform.position;

        //TARGETING
        if (Idling) { FindEnemy(); }
        else
        {
            curSAT -= Time.deltaTime;
            if (curSAT < 0)
            {
                StartCoroutine(SlashAttack());
                curSAT = 9999;
            }
        }

        //MOVING
        if (moveFreely)
        {
            rb.AddForce(dir * Mathf.Pow(dir.magnitude, 2) * speedMod);
            transform.Rotate(0, 0, dir.magnitude);
            if (rb.velocity.magnitude > maxVel)
            {
                rb.velocity = Vector2.Lerp(rb.velocity, Vector3.zero, Time.deltaTime * 5);
            }
        }
        if (rb.velocity.magnitude > maxVelocity)
            rb.velocity = rb.velocity.normalized * maxVelocity;

        //PULL CHECK
        if (!moveFreely && Vector2.Distance(pullPos, transform.position) < 0.5f) { moveFreely = true; FindEnemy(); playerChar.gameObject.GetComponent<Player>().leash.enabled = false; }
    }

    IEnumerator SlashAttack()
    {
        moveFreely = false;
        Vector2 dir = curTarget.position - transform.position;
        rb.velocity = -dir.normalized * 15;
        yield return new WaitForSeconds(0.2f);
        rb.AddTorque(100, ForceMode2D.Impulse);
        rb.velocity = Vector2.zero;
        yield return new WaitForSeconds(0.3f);
        dir = curTarget.position - transform.position;
        rb.AddForce(dir.normalized * 250, ForceMode2D.Impulse);
        
        isSlashing = true;
        slashes.Play();
        yield return new WaitForSeconds(0.25f);
        rb.velocity = Vector2.zero;
        isSlashing = false;
        slashes.Stop();
        yield return new WaitForSeconds(0.2f);
        rb.AddTorque(-80, ForceMode2D.Impulse);
        moveFreely = true;
        curSAT = slashAttackTimer;
        yield return null;
    }

    void FindEnemy()
    {
        if (Physics2D.OverlapCircle(transform.position, curRange * rangePossMod, cf, enemyColls) != 0)
        {
            float smallestTargetDist = curRange * rangePossMod;
            int closestEnemy = 0;
            for (int i = 0; i < enemyColls.Count; i++)
            {
                Vector2 targetDir = enemyColls[i].transform.position - transform.position;
                float currTargetDist = targetDir.magnitude;
                if (currTargetDist < smallestTargetDist) { smallestTargetDist = currTargetDist; closestEnemy = i; }
            }
            Idling = false;
            curTarget = enemyColls[closestEnemy].transform;
            reticle.SetActive(true);
            reticle.transform.position = curTarget.position - Vector3.forward;
            reticle.transform.parent = curTarget;
        }
        else { Idling = true; }
    }
    public void OnEnemyDeath()
    {
        reticle.transform.parent = null;
        reticle.SetActive(false);

        float randTime = Random.Range(1f, 4f);
        Invoke("Untarget", randTime);
    }

    void Untarget()
    {
        
        curTarget=playerChar.transform;
        FindEnemy();

    }

    public void Pull()
    {
        curTarget = playerChar.transform;
        moveFreely = false;
        rb.velocity = Vector2.zero;
        rb.AddForce((playerChar.position - transform.position) * pullSpeed, ForceMode2D.Force);
        pullPos = playerChar.position;
    }
}
