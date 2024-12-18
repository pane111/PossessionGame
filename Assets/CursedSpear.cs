using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursedSpear : CursedWeapon
{
    public float attackForce;
    public float attackTime;
    public float attackCooldown;
    float ccd=0;
    public float attackWindup;
    public float moveSpeed;
    bool attacking;
    Vector2 pDir;
    public ParticleSystem fire;

    // Update is called once per frame
    void Update()
    {

        if (playerFound)
        {
            lr.transform.parent = null;
            lr.transform.position = lr.transform.position - Vector3.forward * 6;
            lr.SetPosition(0, transform.position);
            lr.SetPosition(1, enemy.transform.position);
            pDir = player.transform.position - transform.position;
            float pDist = pDir.magnitude;
            if (!attacking)
            {
                GetComponent<Rigidbody2D>().MovePosition(Vector3.Lerp(transform.position, player.GetComponent<Player>().orbiter.position, Time.deltaTime * moveSpeed));
                GetComponent<Rigidbody2D>().AddTorque(pDir.magnitude);
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
        transform.rotation = Quaternion.Euler(0, 0, angle);
        rb.velocity = -pDir.normalized * moveSpeed;
        Vector2 curDir = pDir;
        yield return new WaitForSeconds(attackWindup);
        rb.velocity = Vector2.zero;
        yield return new WaitForSeconds(0.1f);
        //angle = Mathf.Atan2(player.position.y - transform.position.y, player.position.x - transform.position.x) * Mathf.Rad2Deg - 90;
        //transform.rotation = Quaternion.Euler(0, 0, angle);
        rb.velocity = curDir.normalized * attackForce;
        yield return new WaitForSeconds(attackTime);
        rb.velocity = Vector2.zero;
        yield return new WaitForSeconds(1);
        attacking = false;
        ccd = attackCooldown;
        yield return null;
    }
}
