using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackSpawner : MonoBehaviour
{
    public int repeatTimes;
    public float repeatDelay;
    public Vector2 dir;
    public Vector2 dir2;
    public bool spawnSpawner;
    public bool deleteAfter;
    public float deleteTime;
    Vector2 curPos;

    public GameObject attackPrefab;
    void Start()
    {
        curPos = transform.position;
        SpawnAttack();
    }

    void SpawnAttack()
    {
        GameObject spawned = Instantiate(attackPrefab,curPos,Quaternion.identity);

        if (spawnSpawner)
        {
            spawned.GetComponent<AttackSpawner>().dir = dir2;
        }
        if (deleteAfter)
        {
            Destroy(spawned, deleteTime);
        }
        curPos += dir;
        if (repeatTimes>0)
        {
            repeatTimes--;

            Invoke("SpawnAttack", repeatDelay);
        }
        else
        {
            Destroy(gameObject);
        }
    }

}
