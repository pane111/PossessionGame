using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deflector : MonoBehaviour
{
    public Transform deflectFrom;
    public float repelForce;
    public ParticleSystem effect;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<SwordScript>() != null)
        {

            collision.gameObject.GetComponent<SwordScript>().Deflected();
            effect.Play();
            Vector2 dir = collision.transform.position - deflectFrom.position;
            collision.gameObject.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            collision.gameObject.GetComponent<Rigidbody2D>().AddForce(dir.normalized * repelForce,ForceMode2D.Impulse);
        }
    }
}
