using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BossPhaseTransition : MonoBehaviour
{
    public GameObject newPhase;
    public GameObject bgEffect;
    public bool centerCameraOnThis;
    public Transform toCenterOn;

    public List<GameObject> toEnable = new List<GameObject>();
    public List<GameObject> toDisable = new List<GameObject>();

    public TextMeshProUGUI dText;
    public Animator dAnim;
    public bool disableDialogue;

    private void Start()
    {
        if (centerCameraOnThis)
        {
            Camera.main.GetComponent<CamScript>().player = toCenterOn;
        }

        if (disableDialogue)
        {
            dAnim.gameObject.SetActive(false);
        }
    }

    public void Dialogue(string text)
    {
        dText.text = text;
        dAnim.SetTrigger("Dialogue");
    }

    public void EnableThings()
    {
        foreach (GameObject go in toEnable)
        {
            if (go != null)
            {
                go.SetActive(true);
            }
        }
    }
    

    public void DisableThings()
    {
        foreach (GameObject go in toDisable)
        { if (go != null) { go.SetActive(false);} }
    }


    public void SpawnBoss()
    {
        if (centerCameraOnThis)
        {
            Camera.main.GetComponent<CamScript>().player = GameObject.Find("Player").transform;
        }
        bgEffect.SetActive(false);
        newPhase.SetActive(true);
        newPhase.transform.position = transform.position;
        newPhase.transform.position -= Vector3.forward * 5;
        Destroy(gameObject, 3);
    }
}
