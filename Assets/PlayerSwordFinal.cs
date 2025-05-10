using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSwordFinal : MonoBehaviour
{
    public bool following;
    public Transform owner;
    public Transform target;
    Rigidbody rb;
    public float speedMod;
    public float maxVel;
    public float minAtkTime;
    public float maxAtkTime;
    public ParticleSystem slashes;
    public float repelForce;
    public Transform sprite;
    bool pullCancelled = false;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        float r = Random.Range(minAtkTime, maxAtkTime);
        //Invoke("StartAtk", r);
    }

    // Update is called once per frame
    void Update()
    {
        
        sprite.LookAt(owner);
        if (following)
        {
            Vector3 dir = owner.position - transform.position;
            rb.AddForce(dir * Mathf.Pow(dir.magnitude, 2) * speedMod);
            transform.Rotate(0, 0, dir.magnitude);
            if (rb.velocity.magnitude > maxVel)
            {
                rb.velocity = Vector3.Lerp(rb.velocity, Vector3.zero, Time.deltaTime * 45);
            }
        }
        if ((owner.position - transform.position).magnitude > 200)
        {
            rb.velocity = Vector3.zero;
            transform.position = owner.position;
        }
    }

    void StartAtk()
    {
        StartCoroutine(Attack());
    }
    public void Repel()
    {
        
            StartCoroutine(RepelC());
        
        
    }
    public void Pull()
    {
        StartCoroutine(PullC());
    }
    IEnumerator PullC()
    {
        following = false;

        yield return new WaitForSeconds(0.15f);
        rb.velocity = Vector3.zero;
        slashes.Stop();
        Vector3 dir = owner.position - transform.position;
        transform.position = owner.position;
        Camera.main.GetComponent<CameraRotation>().rb.AddForce(-dir.normalized * 5,ForceMode.Impulse);
        
        yield return new WaitForSeconds(1);
        Camera.main.GetComponent<CameraRotation>().canRepel = true;
        if (!pullCancelled) { following = true; }
        
        yield return new WaitForSeconds(1);
        Camera.main.GetComponent<CameraRotation>().canPull = true;
        
        yield return null;
    }
    IEnumerator RepelC()
    {
        if ((owner.position - transform.position).magnitude <= 12)
        {
            pullCancelled = true;
            following = false;
            rb.velocity = Vector3.zero;
            transform.position = owner.position;
            yield return new WaitForSeconds(0.1f);
            Camera.main.GetComponent<CameraRotation>().repel.Play();
            yield return new WaitForSeconds(0.1f);
            
            rb.AddForce(Camera.main.transform.forward.normalized * repelForce, ForceMode.Impulse);
            rb.AddTorque(rb.velocity.normalized * 25, ForceMode.Impulse);
            slashes.Play();
            Camera.main.GetComponent<CameraRotation>().canPull = true;
            yield return new WaitForSeconds(2);
            slashes.Stop();
            Camera.main.GetComponent<CameraRotation>().canRepel = true;
            rb.velocity = Vector3.zero;
            following = true;
            pullCancelled = false;
            yield return null;
        }
        else
        {
            yield return new WaitForSeconds(0.5f);
            Camera.main.GetComponent<CameraRotation>().canRepel = true;
            Camera.main.GetComponent<CameraRotation>().canPull = true;
        }
        
        yield return null;
    }
    IEnumerator Attack()
    {
        following = false;
        Vector2 dir = target.position - transform.position;
        rb.velocity = -dir.normalized * 15;

        yield return new WaitForSeconds(0.2f);
        rb.AddTorque(rb.velocity.normalized * 5, ForceMode.Impulse);
        rb.velocity = Vector3.zero;

        yield return new WaitForSeconds(0.8f);
        GetComponent<SpreadShot>().OnShoot();
        rb.AddForce(dir.normalized * 100, ForceMode.Impulse);
        slashes.Play();
        yield return new WaitForSeconds(0.25f);
        GetComponent<SpreadShot>().OnShoot();
        rb.velocity = Vector3.zero;
        slashes.Stop();
        yield return new WaitForSeconds(0.2f);
        rb.angularVelocity = Vector3.zero;
        following = true;
        float r = Random.Range(minAtkTime, maxAtkTime);
        Invoke("StartAtk", r);
        yield return null;
    }
}
