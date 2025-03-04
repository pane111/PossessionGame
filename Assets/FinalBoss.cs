using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinalBoss : BossParentScript
{
    public Color bgColor;
    public Sprite p2;
    public Sprite p3;
    Transform player;
    public Animator clockAnim;
    public Renderer bgR;
    bool ultAttackTriggered = false;
    bool fireandicetriggered = false;
    public GameObject ultAttack;
    public Animator anim;
    public bool canDoThings=true;
    public float ultTimer;
    public GameObject ring;
    bool invincible=false;
    public GameObject fireShooter;
    public GameObject iceShooter;
    public GameObject shooter;
    private void Start()
    {
        player = GameObject.Find("Player").transform;
        CurHealth = maxHealth;
        TakeDamage(0);
        Camera.main.backgroundColor = bgColor;
        bgEffect.SetActive(true);
        //StartCoroutine(TimeManip());
    }

    private void Update()
    {
        if (CurHealth <= maxHealth*0.8f && Time.timeScale == 1 && !fireandicetriggered)
        {
            TriggerFireAndIceAttack();
        }
        if (CurHealth<=maxHealth*0.3f && Time.timeScale==1 && !ultAttackTriggered && fireandicetriggered)
        {
            TriggerUltAttack();
        }
    }
    public void TriggerFireAndIceAttack()
    {
        fireandicetriggered = true;
        StartCoroutine(FireAndIce());
    }

    IEnumerator FireAndIce()
    {
        
        invincible = true;
        dialogueText.text = "The very elements are mine to control! Let's see how you deal with THIS!";
        dialogAnim.SetTrigger("Dialogue");
        GameObject nring = Instantiate(ring, transform.position, Quaternion.identity);
        //float rx = Random.Range(-20, 20);
        //float ry = Random.Range(-12,12);
        Vector3 offset = new Vector3(-12, 0,0);

        GameObject fs = Instantiate(fireShooter, transform.position + offset, Quaternion.identity);
        //rx = Random.Range(-20, 20);
       // ry = Random.Range(-12,12);
        offset = new Vector3(12, 0,0);
        GameObject ics = Instantiate(iceShooter, transform.position + offset, Quaternion.identity);
        yield return new WaitForSeconds(20);
        if (nring.GetComponent<FireRing>().isFire && nring.GetComponent<FireRing>().level>0) {
            dialogueText.text = "Mwahahahaaa! Can you feel the fires of hell searing your flesh? What will you do now, I wonder?";
            dialogAnim.SetTrigger("Dialogue");
        }
        else if (!nring.GetComponent<FireRing>().isFire && nring.GetComponent<FireRing>().level > 0)
        {
            dialogueText.text = "Mwahahahaaa! Can you feel the eternal ice freezing your blood? What will you do now, I wonder?";
            dialogAnim.SetTrigger("Dialogue");
        }
        else if (nring.GetComponent<FireRing>().level==0)
        {
            dialogueText.text = "Hmm... Have you figured it out? No matter... We've only just begun! Mwahahahahahaaaa!!";
            dialogAnim.SetTrigger("Dialogue");
        }

        yield return new WaitForSeconds(10);
        if (nring.GetComponent<FireRing>().level == 0)
        {
            dialogueText.text = "Seems your head isn't completely empty... Very well... it's time I stop holding back!";
            dialogAnim.SetTrigger("Dialogue");
        }
        invincible = false;
        Destroy(ics);
        Destroy(fs);
        yield return new WaitForSeconds(5);
        dialogueText.text = "Time itself bends to my will! I will devour all that lives, has ever lived, and will ever live!";
        dialogAnim.SetTrigger("Dialogue");
        StartCoroutine(TimeManip());
        yield return null;
    }

    IEnumerator TimeManip()
    {
        if (canDoThings)
        {
            float r = Random.Range(0, 100);
            if (r <= 50)
            {
                Time.timeScale = 2f;
                GameManager.Instance.storedTS = 2;
                Time.fixedDeltaTime = 0.02F * Time.timeScale;
            }
            else
            {
                Time.timeScale = 0.65f;
                GameManager.Instance.storedTS = 0.65f;
                Time.fixedDeltaTime = 0.02F * Time.timeScale;
            }
            clockAnim.SetTrigger("Trigger");
            yield return new WaitForSecondsRealtime(8);
            Time.timeScale = 1;
            GameManager.Instance.storedTS =1;
            Time.fixedDeltaTime = 0.02F * Time.timeScale;
            clockAnim.SetTrigger("Trigger");
        }
        
        yield return new WaitForSecondsRealtime(Random.Range(8,13));
        
        StartCoroutine(TimeManip());
        yield return null;
    }
    public void DisableActions()
    {
        canDoThings = false;
        GetComponent<Collider2D>().enabled = false;
    }

    public void EnableActions()
    {
        shooter.SetActive(true);
        portrait = p3;
        dPortrait.sprite = portrait;
        canDoThings = true;
        GetComponent<Collider2D>().enabled = true;
        dialogueText.text = "You still stand...? But how? This cannot be...!";
        dialogAnim.SetTrigger("Dialogue");
    }

    void TriggerUltAttack()
    {
        shooter.SetActive(false);
        portrait = p2;
        dPortrait.sprite = portrait;
        ultAttackTriggered = true;
        DisableActions();
        dialogueText.text = "Souls of fallen warriors, come to me! I command you... erase this filth!";
        dialogAnim.SetTrigger("Dialogue");
        anim.SetTrigger("Ult");
        Invoke("EnableActions", ultTimer);
        GameObject u = Instantiate(ultAttack,transform.position, Quaternion.identity);
        
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<SwordScript>() != null && !invincible)
        {

            SwordScript sword = collision.GetComponent<SwordScript>();
            if (sword.curTarget == this.gameObject.transform) { sword.attacksCount++; }
            Vector2 dir = transform.position - collision.transform.position;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            Quaternion lDir = Quaternion.AngleAxis(angle, Vector3.forward);
            GameObject bs = Instantiate(GameManager.Instance.bSplatter, transform.position, lDir);

            bs.transform.position = (Vector2)transform.position + dir.normalized;
            if (collision.GetComponent<SwordScript>().isSlashing)
            {
                if (CurHealth - 3 <= 0)
                {
                    collision.GetComponent<SwordScript>().curTarget = player;
                    
                }
                TakeDamage(3);

            }
            else
            {
                if (CurHealth - 1 <= 0)
                {
                    collision.GetComponent<SwordScript>().curTarget = player;
                }
                TakeDamage(1);
                if (sword.curTarget == sword.playerChar || sword.curTarget == sword.playerChar.gameObject.GetComponent<Player>().orbiter) { sword.curTarget = this.gameObject.transform; sword.Idling = false; sword.SIforNPC(); }
            }
            if (CurHealth <= 0)
            {
                dialogueText.text = "N-no... Please... I don't want to die... Not yet...! Noooooo!!!";
                dialogAnim.SetTrigger("Dialogue");
            }

            bgR.material.SetFloat("_Exposure", 1 - (1 - (CurHealth / maxHealth))*1);
        }
    }
}
