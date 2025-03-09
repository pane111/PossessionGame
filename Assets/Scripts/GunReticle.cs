using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunReticle : MonoBehaviour
{
    Player p;
    public float shotDelay;
    public ParticleSystem ps;
    bool following = true;
    void Start()
    {
        p = FindObjectOfType<Player>();
        StartCoroutine(ShootPlayer());
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (following)
        {
            transform.position = Vector3.Lerp(transform.position,p.transform.position - Vector3.forward,Time.deltaTime*14);
        }
    }

    IEnumerator ShootPlayer()
    {
        yield return new WaitForSeconds(shotDelay);
        ps.Play();
        yield return new WaitForSeconds(0.9f);
        following = false;
        yield return new WaitForSeconds(0.1f);
        Vector2 dir = transform.position-p.transform.position;
        if (dir.magnitude <= 1.2f)
        {
            p.TakeCustomDamage(20);
        }
        yield return new WaitForSeconds(0.3f);
        following = true;
        StartCoroutine(ShootPlayer());

        yield return null;
    }
}
