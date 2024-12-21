using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using static Unity.Burst.Intrinsics.Arm;

public class NPC : MonoBehaviour
{
    private SpriteRenderer sr;
    public Rigidbody2D rb;
    SwordScript sword;
    [Header("Base Values")]
    public float maxHealth;
    [SerializeField] private float curHealth;
    bool dead = false;
    public float speed;
    private int mod = 1;
    [Header("Visuals")]
    public float flipTime;
    Sprite initSprite;
    Color initColor;
    public Sprite demonSprite;
    public Sprite corpseSprite;
    public GameObject hitEffect;
    public ParticleSystem blood;
    public ParticleSystem bloodSpray;
    public SpriteRenderer bloodstain;


    bool dm = false;
    private void Start()
    {
        curHealth = maxHealth;
        rb = GetComponent<Rigidbody2D>();
        sword = FindObjectOfType<SwordScript>();
        sr = GetComponent<SpriteRenderer>();
        sr.sprite = GameManager.Instance.randomSprite();
        initSprite = sr.sprite;
        sr.color = GameManager.Instance.randomNpcColor();
        initColor = sr.color;
        bloodstain.color = initColor;
        bloodstain.enabled = false;

        GameManager.Instance.startDM += EnterDM;
        GameManager.Instance.stopDM += ExitDM;

        StartCoroutine(MoveRandom());
    }

    IEnumerator MoveRandom()
    {
        yield return new WaitForSeconds(Random.Range(1.5f, 3f));
        Vector3 dest = Vector2.zero;
        dest.x = Random.Range(transform.position.x - 5, transform.position.x + 5);
        dest.y = Random.Range(transform.position.y - 5, transform.position.y + 5);
        rb.velocity = (dest - transform.position).normalized * speed * mod;
        yield return new WaitForSeconds(Random.Range(1, 2));
        rb.velocity = Vector2.zero;
        if(!dead) StartCoroutine (MoveRandom());
    }

    public void EnterDM()
    {
        mod = 0;
        rb.velocity = Vector2.zero;
        dm = true;
        gameObject.layer = 0;
        if (curHealth > 0 && gameObject.activeInHierarchy)
        {
            GetComponent<SpriteRenderer>().sprite = demonSprite;
            GetComponent<SpriteRenderer>().color = Color.white;
            rb.velocity = Vector2.zero;
            rb.isKinematic = true;

            StartCoroutine(flipSprite());
        }
    }
    public void ExitDM()
    {
        dm = false;
        if (curHealth > 0)
        {
            gameObject.layer = 6;
            GetComponent<SpriteRenderer>().sprite = initSprite;
            GetComponent<SpriteRenderer>().color = initColor;
            rb.isKinematic = false;
            GetComponent<Collider2D>().enabled = true;
            mod = 1;
        }
    }

    IEnumerator flipSprite()
    {
        if (dm && !dead)
        {
            GetComponent<SpriteRenderer>().flipX = !GetComponent<SpriteRenderer>().flipX;
            yield return new WaitForSeconds(flipTime);
            StartCoroutine(flipSprite());
        }
        else
        {
            if (!dead)
            {
                GetComponent<SpriteRenderer>().flipX = !GetComponent<SpriteRenderer>().flipX;
                GameManager.Instance.GetComponent<AudioManager>().NPC_Footstep.Post(gameObject);
                yield return new WaitForSeconds(flipTime / 2);
                StartCoroutine(flipSprite());
            }
        }

        yield return null;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.name == "PlayerSword" && !dm)
        {
            if (other.gameObject.GetComponent<SwordScript>().isSlashing)
            {
                StartCoroutine(TakeDamage(3));
            }
            else
            {
                StartCoroutine(TakeDamage(1));
                if (curHealth > 0) { 
                    if (sword.curTarget == sword.playerChar || sword.curTarget == sword.playerChar.gameObject.GetComponent<Player>().orbiter) { sword.curTarget = this.gameObject.transform; sword.Idling = false; sword.SIforNPC(); }
                }
            }
            Vector2 dir = transform.position - other.transform.position;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            Quaternion lDir = Quaternion.AngleAxis(angle, Vector3.forward);
            GameObject bs = Instantiate(GameManager.Instance.bSplatter, transform.position, lDir);
            bs.transform.position = (Vector2)transform.position + dir.normalized;
        }
    }

    IEnumerator TakeDamage(float amount)
    {
        AudioManager.Instance.NPCTakeDmg.Post(gameObject);
        curHealth -= amount;
        if (curHealth <= 0)
        {
            GameManager.Instance.gameObject.GetComponent<AudioManager>().NPCDeath.Post(gameObject);
            ParticleSystem.MainModule sma = bloodSpray.main;
            sma.startColor = GameManager.Instance.randomColor();
            if (!dead) { bloodSpray.Play(); GameManager.Instance.AddKill(); }

            dead = true;
            mod = 0;
            GetComponent<Collider2D>().isTrigger = true;
            GetComponent<SpriteRenderer>().sprite = corpseSprite;
            sword.OnEnemyDeath();
            bloodstain.enabled = true;
            rb.velocity = Vector3.zero;
            rb.isKinematic = true;
            FindObjectOfType<Player>().StopBleeding();
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
