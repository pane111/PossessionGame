using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursedShield : CursedWeapon
{
    public Transform rotator;
    public float rotSpeed;
    float curRot = 0;
    public ParticleSystem fire;
   

    
    void Update()
    {
        FindPlayer();
        rotator.rotation = Quaternion.Euler(0, 0, curRot += Time.deltaTime * rotSpeed);
        if (curRot >= 360 || curRot <= -360)
        {
            curRot = 0;
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
