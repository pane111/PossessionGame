using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossPhaseTransition : MonoBehaviour
{
    public GameObject newPhase;
    public void SpawnBoss()
    {
        newPhase.SetActive(true);
        newPhase.transform.position = transform.position;
        Destroy(gameObject, 3);
    }
}
