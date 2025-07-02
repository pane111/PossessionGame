using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    [Header("Basic Movement")]
    public float maxVel;
    public float speedModIdle;
    public float speedModAttack;
    private float speedMod;
    private float targetTimer;
    public float maxTargetTimer;
    [SerializeField]
    float maxVelPossMod;
    [Header("Pull")]
    public Image pullImg;
    public float pullSpeed;
    public float maxVelocity; //Maximum velocity the sword can go at
    public float pullCooldown;
    private Vector2 pullPos;
    [Header("Repell")]
    public Image repellImg;
    public float repellForce;
    public float repellRange;
    public float repellCooldown;
    public float repellDuration;
    public float callbackTime;
    public ParticleSystem repel;
    [Header("Attack")]
    public int attacksUntilUntarget;
    [HideInInspector] public int attacksCount;
    public float curRange;
    [SerializeField]
    float rangePossMod;
    public ParticleSystem slashes;
    public float slashAttackTimer = 2;
    private float curSAT = 2;
    public bool isSlashing = false;
    public ContactFilter2D cf;
    [Header("Other")]
    public GameObject reticle;
    List<Collider2D> enemyColls = new List<Collider2D>();
    public GameObject exclamation;
    bool exclFollowing = false;

    public bool controlled = false;
    Coroutine slash;
    
    void Start()
    {
        exclamation.SetActive(false);
        curTarget = playerChar;
        speedMod = speedModIdle;
        Idling = true;
        moveFreely = true;
        targetTimer = maxTargetTimer;
        attacksCount = 0;
    }

    private void Update()
    {
        if(exclFollowing)
        { exclamation.transform.position = new Vector3(transform.position.x, transform.position.y + 1.4f, transform.position.z); }
    }

    void FixedUpdate()
    {
        if (!controlled)
        {
            if (Idling)
            {
                targetTimer -= Time.deltaTime;
                if (targetTimer < 0)
                {
                    //FindEnemy();
                    targetTimer = maxTargetTimer;
                }
                curTarget = playerChar.gameObject.GetComponent<Player>().orbiter;
            }
            else
            {
                if (attacksCount == attacksUntilUntarget) { Untarget(); attacksCount = 0; }
                else
                {
                    curSAT -= Time.deltaTime;
                    if (curSAT < 0)
                    {
                        if (slash != null)
                            slash = null;
                        slash = StartCoroutine(SlashAttack());
                        curSAT = 9999;
                    }
                }
            }

            if (curTarget == null)
            {
                curTarget = playerChar.gameObject.GetComponent<Player>().orbiter;
                Idling = true;
            }

            Vector2 dir = curTarget.position - transform.position;

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

            if (!moveFreely && Vector2.Distance(pullPos, transform.position) < 0.5f) { moveFreely = true; playerChar.gameObject.GetComponent<Player>().leash.enabled = false; }
        }
        
    }
    IEnumerator SlashDir(Vector2 dir)
    {
        AudioManager.Instance.SwordBigSlash.Post(gameObject);
        moveFreely = false;
        rb.AddForce(dir.normalized * 250, ForceMode2D.Impulse);
        isSlashing = true;
        slashes.Play();
        yield return new WaitForSeconds(0.5f);
        rb.velocity = Vector2.zero;
        isSlashing = false;
        slashes.Stop();
        yield return new WaitForSeconds(0.2f);
        rb.AddTorque(-80, ForceMode2D.Impulse);
        moveFreely = true;
        curSAT = slashAttackTimer;
        yield return null;
    }
    IEnumerator SlashAttack()
    {
        AudioManager.Instance.SwordBigSlash.Post(gameObject);
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
    /*
    void FindEnemy()
    {
        if (Physics2D.OverlapCircle(transform.position, curRange * rangePossMod, cf, enemyColls) != 0)
        {
            AudioManager.Instance.SwordDetect.Post(gameObject);
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
            StartCoroutine(ShowInterest());
            
            //reticle.SetActive(true);
            //reticle.transform.position = curTarget.position - Vector3.forward;
            //reticle.transform.parent = curTarget;
            
}
        else { Idling = true; }
    }
    */
    IEnumerator ShowInterest()
    {
        print("huh");
        exclamation.SetActive(true);
        exclamation.transform.position = new Vector3(transform.position.x, transform.position.y + 1.4f, transform.position.z);
        exclFollowing = true;
        yield return new WaitForSeconds(0.6f);
        exclamation.SetActive(false);
        exclFollowing = false;
    }

    public void SIforNPC() { StartCoroutine(ShowInterest()); }

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
        Idling = true;
    }

    public void Pull()
    {
        AudioManager.Instance.Pull.Post(gameObject);
        curTarget = playerChar.transform;
        Idling = true;
        moveFreely = false;
        pullImg.color = new Color(pullImg.color.r, pullImg.color.g, pullImg.color.b, 0.2f);
        rb.velocity = Vector2.zero;
        rb.AddForce((playerChar.position - transform.position) * pullSpeed, ForceMode2D.Force);
        playerChar.GetComponent<Player>().rb.AddForce((transform.position - playerChar.position) * pullSpeed, ForceMode2D.Force);
        pullPos = playerChar.position;
    }
    public void SlashInDirection(Vector2 dir)
    {
        StartCoroutine(SlashDir(dir));
    }

    public void RepellFunction()
    {
        StartCoroutine(Repell());
    }
    public void Deflected()
    {
        StartCoroutine(OnDeflected());
    }
    IEnumerator OnDeflected()
    {
        GameManager.Instance.gameObject.GetComponent<AudioManager>().SwordDeflect.Post(gameObject);
        moveFreely = false;
        yield return new WaitForSeconds(0.2f);
        rb.velocity = Vector2.zero;
        yield return new WaitForSeconds(1);
        moveFreely = true;
        yield return null;
    }

    IEnumerator Repell()
    {
        repel.Play();
        repellImg.color = new Color(repellImg.color.r, repellImg.color.g, repellImg.color.b, 0.2f);
        moveFreely = false;
        rb.velocity = Vector2.zero;
        rb.AddForce((transform.position - playerChar.position).normalized * repellForce, ForceMode2D.Force);
        rb.AddTorque(100, ForceMode2D.Impulse);
        isSlashing = true;
        slashes.Play();
        yield return new WaitForSeconds(repellDuration);
        rb.AddTorque(-100, ForceMode2D.Impulse);
        isSlashing = false;
        slashes.Stop();
        rb.velocity /= 10;
        yield return new WaitForSeconds(callbackTime);
        moveFreely=true;
        yield return null;
    }
}
