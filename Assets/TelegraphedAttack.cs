using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TelegraphedAttack : MonoBehaviour
{
    public float timeUntilHit;
    public SpriteRenderer sr;
    public ParticleSystem effect;
    public GameObject eAnim;
    public GameObject toSpawn;
    public float damage = 5;
    public bool kb;
    public enum DangerType
    {
        Effect,
        Anim,
        Spawn
    }
    public DangerType type;

    public float destroyTime;
    public Vector2 size;
    public bool isCircle;
    private void Start()
    {
        Invoke("TriggerEffect", timeUntilHit);
        GetComponent<Animator>().SetFloat("Time", 1/timeUntilHit);
    }


    void TriggerEffect()
    {
        if (type == DangerType.Effect)
        {
            effect.Play();
        }
        else if (type == DangerType.Spawn)
        {
            Instantiate(toSpawn,transform.position,Quaternion.identity);
        }
        else
        {
            eAnim.SetActive(true);
        }

        
        if (isCircle)
        {
            Collider2D hitColliders = Physics2D.OverlapCircle(transform.position, size.x);
            if (hitColliders != null)
            {
                print(hitColliders.gameObject);
                if (hitColliders.gameObject.name == "Player")
                {
                    hitColliders.gameObject.GetComponent<Player>().TakeCustomDamage(damage);
                    if (kb)
                    {
                        hitColliders.gameObject.GetComponent<Player>().TriggerKB(transform.position);
                    }

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
                    hitColliders.gameObject.GetComponent<Player>().TakeCustomDamage(damage);
                    if (kb)
                    {
                        hitColliders.gameObject.GetComponent<Player>().TriggerKB(transform.position);
                    }

                }
            }
        }
        
        Destroy(gameObject, destroyTime);
    }
}
