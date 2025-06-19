using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotating : MonoBehaviour
{
    public float speed;
    void Update()
    {
        transform.Rotate(0,0,speed);
        if (transform.rotation.z >= 360 || transform.rotation.z <= -360)
        {
            transform.rotation= Quaternion.Euler(0,0,0);
        }
    }
}
