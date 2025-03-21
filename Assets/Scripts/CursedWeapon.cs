using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursedWeapon : MonoBehaviour
{
    public float maxHealth;
    public float curHealth;
    public GameObject demonHeart;
    public Transform player;
    private SwordScript sword;

    public float detectRange;
    public bool playerFound;
    public ParticleSystem damageEffect;
    bool dead = false;
    public LineRenderer lr;
    bool canTakeDamage;
    public bool playerContact = false;
    public SpriteRenderer sr;
    public GameObject cBreakEffect; 
    [HideInInspector] public Rigidbody2D rb;
    protected virtual void Start()
    {
        GetComponent<Deflector>().deflectFrom = transform;

        GetComponent<Deflector>().repelForce = 100;
        demonHeart.GetComponent<DemonHeart>().weapon = this;
        player = GameObject.Find("Player").transform;
        GameManager.Instance.startDM += this.OnDM;
        GameManager.Instance.stopDM += this.OnExitDM;
        lr.enabled = true;
        sword = FindObjectOfType<SwordScript>();
        rb = GetComponent<Rigidbody2D>();
    }

    public virtual void TakeDamage(float amount)
    {
        if (canTakeDamage)
        {
            AudioManager.Instance.SwordSlash.Post(gameObject);
            damageEffect.Play();
            curHealth -= amount;
            if (curHealth <= 0)
            {
                CrushCrystal();
            }
            else
            {
                demonHeart.GetComponent<DemonHeart>().crystalHit.Play();
                lr.SetPosition(0, Vector3.zero);
                lr.SetPosition(1, demonHeart.transform.position - transform.position);
            }
        }
        
    }

    public void CrushCrystal()
    {
        if (!GameManager.Instance.crystalTutorial)
        {
            GameManager.Instance.SendNotification("You have shattered the barrier protecting the demon's heart! Find the heart and attack it!");
            GameManager.Instance.crystalTutorial = true;
        }
        cBreakEffect.SetActive(true);
        demonHeart.GetComponent<DemonHeart>().ExposeHeart();
    }

    public virtual void OnDM()
    {
        if (!dead)
        {
            gameObject.SetActive(true);
            canTakeDamage = true;
            GetComponent<Deflector>().deflectionActive = false;
            sr.color = Color.white;
        }
            
    }
    public virtual void OnExitDM()
    {
        
        if (Vector2.Distance(player.position, transform.position) >= detectRange) { //playerContact = false;
                                                                                   }

        if (playerContact && !dead)
        {
            GetComponent<Deflector>().deflectionActive = true;
            sr.color = new Color(1, 1, 1, 0.3f);
            canTakeDamage = false;
        }
        else
        {
            if (!dead) gameObject.SetActive(false);
        }
    }

    public virtual void FindPlayer()
    {
        float pDist = (player.position - transform.position).magnitude;
        if (pDist <= detectRange) { playerFound = true; }
    }

    public virtual void OnDeath()
    {
        dead = true;
        lr.gameObject.SetActive(false);
        GameManager.Instance.player.OnPurify();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<SwordScript>() != null)
        {
            playerContact = true;
            playerFound = true;
            SwordScript sword = collision.GetComponent<SwordScript>();
            if (sword.curTarget == this.gameObject.transform) { sword.attacksCount++; }
            Vector2 dir = transform.position - collision.transform.position;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            Quaternion lDir = Quaternion.AngleAxis(angle, Vector3.forward);
            GameObject bs = Instantiate(GameManager.Instance.bSplatter, transform.position, lDir);

            bs.transform.position = (Vector2)transform.position + dir.normalized;
            if (collision.GetComponent<SwordScript>().isSlashing)
            {
                TakeDamage(3 + GameManager.Instance.dmgUpgrades* GameManager.Instance.dmgIncrease*3);
            }
            else
            {
                TakeDamage(1 + +GameManager.Instance.dmgUpgrades * GameManager.Instance.dmgIncrease);
                if (sword.curTarget == sword.playerChar || sword.curTarget == sword.playerChar.gameObject.GetComponent<Player>().orbiter) { sword.curTarget = this.gameObject.transform; sword.Idling = false; sword.SIforNPC(); }
            }
        }
    }
}
