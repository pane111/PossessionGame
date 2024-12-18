using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatueScript : MonoBehaviour
{
    public float cooldown;
    public GameObject effect;
    public void Reactivate()
    {
        effect.SetActive(true);
        GetComponent<Collider2D>().enabled = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name=="Player")
        {
            effect.SetActive(false);
            GetComponent<Collider2D>().enabled = false;
            Invoke("Reactivate", cooldown);
        }
    }
}
