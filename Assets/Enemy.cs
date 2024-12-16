using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float maxHealth = 100;
    public float curHealth = 100;
    public float speed;
    public float detectRange;
    public bool playerFound;
    public Rigidbody2D rb;

    public GameObject hitEffect;
    public ParticleSystem blood;

    public Transform player;
    void Start()
    {
        player = GameObject.Find("Player").transform;
        GetComponent<SpriteRenderer>().color = GameManager.Instance.randomColor();
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 pDir = player.position - transform.position;
        pDir.Normalize();
        RaycastHit2D hit;
        hit = Physics2D.Raycast(transform.position, pDir, detectRange);
        if (hit)
        {
            if (hit.transform == player)
            {
                playerFound = true;
                GetComponent<Animator>().enabled = true;
            }
            else
            {
                playerFound = false;
                GetComponent<Animator>().enabled = false;
            }
        }

        if (playerFound)
        {
            rb.velocity = pDir * speed;
        }
        else
        {
            rb.velocity = Vector2.zero;
        }
    }
    

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.name == "PlayerSword")
        {
            if (other.gameObject.GetComponent<SwordScript>().isSlashing)
            {
                StartCoroutine(TakeDamage(3));
            }
            else
            {
                StartCoroutine(TakeDamage(1));
            }
            Vector2 dir = transform.position - other.transform.position;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            Quaternion lDir = Quaternion.AngleAxis(angle, Vector3.forward);
            GameObject bs = Instantiate(GameManager.Instance.bSplatter,transform.position, lDir);
            

            bs.transform.position = (Vector2)transform.position + dir.normalized;
        }
    }

    IEnumerator TakeDamage(float amount)
    {
        curHealth -= amount;
        ParticleSystem.MainModule ma = blood.main;
        ma.startColor = GameManager.Instance.randomColor();
        blood.Play();
        hitEffect.SetActive(true);
        yield return new WaitForSeconds(0.15f);
        hitEffect.SetActive(false);
        yield return null;
    }
}
