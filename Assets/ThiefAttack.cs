using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThiefAttack : MonoBehaviour
{
    public GameObject spin;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void SpinAtk()
    {
        Instantiate(spin,transform.position,Quaternion.identity);
    }
}
