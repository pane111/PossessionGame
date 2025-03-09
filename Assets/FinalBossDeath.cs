using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FinalBossDeath : MonoBehaviour
{
    public List<GameObject> toDisable = new List<GameObject>();
    public ParticleSystem playerRepel;
    public ParticleSystem bleed;
    void Start()
    {
        foreach (GameObject go in toDisable)
        {
            if (go != null)
            {
                go.SetActive(false);
            }
        }
        Camera.main.GetComponent<CamScript>().player = this.transform;
    }

    public void Repel()
    {
        playerRepel.Play();
    }
    public void Bleed()
    {
        bleed.Play();
    }
    public void PlayEnding()
    {
        GameManager.Instance.TriggerEnding();
    }
}
