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
        if (GameManager.Instance.expertMode)
        {
            rotSpeed *= 1.4f;
        }
    }

    void Update()
    {
        FindPlayer();
        
        if (curRot >= 360 || curRot <= -360)
        {
            curRot = 0;
        }
        if (curHealth <= 0)
        {
            rb.MovePosition(Vector2.Lerp(transform.position, demonHeart.transform.position, Time.deltaTime * 2));
            rotator.rotation *= Quaternion.Euler(0,0,rotSpeed * Time.deltaTime * 360);
        }
        else if (playerFound)
        {
            Vector2 dir = player.position - transform.position;
            Vector2 targetPos = (Vector2)transform.position + dir - ((dir).normalized * playerDist);
            rb.MovePosition(Vector2.Lerp(transform.position, targetPos, Time.deltaTime * travelSpeed));
            rotator.up = Vector2.Lerp(rotator.up, -(player.transform.position - transform.position), rotSpeed * Time.deltaTime);
        }
        lr.SetPosition(1, demonHeart.transform.position - transform.position);
    }

    void SpawnCreature()
    {
        
        print("Trying to spawn creature");
        if (playerFound && !dead)
        {
            if (GameManager.Instance.expertMode)
            {
                GetComponent<SpreadShot>().OnShoot();
            }
            print("Creature spawned");
            Instantiate(creature,transform.position,Quaternion.identity);
        }

        Invoke("SpawnCreature", 14);
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
