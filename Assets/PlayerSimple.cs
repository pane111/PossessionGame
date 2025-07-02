using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerSimple : MonoBehaviour
{
    public float speed;
    public SpriteRenderer sr;
    Rigidbody2D rb;
    public Animator flash;
    public Animator anim;
    int kills = 0;
    public List<string> dialogues = new List<string> ();
    public TextMeshProUGUI swordDialogue;
    void Start()
    {
        anim= GetComponentInChildren<Animator> ();
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        if (input.magnitude > 0 )
        {
            anim.SetFloat("Speed", speed/3 * 0.7f);
            anim.SetFloat("X", input.x);
            anim.SetFloat ("Y", input.y);
            rb.velocity = speed * input.normalized;
           
        }
        else
        {
            anim.SetFloat("Speed", 0);
            rb.velocity = Vector3.zero;
        }
    }
    IEnumerator OnKill()
    {
        /*
        if (kills < dialogues.Count)
            swordDialogue.text = dialogues[kills];
        */
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
