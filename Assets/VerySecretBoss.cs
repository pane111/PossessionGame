using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class VerySecretBoss : MonoBehaviour
{

    public Image hpBar;
    public int maxHp;
    public int curHp;
    int savedHp;
    public Animator hpBarAnim;
    public TextMeshProUGUI dText;
    public GameObject blades;
    public SpreadShot shooter;
    public float charDelay;
    public float dialogueDelay;
    public bool attacked;
    public bool parried;
    public SecretPlayer p;
    public GameObject stars;
    IEnumerator battle2;
    AudioSource a;
    void Start()
    {
        a = GetComponent<AudioSource>();
        StartCoroutine(ConductBattle());
        p.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    IEnumerator ConductBattle()
    {
        print("Conducting battle");
        yield return new WaitForSeconds(1);
        string dialogue = "Hehehe... How's this?";
        StartCoroutine(Dialogue(dialogue));
        yield return new WaitForSeconds(dialogue.Length * charDelay + dialogueDelay);
        dialogue = "Let me explain how this all works.";
        StartCoroutine(Dialogue(dialogue));
        yield return new WaitForSeconds(dialogue.Length * charDelay + dialogueDelay);
        dialogue = "See that bar under your health bar? That is your Action Points, or AP.";
        StartCoroutine(Dialogue(dialogue));
        yield return new WaitForSeconds(dialogue.Length * charDelay + dialogueDelay);
        dialogue = "You'll gain AP over time.";
        StartCoroutine(Dialogue(dialogue)); ;
        yield return new WaitForSeconds(dialogue.Length * charDelay + dialogueDelay);
        dialogue = "When you have at least 1 AP, you can press the 'Repel' button to attack me!";
        StartCoroutine(Dialogue(dialogue));
        yield return new WaitForSeconds(dialogue.Length * charDelay + dialogueDelay);
        dialogue = "Now try it! Press the 'Repel' button once you get some AP!";
        StartCoroutine(Dialogue(dialogue));
        yield return new WaitForSeconds(dialogue.Length * charDelay);
        p.enabled = true;
        while (!attacked)
        {
            yield return null;
        }
        dialogue = "Yes, yes! Perfect! Just like that!";
        StartCoroutine(Dialogue(dialogue));
        yield return new WaitForSeconds(dialogue.Length * charDelay + dialogueDelay);
        dialogue = "Now... why don't we learn about blocking?";
        StartCoroutine(Dialogue(dialogue));
        yield return new WaitForSeconds(dialogue.Length * charDelay + dialogueDelay);
        dialogue = "You can press the 'Dash' button to block. Hold it down to keep blocking!";
        StartCoroutine(Dialogue(dialogue));
        yield return new WaitForSeconds(dialogue.Length * charDelay + dialogueDelay);
        dialogue = "When you block one of my attacks, you take less damage, but lose some AP. Don't go below 0!";
        StartCoroutine(Dialogue(dialogue));
        yield return new WaitForSeconds(dialogue.Length * charDelay + dialogueDelay);
        dialogue = "Now... try blocking my attack right before it hits you.";
        StartCoroutine(Dialogue(dialogue));
        yield return new WaitForSeconds(dialogue.Length * charDelay + dialogueDelay);
        shooter.OnShoot();
        while (!parried)
        {
            if (curHp <= 80)
            {
                curHp = 100;
                hpBar.fillAmount = (float)curHp / (float)maxHp;
                dialogue = "You think you're funny, huh? Sorry pal, but that won't work. I make the rules here.";
                StartCoroutine(Dialogue(dialogue));
                yield return new WaitForSeconds(dialogue.Length * charDelay + dialogueDelay);
                dialogue = "Now... try blocking my attack right before it hits you.";
                StartCoroutine(Dialogue(dialogue));
                yield return new WaitForSeconds(dialogue.Length * charDelay + dialogueDelay);
            }
            yield return null;
        }
        dialogue = "Perfect!! You just performed a Parry. When you Parry an attack, you gain a lot of AP and even heal!";
        StartCoroutine(Dialogue(dialogue));
        yield return new WaitForSeconds(dialogue.Length * charDelay + dialogueDelay);
        dialogue = "It's quite magnificent. Try Parrying as much as possible...!";
        StartCoroutine(Dialogue(dialogue));
        yield return new WaitForSeconds(dialogue.Length * charDelay + dialogueDelay);
        dialogue = "";
        StartCoroutine(Dialogue(dialogue));
        yield return new WaitForSeconds(dialogue.Length * charDelay + dialogueDelay);

        dialogue = "Now... let's increase the frequency of my attacks.";
        StartCoroutine(Dialogue(dialogue));
        yield return new WaitForSeconds(dialogue.Length * charDelay + dialogueDelay);
        shooter.retriggerDelay = 1.5f;
        dialogue = "Do you think this is too hard? Or fine? Hehe...";
        StartCoroutine(Dialogue(dialogue));
        yield return new WaitForSeconds(dialogue.Length * charDelay + dialogueDelay*4);
        dialogue = "Hmmm... it's getting a little repetitive. Let's spice it up by adding another attack.";
        StartCoroutine(Dialogue(dialogue));
        yield return new WaitForSeconds(dialogue.Length * charDelay + dialogueDelay);
        dialogue = "You may find this one a bit more difficult to parry. But I believe in you.";
        StartCoroutine(Dialogue(dialogue));
        yield return new WaitForSeconds(dialogue.Length * charDelay + dialogueDelay);
        shooter.retriggerDelay = 5;
        blades.SetActive(true);
        dialogue = "";
        StartCoroutine(Dialogue(dialogue));
        yield return new WaitForSeconds(dialogue.Length * charDelay + dialogueDelay*6);
        dialogue = "Oh, and don't worry about dying. I haven't really thought about what should happen if you die yet...";
        StartCoroutine(Dialogue(dialogue));
        yield return new WaitForSeconds(dialogue.Length * charDelay + dialogueDelay);
        dialogue = "But there should be *some* kind of punishment, right?";
        StartCoroutine(Dialogue(dialogue));
        yield return new WaitForSeconds(dialogue.Length * charDelay + dialogueDelay);
        blades.SetActive(false);
        shooter.canShoot = false;
        dialogue = "How about I give you a checkpoint right here? And then we have a proper fight?";
        StartCoroutine(Dialogue(dialogue));
        yield return new WaitForSeconds(dialogue.Length * charDelay + dialogueDelay);
        dialogue = "Okay, from now on, if you die, you gotta restart... Hehe!";
        StartCoroutine(Dialogue(dialogue));
        yield return new WaitForSeconds(dialogue.Length * charDelay + dialogueDelay*2);
        savedHp = curHp;
        Part2();
        yield return null;

    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Sword"))
        {
            TakeDamage();
        }
    }
    public void Part2()
    {
        curHp = savedHp;
        hpBar.fillAmount = (float)curHp / (float)maxHp;
        p.canDie = true;
        stars.GetComponent<SpreadShot>().canShoot = false;
        stars.SetActive(false);
        shooter.retriggerDelay = 3;
        if (battle2 != null)
        {
            StopCoroutine(battle2);
        }
        battle2 = BattlePart2();
        StartCoroutine(battle2);
    }
    IEnumerator BattlePart2()
    {
        string dialogue = "";
        StartCoroutine(Dialogue(dialogue));
        yield return new WaitForSeconds(dialogue.Length * charDelay + dialogueDelay);
        dialogue = "Now... Let's do this for real! Hehehe!";
        StartCoroutine(Dialogue(dialogue));
        yield return new WaitForSeconds(dialogue.Length * charDelay + dialogueDelay);
        p.redFlash.SetActive(false);
        shooter.canShoot = true;
        blades.SetActive(true);
        dialogue = "";
        StartCoroutine(Dialogue(dialogue));
        yield return new WaitForSeconds(dialogue.Length * charDelay + dialogueDelay);
        yield return new WaitForSeconds(10);
        dialogue = "I'm adding a new attack... teehee!";
        StartCoroutine(Dialogue(dialogue));
        yield return new WaitForSeconds(dialogue.Length * charDelay + dialogueDelay);
        stars.SetActive(true);
        stars.GetComponent<SpreadShot>().canShoot = true;
        shooter.retriggerDelay = 7;
        dialogue = "";
        StartCoroutine(Dialogue(dialogue));
        yield return new WaitForSeconds(dialogue.Length * charDelay + dialogueDelay);
        dialogue = "Are you enjoying this...?";
        StartCoroutine(Dialogue(dialogue));
        yield return new WaitForSeconds(dialogue.Length * charDelay + dialogueDelay);
        dialogue = "Isn't this so much more engaging?";
        StartCoroutine(Dialogue(dialogue));
        yield return new WaitForSeconds(dialogue.Length * charDelay + dialogueDelay);
        dialogue = "...I doubt you'd be able to keep up if I added more attacks, though.";
        StartCoroutine(Dialogue(dialogue));
        yield return new WaitForSeconds(dialogue.Length * charDelay + dialogueDelay);
        dialogue = "...";
        StartCoroutine(Dialogue(dialogue));
        yield return new WaitForSeconds(dialogue.Length * charDelay + dialogueDelay);
        dialogue = "";
        StartCoroutine(Dialogue(dialogue));
        yield return new WaitForSeconds(dialogue.Length * charDelay + dialogueDelay);
        dialogue = "Hmmm... What to do now...?";
        StartCoroutine(Dialogue(dialogue));
        yield return new WaitForSeconds(dialogue.Length * charDelay + dialogueDelay);
        dialogue = "";
        StartCoroutine(Dialogue(dialogue));
        yield return new WaitForSeconds(dialogue.Length * charDelay + dialogueDelay*3);
        dialogue = "...";
        StartCoroutine(Dialogue(dialogue));
        yield return new WaitForSeconds(dialogue.Length * charDelay + dialogueDelay);
        dialogue = "Okay I'm bored let's have a normal fight";
        StartCoroutine(Dialogue(dialogue));
        yield return new WaitForSeconds(dialogue.Length * charDelay + dialogueDelay*2);
        SceneManager.LoadScene("FinalFightForReal");
        yield return null;
        
    }
    public void TakeDamage()
    {
        attacked = true;
        curHp--;
        hpBarAnim.SetTrigger("Hit");
        GetComponent<Animator>().SetTrigger("Damage");
        hpBar.fillAmount = (float)curHp / (float)maxHp;
    }
    IEnumerator Dialogue(string text)
    {
        dText.maxVisibleCharacters = 0;
        dText.text = text;
        while (dText.maxVisibleCharacters < text.Length) { 
            
            dText.maxVisibleCharacters++;
            a.pitch = Random.Range(0.8f, 1.1f);
            a.Play();
            yield return new WaitForSeconds(charDelay);
            a.Stop();
            yield return null;
        
        }
        dText.maxVisibleCharacters = text.Length;
        yield return null;
    }
}
