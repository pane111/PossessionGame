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
    Vector2 curPos2;
    public float initDelay=0;
    public bool repeatInOtherDirectionAsWell;
    bool isFirst = true;

    public GameObject attackPrefab;
    void Start()
    {
        curPos = transform.position;
        curPos2 = transform.position;
        Invoke("SpawnAttack", initDelay);
    }

    void SpawnAttack()
    {
        GameObject spawned = Instantiate(attackPrefab,curPos,Quaternion.identity);

        GameObject spawned2 = null;
        if (repeatInOtherDirectionAsWell && !isFirst)
        {
            spawned2 = Instantiate(attackPrefab, curPos2, Quaternion.identity);
            
        }
        isFirst = false;
        if (spawnSpawner)
        {
            spawned.GetComponent<AttackSpawner>().dir = dir2;
        }
        else
        {
            curPos2 += dir2;
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
