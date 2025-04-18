using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CursedShield : CursedWeapon
{
    public Transform rotator;
    public float rotSpeed;
    float curRot = 0;
    public ParticleSystem fire;
    public float travelSpeed;
    public float playerDist;
    public GameObject creature;

    public override void OnStart()
    {
        base.OnStart();
        SpawnCreature();
    }

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

    void SpawnCreature()
    {
        
        print("Trying to spawn creature");
        if (playerFound && curHealth > 0)
        {
            print("Creature spawned");
            Instantiate(creature,transform.position,Quaternion.identity);
        }

        Invoke("SpawnCreature", 10);
    }

    public override void OnDeath()
    {
        dead = true;
        GameManager.Instance.player.OnPurify();
        fire.Play();
        fire.transform.parent = null;
        Destroy(fire, 4);
        GetComponent<Collider2D>().enabled = false;
        rb.isKinematic = true;
        gameObject.SetActive(false);
        rotator.gameObject.SetActive(false);
        print("Reached end of death sequence");

    }
}
