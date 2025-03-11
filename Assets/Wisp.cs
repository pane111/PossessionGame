using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wisp : MonoBehaviour
{
    SpriteRenderer sr;
    float flipTime = 0.15f;

    public string msg;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        flipTime-=Time.deltaTime;
        if (flipTime < 0 )
        {
            sr.flipX = !sr.flipX;
            flipTime = 0.15f;
        }

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if ( collision.gameObject.name=="Player" ) {
            GameManager.Instance.SendNotification(msg);
            Destroy(gameObject);
        
        }
    }
}
