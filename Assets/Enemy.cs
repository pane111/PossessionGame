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
    public GameObject crystal;
    public GameObject heart;
    public bool heartExposed;
    public ParticleSystem crystalHit;
    Deflector d;
    public CursedWeapon weapon;
    void Start()
    {
        d = GetComponent<Deflector>();
        d.deflectionActive = false;
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
                    if (!playerFound)
                    {
                        playerFound = true;
                        StartCoroutine(flipSprite());
                    }
                    
                    
                }
                else
                {
                    playerFound = false;
                    
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
    public void ExposeHeart()
    {
        d.deflectionActive = false;
        heartExposed = true;
        crystal.SetActive(false);
        heart.SetActive(true);
    }

    void EnterDM()
    {
        if (curHealth > 0 && gameObject.activeInHierarchy)
        {
            gameObject.layer = 0;
            //GetComponent<Deflector>().enabled = true; <-- Enable/Disable does not work on scripts that do not implement monobehaviour methods (start, update)

            demon = true;
            crystal.SetActive(!heartExposed);
            heart.SetActive(heartExposed);
            GetComponent<SpriteRenderer>().sprite = demonSprite;
            GetComponent<SpriteRenderer>().color = Color.white;
            rb.velocity= Vector2.zero;
            rb.isKinematic = true;

            d.deflectionActive = !heartExposed;

            StartCoroutine(flipSprite());

            
        }
        
    }

    void ExitDM()
    {
        d.deflectionActive = false;
        if (curHealth > 0)
        {
            gameObject.layer = 6;
            playerFound = false;
            demon = false;
            crystal.SetActive(false);
            heart.SetActive(false);
            GetComponent<SpriteRenderer>().sprite = initSprite;
            GetComponent<SpriteRenderer>().color = initColor;
            rb.isKinematic = false;
            GetComponent<Collider2D>().enabled = true;
        }
    }

    IEnumerator flipSprite()
    {
        if (demon)
        {
            GetComponent<SpriteRenderer>().flipX = !GetComponent<SpriteRenderer>().flipX;
            yield return new WaitForSeconds(flipTime);
            StartCoroutine(flipSprite());
        }
        else
        {
            if (playerFound)
            {
                GetComponent<SpriteRenderer>().flipX = !GetComponent<SpriteRenderer>().flipX;
                GameManager.Instance.GetComponent<AudioManager>().NPC_Footstep.Post(gameObject);
                yield return new WaitForSeconds(flipTime);
                StartCoroutine(flipSprite());
            }
        }
        
        yield return null;
    }
    

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.name == "PlayerSword" && !purified)
        {
            if(demon && !heartExposed)
            {
                AudioManager.Instance.CrystalHit.Post(gameObject);
            }
            else
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
                GameObject bs = Instantiate(GameManager.Instance.bSplatter, transform.position, lDir);

                if (heartExposed && demon)
                {
                    AudioManager.Instance.NPCHeartHit.Post(gameObject);
                    StartCoroutine(TakeDamage(0));
                    heart.GetComponent<Collider2D>().enabled = false;
                    heart.SetActive(false);
                    player.GetComponent<Player>().OnPurify();
                    Purify();
                }
                bs.transform.position = (Vector2)transform.position + dir.normalized;
            }
        }
    }
    public void Purify()
    {
        if (curHealth > 0)
        {
            weapon.OnDeath();
            purified = true;
            gameObject.layer = 0;
            sparkles.Play();
            rb.isKinematic = true;
            GetComponent<Collider2D>().isTrigger = true;
        }
        
    }
    public void Rescue()
    {
        AudioManager.Instance.Purify.Post(gameObject);
        GetComponent<SpriteRenderer>().enabled = false;
        lightBeam.SetActive(true);
        Invoke("DisableThis", 1);
    }

    void DisableThis()
    {
        gameObject.SetActive(false);
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
