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
    public bool ice;
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
    private void OnDrawGizmos()
    {
        Gizmos.DrawCube(transform.position, new Vector3(size.x, size.y, 0));
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
            Collider2D hitColliders = Physics2D.OverlapCircle(transform.position, size.x,~7);
            if (hitColliders != null)
            {
                print(hitColliders.gameObject);
                if (hitColliders.gameObject.name == "Player" && damage != 0)
                {
                    hitColliders.gameObject.GetComponent<Player>().TakeCustomDamage(damage);
                    if (kb)
                    {
                        hitColliders.gameObject.GetComponent<Player>().TriggerKB(transform.position);
                    }

                }

                if (damage == 0 && hitColliders.gameObject.tag=="Ring")
                {
                        hitColliders.gameObject.GetComponent<FireRing>().Hit(ice);
                }
            }
            
        }
        else
        {
            Collider2D hitColliders = Physics2D.OverlapBox(transform.position, size,0,~7);
            //Collider2D hitColliders = Physics2D.OverlapArea((Vector2)transform.position + size/2, (Vector2)transform.position - size/2);
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
