using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EvilSword : MonoBehaviour
{
    public bool following;
    public Transform owner;
    public Transform player;
    Rigidbody2D rb;
    public float speedMod;
    public float maxVel;
    public float minAtkTime;
    public float maxAtkTime;
    public ParticleSystem slashes;
    void Start()
    {
        player = GameObject.Find("Player").transform; 
        rb = GetComponent<Rigidbody2D>();
        float r = Random.Range(minAtkTime, maxAtkTime);
        Invoke("StartAtk", r);
    }

    // Update is called once per frame
    void Update()
    {
        if (following)
        {
            Vector2 dir = owner.position-transform.position;
            rb.AddForce(dir * Mathf.Pow(dir.magnitude, 2) * speedMod);
            transform.Rotate(0, 0, dir.magnitude);
            if (rb.velocity.magnitude > maxVel)
            {
                rb.velocity = Vector2.Lerp(rb.velocity, Vector3.zero, Time.deltaTime * 45);
            }
        }
    }

    void StartAtk()
    {
        StartCoroutine(Attack());
    }
    IEnumerator Attack()
    {
        following = false;
        Vector2 dir = player.position - transform.position;
        rb.velocity = -dir.normalized * 15;
        
        yield return new WaitForSeconds(0.2f);
        rb.AddTorque(100, ForceMode2D.Impulse);
        rb.velocity = Vector2.zero;
        
        yield return new WaitForSeconds(0.8f);
        GetComponent<SpreadShot>().OnShoot();
        rb.AddForce(dir.normalized * 100, ForceMode2D.Impulse);
        slashes.Play();
        yield return new WaitForSeconds(0.25f);
        GetComponent<SpreadShot>().OnShoot();
        rb.velocity = Vector2.zero;
        slashes.Stop();
        yield return new WaitForSeconds(0.2f);
        rb.angularVelocity = 0;
        following = true;
        float r = Random.Range(minAtkTime, maxAtkTime);
        Invoke("StartAtk", r);
        yield return null;
    }

}
