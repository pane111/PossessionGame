using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float maxHealth = 100;
    public float curHealth = 100;
    public float speed;
    public float detectRange;
    public bool playerFound;
    public Rigidbody2D rb;
    public Sprite corpseSprite;
    public GameObject hitEffect;
    public ParticleSystem blood;
    public ParticleSystem bloodSpray;
    public ParticleSystem sparkles;
    bool dead = false;
    public Transform player;
    SwordScript sword;
    public SpriteRenderer bloodstain;
    public LayerMask lm;
    public GameObject lightBeam;
    public bool purified;
    bool demon;
    public Sprite demonSprite;
    Sprite initSprite;
    Color initColor;
    public float flipTime;
    void Start()
    {
        initSprite = GetComponent<SpriteRenderer>().sprite;
        sword = FindObjectOfType<SwordScript>();
        player = GameObject.Find("Player").transform;
        Color c = GameManager.Instance.randomColor();
        GetComponent<SpriteRenderer>().color = c;
        initColor = c;
        bloodstain.color = c;
        bloodstain.enabled = false;

        GameManager.Instance.startDM += EnterDM;
        GameManager.Instance.stopDM += ExitDM;
    }

    // Update is called once per frame
    void Update()
    {
        if (!dead && !purified && !demon)
        {
            Vector2 pDir = player.position - transform.position;
            pDir.Normalize();
            RaycastHit2D hit;
            hit = Physics2D.Raycast(transform.position, pDir, detectRange,lm);
            if (hit)
            {
                if (hit.transform == player)
                {
                    playerFound = true;
                    GetComponent<Animator>().enabled = true;
                }
                else
                {
                    playerFound = false;
                    GetComponent<Animator>().enabled = false;
                }
            }

            if (playerFound)
            {
                rb.velocity = pDir * speed;
            }
            else
            {
                rb.velocity = Vector2.zero;
            }
        }
        
    }

    void EnterDM()
    {
        demon = true;
        GetComponent<SpriteRenderer>().sprite = demonSprite;
        GetComponent<SpriteRenderer>().color = Color.white;
        GetComponent<Animator>().enabled = false;
        rb.isKinematic = true;
        GetComponent<Collider2D>().isTrigger = true;
        StartCoroutine(flipSprite());
    }

    void ExitDM()
    {
        demon = false;
        GetComponent<SpriteRenderer>().sprite = initSprite;
        GetComponent<SpriteRenderer>().color = initColor;
        GetComponent<Animator>().enabled = true;
        rb.isKinematic = false;
        GetComponent<Collider2D>().isTrigger = false;
    }

    IEnumerator flipSprite()
    {
        if (demon)
        {
            GetComponent<SpriteRenderer>().flipX = !GetComponent<SpriteRenderer>().flipX;
            yield return new WaitForSeconds(flipTime);
            StartCoroutine(flipSprite());
        }
        
        yield return null;
    }
    

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.name == "PlayerSword" && !purified)
        {
            if (other.gameObject.GetComponent<SwordScript>().isSlashing)
            {
                StartCoroutine(TakeDamage(3));
            }
            else
            {
                StartCoroutine(TakeDamage(1));
            }
            Vector2 dir = transform.position - other.transform.position;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            Quaternion lDir = Quaternion.AngleAxis(angle, Vector3.forward);
            GameObject bs = Instantiate(GameManager.Instance.bSplatter,transform.position, lDir);
            

            bs.transform.position = (Vector2)transform.position + dir.normalized;
        }
    }
    public void Purify()
    {
        purified = true;
        gameObject.layer = 0;
        sparkles.Play();
        rb.isKinematic = true;
        GetComponent<Collider2D>().isTrigger = true;
    }
    public void Rescue()
    {
        GetComponent<SpriteRenderer>().enabled = false;
        lightBeam.SetActive(true);
        Destroy(gameObject,1);
    }

    IEnumerator TakeDamage(float amount)
    {
        curHealth -= amount;
        if (curHealth <= 0)
        {
            ParticleSystem.MainModule sma = bloodSpray.main;
            sma.startColor = GameManager.Instance.randomColor();
            if (!dead) { bloodSpray.Play(); GameManager.Instance.AddKill(); }
                
            GetComponent<Animator>().enabled = false;
            dead = true;
            GetComponent<Collider2D>().isTrigger = true;
            GetComponent<SpriteRenderer>().sprite = corpseSprite;
            sword.OnEnemyDeath();
            bloodstain.enabled = true;
            rb.velocity = Vector3.zero;
            rb.isKinematic = true;
            player.GetComponent<Player>().StopBleeding();
            gameObject.layer = 0;
        }
        ParticleSystem.MainModule ma = blood.main;
        ma.startColor = GameManager.Instance.randomColor();
        blood.Play();
        hitEffect.SetActive(true);
        yield return new WaitForSeconds(0.05f);
        hitEffect.SetActive(false);
        yield return null;
    }
}
