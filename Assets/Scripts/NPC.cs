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

    bool scared = false;
    Transform player;

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
        player = GameManager.Instance.player.transform;
        GameManager.Instance.startDM += EnterDM;
        GameManager.Instance.stopDM += ExitDM;

        StartCoroutine(MoveRandom());
    }

    private void Update()
    {
        if (!dead && scared && !GameManager.Instance.player.demonModeActive) {
            rb.velocity = (transform.position - player.position).normalized * speed * 2;
            GetComponent<Animator>().enabled = true;
        }
    }

    IEnumerator MoveRandom()
    {
        if (!scared)
        {
            float r1 = Random.Range(1.5f, 3f);
            float r2 = Random.Range(1, 2);

            yield return new WaitForSeconds(r1);
            Vector3 dest = Vector2.zero;
            dest.x = Random.Range(transform.position.x - 5, transform.position.x + 5);
            dest.y = Random.Range(transform.position.y - 5, transform.position.y + 5);
            rb.velocity = (dest - transform.position).normalized * speed * mod;
            GetComponent<Animator>().enabled = true;
            yield return new WaitForSeconds(r2);
            GetComponent<Animator>().enabled = false;
            rb.velocity = Vector2.zero;
            if (!dead) StartCoroutine(MoveRandom());
        }
        
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

    

    public void FootstepSound()
    {
        AudioManager.Instance.NPC_Footstep.Post(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.name == "PlayerSword" && !dm)
        {
            if (sword.curTarget == this.gameObject.transform) { sword.attacksCount++; }
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
        scared = true;
        GetComponent<Animator>().SetFloat("Speed", 2);
        curHealth -= amount;
        if (curHealth <= 0)
        {
            if (!GameManager.Instance.npctutorial)
            {
                GameManager.Instance.npctutorial = true;
                GameManager.Instance.PopupTutorial("Your sword has just killed an innocent person! Be careful, as attacking and killing innocents will heavily corrupt you!", sr.sprite);
            }
            if (GameManager.Instance.player.npckills ==3)
            {
                GameManager.Instance.SendNotification("The path of blood will lead to ruin. You must cease immediately.");
            }
            GameManager.Instance.gameObject.GetComponent<AudioManager>().NPCDeath.Post(gameObject);
            GetComponent<Animator>().enabled = false;
            ParticleSystem.MainModule sma = bloodSpray.main;
            sma.startColor = GameManager.Instance.randomColor();
            if (!dead) { bloodSpray.Play(); GameManager.Instance.AddKill();  }

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
        else
        {
            GameManager.Instance.player.Corruption += 1;
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
