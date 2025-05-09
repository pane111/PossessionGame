using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Seal : MonoBehaviour
{
    public List<DemonHeart> hearts;
    bool broken=false;
    public ParticleSystem burst;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!broken)
        {
            int counter = 0;
            foreach (DemonHeart demonHeart in hearts)
            {
                if (demonHeart.dead)
                {
                    counter++;
                }
            }
            if (counter >= 4)
            {
                broken = true;
                burst.Play();
                GetComponent<Collider2D>().enabled = false;
                GetComponent<SpriteRenderer>().enabled = false;
            }
        }
        
    }
}
