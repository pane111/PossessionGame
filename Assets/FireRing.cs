using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FireRing : MonoBehaviour
{
    public bool isFire;
    public float detonationTime;
    public int level=1;
    Player p;
    public Color fireColor;
    public Color iceColor;
    public GameObject fireAura;
    public GameObject iceAura;
    public ParticleSystem fireDet;
    public ParticleSystem iceDet;
    public TextMeshPro counter;
    
    void Start()
    {
        int r = Random.Range(0, 100);
        level = Random.Range(1, 4);
        print(r);
        if (r < 50)
        {
            isFire = true;
        }
        else
        {
            isFire = false;
        }

        counter.text = level.ToString();
        p=FindObjectOfType<Player>();
        transform.position = p.transform.position;
        transform.parent = p.transform;
        if (isFire)
        {
            GetComponent<SpriteRenderer>().color = fireColor;
            fireAura.SetActive(true);
            iceAura.SetActive(false);
        }
        else
        {
            GetComponent<SpriteRenderer>().color = iceColor;
            fireAura.SetActive(false);
            iceAura.SetActive(true);
        }
    }

    // Update is called once per frame
    void Update()
    {
        detonationTime -= Time.deltaTime;
        if (detonationTime<0)
        {
            Detonate();
            detonationTime = 9999;
        }
    }

    public void Detonate()
    {
        GetComponent<Collider2D>().enabled = false;
        
        transform.parent = null;
        fireAura.SetActive(false);
        iceAura.SetActive(false);
        if (level>0)
        {
            counter.color = Color.red;
            p.TakeForcedDamage(30 * level);
            if (isFire)
            {
                fireDet.Play();
            }
            else
            {
                iceDet.Play();
            }
        }
        else
        {
            counter.color = Color.green;
        }
        GetComponent<SpriteRenderer>().enabled = false; 
           
        Destroy(gameObject,2);
    }
    public void Hit(bool ice)
    {
        if (ice)
        {
            if (isFire)
            {
                level--;
            }
            else
            {
                level++;
            }
        }
        else
        {
            if (isFire)
            {
                level++;
            }
            else
            {
                level--;
            }
        }
        if (level == 0)
        {
            GetComponent<SpriteRenderer>().color = Color.gray;
            iceAura.SetActive(false);
            fireAura.SetActive(false);
        }
        if (level < 0)
        {
            isFire = !isFire;
            level = 1;
            if (isFire)
            {
                GetComponent<SpriteRenderer>().color = fireColor;
                fireAura.SetActive(true);
                iceAura.SetActive(false);
            }
            else
            {
                GetComponent<SpriteRenderer>().color = iceColor;
                fireAura.SetActive(false);
                iceAura.SetActive(true);
            }
        }
        if (level > 0)
        {
            if (isFire)
            {
                GetComponent<SpriteRenderer>().color = fireColor;
                fireAura.SetActive(true);
                iceAura.SetActive(false);
            }
            else
            {
                GetComponent<SpriteRenderer>().color = iceColor;
                fireAura.SetActive(false);
                iceAura.SetActive(true);
            }
        }
        counter.text = level.ToString();
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isFire)
        {
            if (other.gameObject.tag=="Fire")
            {
                level++;
            }
            else if (other.gameObject.tag=="Ice")
            {
                level--;
            }
        }
        else
        {
            if (other.gameObject.tag == "Fire")
            {
                level--;
            }
            else if (other.gameObject.tag == "Ice")
            {
                level++;
            }
        }
        
        if (level==0)
        {
            GetComponent<SpriteRenderer>().color = Color.gray;
            iceAura.SetActive(false);
            fireAura.SetActive(false);
        }
        if (level<0)
        {
            isFire = !isFire;
            level = 1;
            if (isFire)
            {
                GetComponent<SpriteRenderer>().color = fireColor;
                fireAura.SetActive(true);
                iceAura.SetActive(false);
            }
            else
            {
                GetComponent<SpriteRenderer>().color = iceColor;
                fireAura.SetActive(false);
                iceAura.SetActive(true);
            }
        }
        if (level>0)
        {
            if (isFire)
            {
                GetComponent<SpriteRenderer>().color = fireColor;
                fireAura.SetActive(true);
                iceAura.SetActive(false);
            }
            else
            {
                GetComponent<SpriteRenderer>().color = iceColor;
                fireAura.SetActive(false);
                iceAura.SetActive(true);
            }
        }
        counter.text = level.ToString();
    }
}
