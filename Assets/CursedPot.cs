using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursedPot : CursedWeapon
{
    public GameObject orb;
    public Animator anim;
    public Transform shotPoint;
    public float shotForce;
    public float maxShotCd;
    public float shockDist;
    float shotCd;
    float pDist;
    void Start()
    {
        player = GameObject.Find("Player").transform;
        shotCd =maxShotCd;
        GameManager.Instance.startDM += this.OnDM;
        GameManager.Instance.stopDM += this.OnExitDM;
    }

    // Update is called once per frame
    void Update()
    {
        
        if (playerFound)
        {
            shotCd -= Time.deltaTime;
            if (shotCd <=0)
            {
                shotCd = 9999;
                anim.SetTrigger("Attack");
            }

            pDist = (player.transform.position - transform.position).magnitude;
            if (pDist<=shockDist)
            {
                anim.SetTrigger("Shock");
            }
        }
        else
        {
            FindPlayer();
        }
    }
    public override void FindPlayer()
    {
        base.FindPlayer();
    }
    public void AttemptPushback()
    {
        if(pDist<=shockDist)
        {
            StartCoroutine(PushPlayerBack());
        }
    }
    IEnumerator PushPlayerBack()
    {
        Player p = player.gameObject.GetComponent<Player>();
        p.enabled = false;
        p.GetComponent<Rigidbody2D>().velocity = (player.transform.position - transform.position).normalized * 50;
        p.TakeCustomDamage(10);
        yield return new WaitForSeconds(0.4f);
        p.enabled = true;
        anim.ResetTrigger("Shock");
        yield return null;
    }
    public void DestroyPot()
    {
        gameObject.SetActive(false);
    }
    public override void OnDeath()
    {
        base.OnDeath();
        anim.SetTrigger("Death");
    }

    public void ShootOrb()
    {
        shotCd = maxShotCd;
        GameObject o = Instantiate(orb,shotPoint.position,Quaternion.identity);
        Vector2 dir = player.position - o.transform.position;
        o.GetComponent<Rigidbody2D>().velocity = dir.normalized*shotForce;
        Destroy(o, 10);
    }
}
