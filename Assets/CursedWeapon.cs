using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursedWeapon : MonoBehaviour
{
    public float maxHealth;
    public float curHealth;
    public GameObject enemy;
    public Transform player;

    public float detectRange;
    public bool playerFound;
    public ParticleSystem damageEffect;
    bool dead;
    public LineRenderer lr;

    private void Start()
    {
        player = GameObject.Find("Player").transform;
        GameManager.Instance.startDM += this.OnDM;
        GameManager.Instance.stopDM += this.OnExitDM;
    }

    public virtual void TakeDamage(float amount)
    {
        AudioManager.Instance.SwordSlash.Post(gameObject);
        damageEffect.Play();
        curHealth -= amount;
        if (curHealth <= 0) {
            //OnDeath();
            FreeNPC();
        }
        else
        {
            StartCoroutine(CrystalVisual());
        }
    }

    public void FreeNPC() //Allows player to finish the NPC
    {
        enemy.GetComponent<Enemy>().ExposeHeart();
    }

    public void OnDM()
    {
        if (!dead)
        {
            if (enemy.GetComponent<Enemy>().curHealth <= 0)
            {
                lr.gameObject.SetActive(false);
            }
            gameObject.SetActive(true);
        }
            
    }
    public void OnExitDM()
    {
        if (!dead)
            gameObject.SetActive(false);
        
        
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
        //enemy.GetComponent<Enemy>().Purify();
    }

    IEnumerator CrystalVisual()
    {
        enemy.GetComponent<Enemy>().crystalHit.Play();
        lr.enabled = true;
        lr.SetPosition(0, Vector3.zero);
        lr.SetPosition(1,enemy.transform.position-transform.position);
        
        yield return new WaitForSeconds(0.2f);
        lr.enabled = false;
    }

    // Update is called once per frame
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<SwordScript>() != null)
        {
            playerFound = true;
            Vector2 dir = transform.position - collision.transform.position;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            Quaternion lDir = Quaternion.AngleAxis(angle, Vector3.forward);
            GameObject bs = Instantiate(GameManager.Instance.bSplatter, transform.position, lDir);


            bs.transform.position = (Vector2)transform.position + dir.normalized;
            if (collision.GetComponent<SwordScript>().isSlashing)
            {
                TakeDamage(3);
            }
            else
            {
                TakeDamage(1);
            }
        }
    }
}
