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
    bool dead = false;
    public Transform player;
    SwordScript sword;
    public SpriteRenderer bloodstain;
    void Start()
    {
        
        sword = FindObjectOfType<SwordScript>();
        player = GameObject.Find("Player").transform;
        Color c = GameManager.Instance.randomColor();
        GetComponent<SpriteRenderer>().color = c;
        bloodstain.color = c;
        bloodstain.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!dead)
        {
            Vector2 pDir = player.position - transform.position;
            pDir.Normalize();
            RaycastHit2D hit;
            hit = Physics2D.Raycast(transform.position, pDir, detectRange);
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
    

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.name == "PlayerSword")
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

    IEnumerator TakeDamage(float amount)
    {
        curHealth -= amount;
        if (curHealth <= 0)
        {
            ParticleSystem.MainModule sma = bloodSpray.main;
            sma.startColor = GameManager.Instance.randomColor();
            if (!dead)
                bloodSpray.Play();
            GetComponent<Animator>().enabled = false;
            dead = true;
            GetComponent<Collider2D>().isTrigger = true;
            GetComponent<SpriteRenderer>().sprite = corpseSprite;
            sword.OnEnemyDeath();
            bloodstain.enabled = true;
            rb.velocity = Vector3.zero;
            rb.isKinematic = true;
            
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
