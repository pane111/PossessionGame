using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UltAttack : MonoBehaviour
{
    public List<float> timers = new List<float>();
    public List<GameObject> attacks = new List<GameObject>();

    void StartAttack()
    {
        StartCoroutine(ultimateAttack());
    }

    IEnumerator ultimateAttack()
    {
        for (int i = 0; i < timers.Count; i++)
        {
            GameObject a = Instantiate(attacks[i],transform.position,Quaternion.identity);
            yield return new WaitForSeconds(timers[i]);
            Destroy(a);


        }


        yield return null;
    }
}
