using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamScript : MonoBehaviour
{
    public Transform player;
    public float followSpeed;
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 followPos = new Vector3(player.position.x, player.position.y, -20);
        transform.position = Vector3.Lerp(transform.position, followPos, Time.deltaTime*followSpeed *1.1f);
        
    }
}
