using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursedSpear : CursedWeapon
{
    public float attackForce;
    public float attackTime;
    public float attackCooldown;
    float ccd=0;
    float rot = 0;
    public float attackWindup;
    public float moveSpeed;
    bool attacking;
    Vector2 pDir;
    public ParticleSystem fire;
    public Transform sprite;
    public GameObject indicator;
   
    // Update is called once per frame

    public override void OnDM()
    {
        base.OnDM();
        attacking = false;
        ccd = attackCooldown;
    }
    void Update()
    {
        FindPlayer();
        lr.SetPosition(1, demonHeart.transform.position - transform.position);
        if (playerFound)
        {
            pDir = player.transform.position - transform.position;
            float pDist = pDir.magnitude;
            if (!attacking)
            {
                rot += Mathf.Pow(3f,Time.deltaTime);
                GetComponent<Rigidbody2D>().MovePosition(Vector3.Lerp(transform.position, player.GetComponent<Player>().orbiter.position - (Vector3)pDir.normalized * 3.5f, Time.deltaTime * moveSpeed));
                //GetComponent<Rigidbody2D>().AddTorque(pDir.magnitude);
                sprite.rotation = Quaternion.Euler(0, 0, rot);
                if (ccd <= 0)
                {
                    ccd = 9999;
                    StartCoroutine(Attack());
                }
                ccd -= Time.deltaTime;
            }
        }
        
    }

    public override void OnDeath()
    {
        base.OnDeath();
        fire.Play();
        fire.transform.parent = null;
        Destroy(fire,4);
        
        Destroy(lr.gameObject);
        GetComponent<Collider2D>().enabled = false;
        GetComponent<Rigidbody2D>().isKinematic = true;
        gameObject.SetActive(false);

    }

    IEnumerator Attack()
    {
        attacking = true;
        transform.up = player.transform.position - transform.position;
        transform.rotation = Quaternion.Euler(0, 0, transform.rotation.z);

        float angle = Mathf.Atan2(player.position.y-transform.position.y,player.position.x-transform.position.x)*Mathf.Rad2Deg-90;

        
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        rb.angularVelocity = 0;
        sprite.rotation = Quaternion.Euler(0, 0, angle);
        rb.velocity = -pDir.normalized * moveSpeed;
        Vector2 curDir = pDir;

        yield return new WaitForSeconds(attackWindup);
        indicator.SetActive(true);
        rb.velocity = Vector2.zero;
        yield return new WaitForSeconds(0.6f);
        indicator.SetActive(false);
        //angle = Mathf.Atan2(player.position.y - transform.position.y, player.position.x - transform.position.x) * Mathf.Rad2Deg - 90;
        //transform.rotation = Quaternion.Euler(0, 0, angle);
        rb.velocity = curDir.normalized * attackForce;
        yield return new WaitForSeconds(attackTime);
        rb.velocity = Vector2.zero;
        if (GameManager.Instance.expertMode)
        {
            GetComponent<SpreadShot>().OnShoot();
        }
        yield return new WaitForSeconds(1);
        rot = 0;
        attacking = false;
        ccd = attackCooldown;
        yield return null;
    }
}
