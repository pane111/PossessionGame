using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnRandomThing : MonoBehaviour
{
    public List<GameObject> toSpawn = new List<GameObject>();
    public float minDelay;
    public float maxDelay;
    void Start()
    {
        Invoke("Spawn",Random.Range(minDelay,maxDelay));
    }

    void Spawn()
    {
        int r = Random.Range(0,toSpawn.Count);
        GameObject s = Instantiate(toSpawn[r],transform.position,Quaternion.identity);
        gameObject.SetActive(false);
    }
}
