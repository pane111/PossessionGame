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

    public bool transformPlayer;
    public GameObject additionalCutscene;
    public Animator addAnim;

    public Transform altFocus;
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
    public void AddCutscene()
    {
        additionalCutscene.SetActive(true);
    }
    public void TriggerAddAnim()
    {
        addAnim.SetTrigger("Trigger");
    }
    public void FocusAlt()
    {
        Camera.main.GetComponent<CamScript>().player = altFocus;
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
            Camera.main.GetComponent<CamScript>().player = FindObjectOfType<Player>().camTarget.transform.GetChild(0);
        }
        if (transformPlayer)
        {
            GameManager.Instance.player.FBDemonMode();
        }
        bgEffect.SetActive(false);
        newPhase.SetActive(true);
        newPhase.transform.position = transform.position;
        newPhase.transform.position -= Vector3.forward * 5;
        Destroy(gameObject, 3);
    }
}
