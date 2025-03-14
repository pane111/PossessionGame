using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursedShield : CursedWeapon
{
    public Transform rotator;
    public float rotSpeed;
    float curRot = 0;
    public ParticleSystem fire;
    public float travelSpeed;
    public float playerDist;

    void Update()
    {
        FindPlayer();
        rotator.up = Vector2.Lerp(rotator.up, -(player.transform.position - transform.position), rotSpeed * Time.deltaTime);
        if (curRot >= 360 || curRot <= -360)
        {
            curRot = 0;
        }
        if (curHealth <= 0)
        {
            rb.MovePosition(Vector2.Lerp(transform.position, demonHeart.transform.position, Time.deltaTime * travelSpeed));
        }
        else if (playerFound)
        {
            Vector2 dir = player.position - transform.position;
            Vector2 targetPos = (Vector2)transform.position + dir - ((dir).normalized * playerDist);
            rb.MovePosition(Vector2.Lerp(transform.position, targetPos, Time.deltaTime * travelSpeed));
        }
        lr.SetPosition(1, demonHeart.transform.position - transform.position);
    }
    public override void OnDeath()
    {
        base.OnDeath();
        fire.Play();
        fire.transform.parent = null;
        Destroy(fire, 4);
        GetComponent<Collider2D>().enabled = false;
        rb.isKinematic = true;
        gameObject.SetActive(false);

    }
}
