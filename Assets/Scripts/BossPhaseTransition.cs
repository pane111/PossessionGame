using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossPhaseTransition : MonoBehaviour
{
    public GameObject newPhase;
    public GameObject bgEffect;
    public void SpawnBoss()
    {
        bgEffect.SetActive(false);
        newPhase.SetActive(true);
        newPhase.transform.position = transform.position;
        Destroy(gameObject, 3);
    }
}
