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

    
    void Update()
    {
        FindPlayer();
        rotator.rotation = Quaternion.Euler(0, 0, curRot += Time.deltaTime * rotSpeed);
        if (curRot >= 360 || curRot <= -360)
        {
            curRot = 0;
        }
        if (curHealth <= 0)
        {
            GetComponent<Rigidbody2D>().MovePosition(Vector2.Lerp(transform.position,demonHeart.transform.position,Time.deltaTime*travelSpeed));
            lr.SetPosition(1, demonHeart.transform.position - transform.position);
        }
    }
    public override void OnDeath()
    {
        base.OnDeath();
        fire.Play();
        fire.transform.parent = null;
        Destroy(fire, 4);
        GetComponent<Collider2D>().enabled = false;
        GetComponent<Rigidbody2D>().isKinematic = true;
        gameObject.SetActive(false);

    }
}
