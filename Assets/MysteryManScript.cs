using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MysteryManScript : MonoBehaviour
{
    public Transform player;
    float pDist;


    [TextArea(6, 10)]
    public List<string> lines = new List<string>();
    public GameObject dWindow;
    public TextMeshProUGUI dialogueText;
    int curLine = 0;
    bool canProceed = false;
    bool dialogueOpen = false;
    public GameObject portal;
    AudioSource a;
    void Start()
    {
        a = GetComponent<AudioSource>();
        player = GameObject.Find("Player").transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (!dialogueOpen)
        {
            pDist = (player.position-transform.position).magnitude;
            if (pDist < 3)
            {
                dialogueOpen = true;
                StartCoroutine(typeDialogue());
                dWindow.SetActive(true);
                player.GetComponent<PlayerSimple>().anim.SetFloat("Speed", 0); ;
                player.GetComponent<PlayerSimple>().enabled = false;
                player.GetComponent<Rigidbody2D>().velocity = Vector3.zero;
            }
        }
        else
        {
            if (Input.GetButtonDown("Jump") || Input.GetButtonDown("Fire1"))
            {
                if (canProceed)
                {
                    StartCoroutine(typeDialogue());
                }
                else
                {
                    dialogueText.maxVisibleCharacters = lines[curLine].Length;
                }
            }
        }
    }
    IEnumerator typeDialogue()
    {
        if (curLine < lines.Count)
        {
            dialogueText.maxVisibleCharacters = 0;
            dialogueText.text = lines[curLine];
            canProceed = false;
            while (dialogueText.maxVisibleCharacters < lines[curLine].Length)
            {

                a.pitch = Random.Range(0.8f, 1.1f);
                a.Play();
                dialogueText.maxVisibleCharacters++;
                yield return new WaitForSeconds(0.01f);
                a.Stop();
                yield return null;
            }
            canProceed = true;
            curLine++;
        }
        else
        {
            player.GetComponent<PlayerSimple>().enabled = true;
            dWindow.SetActive(false);
            portal.SetActive(true);
            Destroy(gameObject);
        }
        
        yield return null;
    }
}
