using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deflector : MonoBehaviour
{
    public Transform deflectFrom;
    public bool deflectionActive = true;
    public bool deflectPlayerOnTouch;
    public float repelForce;
    public ParticleSystem effect;
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<SwordScript>() != null && deflectionActive)
        {

            collision.gameObject.GetComponent<SwordScript>().Deflected();
            effect.Play();
            Vector2 dir = collision.transform.position - deflectFrom.position;
            collision.gameObject.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            collision.gameObject.GetComponent<Rigidbody2D>().AddForce(dir.normalized * repelForce,ForceMode2D.Impulse);
        }

        if (collision.gameObject.name=="Player" && deflectionActive && deflectPlayerOnTouch)
        {
            Player p = collision.gameObject.GetComponent<Player>();
            p.enabled = false;
            p.GetComponent<Rigidbody2D>().velocity = (p.transform.position - deflectFrom.position).normalized * repelForce;
            effect.Play();
            p.TakeCustomDamage(10);
            p.knockbackFailsafe(0.1f);
        }
    }
}
