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
    private Vector2 pullPos;

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

        //PULL CHECK
        if (!moveFreely && Vector2.Distance(pullPos, transform.position) < 0.5f) { moveFreely = true; FindEnemy(); playerChar.gameObject.GetComponent<Player>().leash.enabled = false; }
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
        }
        else { Idling = true; }
    }

    public void Pull()
    {
        curTarget = playerChar.transform;
        moveFreely = false;
        rb.velocity = Vector2.zero;
        rb.AddForce((playerChar.position - transform.position).normalized * pullSpeed, ForceMode2D.Force);
        pullPos = playerChar.position;
    }
}
