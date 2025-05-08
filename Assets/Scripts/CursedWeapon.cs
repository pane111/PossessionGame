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
    public bool dead = false;
    public LineRenderer lr;
    bool canTakeDamage;
    public bool playerContact = false;
    public SpriteRenderer sr;
    bool hasTriggeredTut = false;
    public GameObject cBreakEffect;
    public Sprite heartSprite;
    public Sprite altSprite;
    Sprite normalSprite;
    [HideInInspector] public Rigidbody2D rb;
    protected virtual void Start()
    {
        normalSprite = sr.sprite;
        if (GameManager.Instance.expertMode)
        {
            maxHealth *= 1.25f;
            curHealth = maxHealth;
        }
        GetComponent<Deflector>().deflectFrom = transform;

        GetComponent<Deflector>().repelForce = 100;
        demonHeart.GetComponent<DemonHeart>().weapon = this;
        player = GameObject.Find("Player").transform;
        GameManager.Instance.startDM += this.OnDM;
        GameManager.Instance.stopDM += this.OnExitDM;
        lr.enabled = true;
        sword = FindObjectOfType<SwordScript>();
        rb = GetComponent<Rigidbody2D>();
        OnStart();
        
    }
    public virtual void OnStart()
    {
        OnExitDM();
    }

    public virtual void TakeDamage(float amount)
    {
        if (canTakeDamage)
        {
            
            
            curHealth -= amount;
            if (curHealth <= 0)
            {
                CrushCrystal();
                AudioManager.Instance.SwordDeflect.Post(gameObject);
                StartCoroutine(LineColor());

            }
            else
            {
                AudioManager.Instance.SwordSlash.Post(gameObject);
                damageEffect.Play();
                demonHeart.GetComponent<DemonHeart>().crystalHit.Play();
                lr.SetPosition(0, Vector3.zero);
                lr.SetPosition(1, demonHeart.transform.position - transform.position);
            }
        }
        
    }
    IEnumerator LineColor()
    {
        lr.material.color = Color.black;
        yield return new WaitForSeconds(0.2f);
        lr.material.color = Color.white;
        yield return null;
    }

    public void CrushCrystal()
    {
        if (!GameManager.Instance.crystalTutorial && !hasTriggeredTut)
        {
            if (GameManager.Instance.cTutorialCount== 3)
            {
                Invoke("Popup", 1.75f);
            }
            else
            {
                GameManager.Instance.SendNotification("You have shattered the barrier protecting the demon's heart! Follow the tether and attack the heart!");
            }
            
            GameManager.Instance.cTutorialCount--;
            hasTriggeredTut = true;
            if (GameManager.Instance.cTutorialCount <= 0)
            {
                GameManager.Instance.crystalTutorial = true;
            }
            
        }
        sr.color = new Color(1, 1, 1, 0.3f);
        cBreakEffect.SetActive(true);
        demonHeart.GetComponent<DemonHeart>().ExposeHeart();
    }
    void Popup()
    {
        GameManager.Instance.PopupTutorial("You have shattered the barrier protecting the demon's heart! Follow the <color=red>tether</color> and attack the heart!", heartSprite);
    }

    public virtual void OnDM()
    {
        if (!dead)
        {
            gameObject.SetActive(true);
            canTakeDamage = true;
            GetComponent<Deflector>().deflectionActive = false;
            sr.sprite = normalSprite;
        }
            
    }
    public virtual void OnExitDM()
    {
        
        if (Vector2.Distance(player.position, transform.position) >= detectRange) { //playerContact = false;
                                                                                   }

        if (playerContact && !dead)
        {
            GetComponent<Deflector>().deflectionActive = true;
            sr.sprite = altSprite;
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
        if (lr!=null)
        {
            lr.gameObject.SetActive(false);
        }
        
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
