using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerSimple : MonoBehaviour
{
    public float speed;
    public SpriteRenderer sr;
    public float stepDur;
    Rigidbody2D rb;
    float curStep=0;
    public Animator flash;

    int kills = 0;
    public List<string> dialogues = new List<string> ();
    public TextMeshProUGUI swordDialogue;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        if (input.magnitude > 0 )
        {
            curStep += Time.deltaTime;
            rb.velocity = input.normalized * speed;
            if (curStep > stepDur )
            {
                curStep = 0;
                sr.flipX = !sr.flipX;
            }
           
        }
        else
        {
            rb.velocity = Vector3.zero;
        }
    }
    IEnumerator OnKill()
    {
        if (kills < dialogues.Count)
            swordDialogue.text = dialogues[kills];

        kills++;
        speed += 0.25f;
        float initSpeed = speed;
        speed = 0;
        yield return new WaitForSeconds(0.25f);
        while (speed < initSpeed)
        {
            speed += Time.deltaTime*1.5f;
            yield return null;
        }
        speed = initSpeed;

        yield return null;
    }
    public void ScreenFlash()
    {
        StartCoroutine(OnKill());
        
        flash.SetTrigger("Flash");
    }
}
