using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CursedStaff : CursedWeapon
{
    bool beamStarted;
    float beamCooldown = 8;

    public Transform beamStart;
    public LineRenderer beam;
    public Transform beamEnd;
    public float followSpeed;
    public float beamDuration;
    public SpreadShot spreadShot;
    public float fireDelay;
    void Update()
    {
        FindPlayer();
        beamCooldown -= Time.deltaTime;
        if (beamCooldown < 0 && playerFound)
        {
            beamCooldown = 9999;
            StartCoroutine(beamAttack());
        }


        if (beamStarted)
        {
            beamEnd.position = Vector3.MoveTowards(beamEnd.position, player.position, followSpeed * Time.deltaTime);
            Vector3 sPos = new Vector3(beamStart.localPosition.x, beamStart.localPosition.y, 0);
            Vector3 ePos = new Vector3(beamEnd.localPosition.x, beamEnd.localPosition.y, 0);
            beam.SetPosition(0, sPos);
            beam.SetPosition(1, ePos);
            var dir = beamEnd.position - beamStart.position;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            beamStart.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            beamEnd.rotation = beamStart.rotation;

            if ((player.position-beamEnd.position).magnitude <= 1)
            {
                player.GetComponent<Player>().TakeDamage();
            }
        }

    }

    public override void FindPlayer()
    {

        float pDist = (player.position - transform.position).magnitude;
        if (pDist <= detectRange) {  
            
            if (!playerFound)
            {
                GetComponent<Animator>().SetTrigger("Awake");
                beamCooldown = 2;
                Invoke("ShootBullets", fireDelay);
            }
            
            playerFound = true; }
    }
    public override void OnDeath()
    {
        base.OnDeath();
        GetComponent<Collider2D>().enabled = false;
        beamCooldown = 9999;
        beamStart.gameObject.SetActive(false);
        beamEnd.gameObject.SetActive(false);
        beam.enabled = false;
        beamStarted = false;
        GetComponent<Animator>().SetTrigger("Death");

    }
    public void DestroyStaff()
    {
        Destroy(gameObject);
    }
    IEnumerator beamAttack()
    {
        beamStart.gameObject.SetActive(true);
        beamEnd.gameObject.SetActive(true);
        beam.enabled = true;
        beamStarted = true;
        yield return new WaitForSeconds(beamDuration);
        beamStarted = false;
        beamCooldown = 8;
        beamStart.gameObject.SetActive(false);
        beamEnd.gameObject.SetActive(false);
        beam.enabled = false;
        yield return null;
    }

    void ShootBullets()
    {
        spreadShot.OnShoot();
        Invoke("ShootBullets", fireDelay);
    }
}
