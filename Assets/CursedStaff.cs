using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CursedStaff : CursedWeapon
{
    bool beamStarted;
    float beamCooldown = 4;
    public float maxBeamCd;
    public Transform beamStart;
    public LineRenderer beam;
    public Transform beamEnd;
    public float followSpeed;
    public float beamDuration;
    public SpreadShot spreadShot;
    public float fireDelay;
    public float moveRange;
    public float moveSpeed;
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
                player.GetComponent<Player>().TakeCustomDamage(0.25f);
            }
        }

        if (curHealth <= 0)
        {
            lr.enabled = true;
            lr.SetPosition(0, Vector3.zero);
            lr.SetPosition(1, enemy.transform.position - transform.position);
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
        spreadShot.enabled = false;
        GetComponent<Collider2D>().enabled = false;
        beamCooldown = 9999;
        beamStart.gameObject.SetActive(false);
        beamEnd.gameObject.SetActive(false);
        beam.enabled = false;
        beamStarted = false;
        GetComponent<Rigidbody2D>().isKinematic = true;
        GetComponent<Animator>().SetTrigger("Death");

    }
    public void DestroyStaff()
    {
        spreadShot.enabled = false;
        gameObject.SetActive(false);
    }
    IEnumerator beamAttack()
    {
        Move();
        beamStart.gameObject.SetActive(true);
        beamEnd.gameObject.SetActive(true);
        beam.enabled = true;
        beamStarted = true;
        yield return new WaitForSeconds(0.5f);
        GetComponent<Rigidbody2D>().velocity = Vector3.zero;
        yield return new WaitForSeconds(beamDuration-0.5f);
        beamStarted = false;
        beamCooldown = maxBeamCd;
        beamStart.gameObject.SetActive(false);
        beamEnd.gameObject.SetActive(false);
        beamEnd.transform.position=beamStart.transform.position;
        beam.enabled = false;
        yield return new WaitForSeconds(Random.Range(0,2));
        Move();
        yield return new WaitForSeconds(0.5f);
        GetComponent<Rigidbody2D>().velocity = Vector3.zero;
        yield return null;
    }

    private void Move()
    {
       
        Vector2 posToMoveTo = (Vector2)player.position + new Vector2(Random.Range(-moveRange, moveRange), Random.Range(-moveRange, moveRange));
        Vector2 dir = posToMoveTo - (Vector2)transform.position;
        GetComponent<Rigidbody2D>().velocity = dir.normalized * moveSpeed * (dir.magnitude/10);
    }

    void ShootBullets()
    {
        
        if (curHealth > 0 && gameObject.activeInHierarchy)
        {
            AudioManager.Instance.OrbShotSmall.Post(gameObject);
            spreadShot.OnShoot();
            Invoke("ShootBullets", fireDelay);
        }
        
    }
}
