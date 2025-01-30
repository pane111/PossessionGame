using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TelegraphedAttack : MonoBehaviour
{
    public float timeUntilHit;
    public SpriteRenderer sr;
    public ParticleSystem effect;
    public float destroyTime;
    public Vector2 size;
    public bool isCircle;
    private void Start()
    {
        Invoke("TriggerEffect", timeUntilHit);
    }


    void TriggerEffect()
    {
        effect.Play();
        if (isCircle)
        {
            Collider2D hitColliders = Physics2D.OverlapCircle(transform.position, size.x);
            if (hitColliders != null)
            {
                print(hitColliders.gameObject);
                if (hitColliders.gameObject.name == "Player")
                {
                    hitColliders.gameObject.GetComponent<Player>().TakeCustomDamage(15);

                }
            }
            
        }
        else
        {
            Collider2D hitColliders = Physics2D.OverlapArea((Vector2)transform.position + size/2, (Vector2)transform.position - size/2);
            if (hitColliders != null)
            {
                print(hitColliders.gameObject);
                if (hitColliders.gameObject.name == "Player")
                {
                    hitColliders.gameObject.GetComponent<Player>().TakeCustomDamage(15);

                }
            }
        }
        
        Destroy(gameObject, destroyTime);
    }
}
