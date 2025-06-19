using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinalBossProjectile : MonoBehaviour
{
    Rigidbody rb;
    Transform player;
    Vector3 dir;
    public bool is2D;
    void Start()
    {
        player = GameObject.Find("Player").transform;
        rb = GetComponent<Rigidbody>();
        Vector3 r = new Vector3(Random.Range(-1,1),Random.Range(-1,1), Random.Range(-1,1));
        rb.AddForce(r.normalized * 50, ForceMode.Impulse);
        Destroy(gameObject, 10);
        
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(player);
        dir = player.position - transform.position;
        rb.velocity = Vector3.Lerp(rb.velocity, dir.normalized * 35, Time.deltaTime*0.7f);
    }
}
