using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.UI;

public class LastFight : MonoBehaviour
{
    public Animator dAnim;
    public TextMeshProUGUI dialogue;
    Coroutine dCo;
    public float dTime;
    public float dStayTime;
    public GameObject rFire;
    Player p;
    public Animator hpBarAnim;
    public Image hpBar;
    public float curHp;
    public float maxHp;
    public SpriteRenderer sr;
    string userName;
    public GameObject shooters1;
    public bool invincible=true;
    public bool skipDialogue = false;
    public bool skipToFinalAttack = false;
    public float healAmt;
    public List<Transform> moveLocations;
    public float moveRange;
    public GameObject spinSlash;
    public GameObject spellslinger;
    public GameObject spellslingerLeft;
    public Sprite spellslingerSprite;
    public Sprite bucketheadSprite;
    public Sprite bunnyCharacter;
    public Sprite bozoSprite;

    public GameObject circleAttack;
    public GameObject bunny;

    public Image specialPortrait;
    public Animator specialAnim;

    public Image dPortrait;
    public Sprite bunnySprite;
    public Sprite bossSprite;

    public GameObject bucketHead;
    public GameObject bozo;
    bool bozoTime = false;
    public Animator flash;
    public List<GameObject> visuals = new List<GameObject>();
    bool canMove = true;

    public SpreadShot ss2;
    public SpreadShot ss3;
    public SpreadShot ss4;
    public GameObject otherUlt;

    bool finalPhase;
    public GameObject finalPhaseGO;
    public FinalChaosPhase f;
    AudioSource aus;
    public AudioSource hit;
    public AudioSource laugh;
    public AudioSource speak;

    public AudioSource p2Music;
    void Start()
    {
        aus = GetComponent<AudioSource>();
        f.hit = hit;
        flash.SetTrigger("Flash");
        aus.Play();
        finalPhaseGO.SetActive(false);
        p = FindObjectOfType<Player>();
        p.onDeath += Heal;
        hpBarAnim.gameObject.SetActive(false);
        userName = System.Environment.UserName;
        if (!skipDialogue )
        {
            StartCoroutine(PreBattle());
        }
        else
        {
            rFire.SetActive(true);
            p.speed *= 2.5f;
            p.maxDashCooldown = 0.25f;
            p.dashForce *= 1.4f;
            StartCoroutine(ConductBattle());
        }
        
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    IEnumerator SpawnSpellslingers()
    {
        StartSpecial(spellslingerSprite);
        Instantiate(spellslinger, moveLocations[1].position,Quaternion.identity);
        yield return new WaitForSeconds(0.5f);
        Instantiate(spellslinger, moveLocations[2].position, Quaternion.identity);
        yield return new WaitForSeconds(0.5f);
        Instantiate(spellslinger, moveLocations[3].position, Quaternion.identity);
        yield return new WaitForSeconds(0.5f);
        yield return new WaitForSeconds(1);
        Instantiate(spellslingerLeft, moveLocations[5].position,Quaternion.identity);
        yield return new WaitForSeconds(0.5f);
        Instantiate(spellslingerLeft, moveLocations[6].position, Quaternion.identity);
        yield return new WaitForSeconds(0.5f);
        Instantiate(spellslingerLeft, moveLocations[7].position, Quaternion.identity);
        yield return new WaitForSeconds(0.5f);
        yield return new WaitForSeconds(40);
        if (!bozoTime)
            StartCoroutine(SpawnSpellslingers());

        yield return null;
    }
    IEnumerator PreBattle()
    {
            StartDialogue("Hehehe... Is this more to your liking?");
            yield return new WaitForSeconds(dStayTime);

            StartDialogue("How about I introduce myself properly?");
            yield return new WaitForSeconds(dStayTime + 2);
            StartDialogue("I am <color=purple>CHAOS</color>... The flame of ambition...");
            yield return new WaitForSeconds(dStayTime + 2);
            StartDialogue("<color=#00FFD7>" + userName + "</color>, was it? Hehe...");
            yield return new WaitForSeconds(dStayTime);
            StartDialogue("You're a truly peculiar one.");
            yield return new WaitForSeconds(dStayTime);
            StartDialogue("But can you overcome the trial before you...?");
            yield return new WaitForSeconds(dStayTime);
            StartDialogue("Only time will tell... Hehehe...");
            yield return new WaitForSeconds(dStayTime);
            StartDialogue("Before we start... Let's even the odds a bit, shall we?");
            yield return new WaitForSeconds(dStayTime);
            GameManager.Instance.SendNotification("You have been <color=purple>HYPERCHARGED</color>!");
            rFire.SetActive(true);
            p.speed *= 2.5f;
            p.maxDashCooldown = 0.25f;
            p.dashForce *= 1.4f;
        p.anim.SetTrigger("TriggerDM");
            StartDialogue("I believe this will make things a little more fair, eh?");
            yield return new WaitForSeconds(dStayTime);
            if (GameManager.Instance.expertMode)
            {
                StartDialogue("Oh, I see you're playing on expert mode... Let me disable that for you.");
                GameManager.Instance.expertMode = false;
                yield return new WaitForSeconds(dStayTime);
                StartDialogue("Alright, enough waiting.");
                yield return new WaitForSeconds(dStayTime);

            }
            

        StartCoroutine(ConductBattle());
        yield return null;
    }
    public void StartSpecial(Sprite s)
    {
        specialPortrait.sprite = s;
        specialAnim.SetTrigger("Attack");
    }
    IEnumerator MoveTo(Vector3 t, bool shoot)
    {
        float dist = 1000f;
        while (Mathf.Abs(dist) > 0.2f)
        {
            transform.position = Vector3.Lerp(transform.position, t, Time.deltaTime*2);
            dist = (transform.position - t).magnitude;
            yield return null;
        }
        if (shoot)
        {
            GameObject s = Instantiate(spinSlash, transform.position + transform.forward, Quaternion.identity);
            Destroy(s, 6);
        }
        yield return new WaitForSeconds(Random.Range(1, 5));
        if (canMove)
            MoveAround();

        yield return null;
    }
    void MoveAround()
    {
        int rnd = Random.Range(0, 4);
        float initChance = Random.Range(0, 100);
        Vector2 posToMoveTo = (Vector2)p.transform.position + new Vector2(Random.Range(-moveRange, moveRange), Random.Range(-moveRange, moveRange));

        if (initChance < 30)
        {
            StartCoroutine(MoveTo(moveLocations[0].position,true));
            laugh.Play();
        }
        else
        {
            StartCoroutine(MoveTo(posToMoveTo,false));
        }
        

    }
    IEnumerator ConductBattle()
    {
        if (!skipToFinalAttack)
        {
            StartDialogue("Now... Let's do this!");
            yield return new WaitForSeconds(dStayTime);
            flash.SetTrigger("Flash");
            laugh.Play();
            aus.Play();
            Camera.main.GetComponent<AudioSource>().Play();
            p2Music.Play();
            MoveAround();
            yield return new WaitForSeconds(0.5f);
            invincible = false;
            hpBarAnim.gameObject.SetActive(true);
            shooters1.SetActive(true);
            yield return new WaitForSeconds(dStayTime);
            StartDialogue("You've seen these attacks already, I'm sure they'll be easy to dodge for you...");
            yield return new WaitForSeconds(dStayTime);
            StartDialogue("Oh, and just so you know... <color=red>If you die, I'll heal myself.</color>");
            yield return new WaitForSeconds(dStayTime * 3);
            StartDialogue("Hehe... It seems an old friend of mine wants to see you. 6 of them, to be precise.");
            yield return new WaitForSeconds(dStayTime);
            StartCoroutine(SpawnSpellslingers());
            StartDialogue("They're real good at slinging spells.");
            yield return new WaitForSeconds(dStayTime);
            StartDialogue("Oh, and it appears that...");
            yield return new WaitForSeconds(dStayTime);
            StartSpecial(bucketheadSprite);
            GameObject bh = Instantiate(bucketHead, moveLocations[4].position, Quaternion.identity);
            Destroy(bh, 45);
            StartDialogue("From beyond time and space... a hero has been summoned to my side!");
            yield return new WaitForSeconds(dStayTime);
            StartDialogue("Hahahaaa!");
            yield return new WaitForSeconds(dStayTime);
            StartDialogue("I'm having a lot of fun, personally. What about you?");
            yield return new WaitForSeconds(dStayTime);
            StartSpecial(bunnyCharacter);
            GameObject b = Instantiate(bunny, p.transform.position, Quaternion.identity);

            StartDialogue("Who... Who the hell is that...?");
            yield return new WaitForSeconds(dStayTime);
            BunnyDialogue("Uhhh... Where am I?");
            yield return new WaitForSeconds(dStayTime);
            StartDialogue("...I don't think I was trying to summon you.");
            yield return new WaitForSeconds(dStayTime);
            BunnyDialogue("Err... Can you please put me back?");
            yield return new WaitForSeconds(dStayTime);
            StartDialogue("Sure, give me a second...");
            yield return new WaitForSeconds(dStayTime);
            BunnyDialogue("...");
            yield return new WaitForSeconds(dStayTime * 2);
            Instantiate(circleAttack, b.transform.position, Quaternion.identity);
            Destroy(b, 1);
            StartDialogue("...Anyways.");
            bozoTime = true;
            yield return new WaitForSeconds(dStayTime);
            StartDialogue("Hold on... Someone is...");
            yield return new WaitForSeconds(dStayTime);
            StartDialogue("Someone is opening a rift in reality...?");
            yield return new WaitForSeconds(dStayTime);
            StartSpecial(bozoSprite);
            Instantiate(bozo, moveLocations[0].position, Quaternion.identity);
            yield return new WaitForSeconds(dStayTime * 2);
            StartDialogue("Just kidding I summoned that guy");
            yield return new WaitForSeconds(dStayTime * 2);
            bozoTime = false;
            StartCoroutine(SpawnSpellslingers());

            while (curHp > 0)
            {
                yield return null;


            }
            bozoTime = true;
            StartDialogue("Aw... I'm already at 0? But I don't wanna stop just yet...");
            yield return new WaitForSeconds(dStayTime);
            StartDialogue("Surely you won't mind if I give myself a little boost...");
            yield return new WaitForSeconds(dStayTime);
            GameManager.Instance.SendNotification("Chaos has healed itself!");
            curHp = maxHp;
            hpBar.fillAmount = curHp / maxHp;
            StartDialogue("What? You do mind? Well, TOO BAD, because...");
            yield return new WaitForSeconds(dStayTime);

            StartDialogue("I haven't even used my <color=red>ultimate attack</color> yet...");
            yield return new WaitForSeconds(dStayTime);
            StartDialogue("Are you ready...?");
            Destroy(shooters1);
            canMove = false;
            yield return new WaitForSeconds(dStayTime);
            StartCoroutine(MoveTo(moveLocations[0].position, false));
            yield return new WaitForSeconds(dStayTime);
            StartDialogue("Hold on I'm still preparing it");
            yield return new WaitForSeconds(dStayTime);
            StartDialogue("Okay there we go");

            hpBar.gameObject.SetActive(false);
            GetComponent<Collider2D>().enabled = false;
            yield return new WaitForSeconds(dStayTime);
            flash.SetTrigger("Flash");
            aus.Play();
            ss2.OnShoot();
            laugh.Play();
            yield return new WaitForSeconds(dStayTime);
            StartDialogue("Let's see you dodge this one!");
            for (int i = 0; i < 50; i++)
            {
                ss2.OnShoot();
                yield return new WaitForSeconds(0.1f);
                if (i == 25)
                {
                    StartSpecial(bucketheadSprite);
                    GameObject bh2 = Instantiate(bucketHead, moveLocations[4].position, Quaternion.identity);
                    Destroy(bh2, 45);
                }
                yield return null;
            }
            ss3.OnShoot();
            yield return new WaitForSeconds(dStayTime);
            StartDialogue("Hehe... Do you remember this attack, perhaps?");
            Instantiate(otherUlt, transform.position, Quaternion.identity);
            yield return new WaitForSeconds(dStayTime);
            StartDialogue("I'm quite a big fan of it... wait, hold on...");
            yield return new WaitForSeconds(dStayTime);
            StartDialogue("Dang it... This arena's too big for that attack... I didn't think of that.");
            yield return new WaitForSeconds(dStayTime);
            StartDialogue("Ugh... Gotta make it harder for you then.");
            bozoTime = false;
            StartCoroutine(SpawnSpellslingers());
            yield return new WaitForSeconds(dStayTime * 1.5f);
            ss3.canShoot = false;
            StartDialogue("I think you can take a bit more pressure.");
            ss4.OnShoot();
            yield return new WaitForSeconds(dStayTime);
            StartDialogue("Hehehe...");
            yield return new WaitForSeconds(dStayTime * 2);
            StartDialogue("Now this is fun!");
            yield return new WaitForSeconds(dStayTime * 2);
            StartDialogue("Hehe...");
            yield return new WaitForSeconds(dStayTime * 2);

            ss4.canShoot = false;
            bozoTime = true;
            StartDialogue("Well...");
            yield return new WaitForSeconds(dStayTime * 2);
            StartDialogue("We've had a lovely time together, haven't we?");
            yield return new WaitForSeconds(dStayTime);

            StartDialogue("But all things must come to an end, I suppose.");
            yield return new WaitForSeconds(dStayTime);
            StartDialogue("I'm kinda out of time. Got somewhere to be.");
            yield return new WaitForSeconds(dStayTime);
            StartDialogue("So...");
            yield return new WaitForSeconds(dStayTime);
        }
        else
        {
            Camera.main.GetComponent<AudioSource>().Play();
            p2Music.Play();
        }
        StartDialogue("I say it's time for me to end this fight with something... exciting.");
        yield return new WaitForSeconds(dStayTime);
        flash.SetTrigger("Flash");
        laugh.Play();
        aus.Play();
        Camera.main.GetComponent<AudioSource>().mute = true;
        p2Music.mute = false;
        foreach (GameObject go in visuals)
        {
            go.SetActive(false);
        }
        invincible = true;
        finalPhase = true;
        finalPhaseGO.SetActive(true);
        Camera.main.GetComponent<CamScript>().player = finalPhaseGO.transform;
        hpBar.gameObject.SetActive(true);
        yield return new WaitForSeconds(dStayTime);
        StartDialogue("This will be fun!");
        Camera.main.GetComponent<CamScript>().player = p.camTarget.transform.GetChild(0).transform;
        f.SpawnHandsVoid();
        yield return new WaitForSeconds(dStayTime);
        f.SpawnDancingSwords();
        yield return new WaitForSeconds(6);
        bozoTime = false;
        StartDialogue("I'm bringing back our funny friends.");
        yield return new WaitForSeconds(dStayTime);
        StartCoroutine(SpawnSpellslingers());
        StartDialogue("Hehehe...");
        yield return new WaitForSeconds(dStayTime);
        yield return new WaitForSeconds(dStayTime*2);
        StartDialogue("Just imagine if I used my full power. ha ha...");
        yield return new WaitForSeconds(dStayTime);
        StartDialogue("What? Of course I'm holding back, what did you think?");
        yield return new WaitForSeconds(dStayTime);
        StartDialogue("Hehe, I'm just having fun.");
        yield return new WaitForSeconds(dStayTime);
        StartDialogue("Like a cat toying with its prey...");
        yield return new WaitForSeconds(dStayTime);
        StartDialogue("...");
        yield return new WaitForSeconds(dStayTime);
        StartDialogue("Well, anyways.");
        yield return new WaitForSeconds(dStayTime);
        StartDialogue("Keep doing your thing.");
        yield return new WaitForSeconds(dStayTime);
        while (f.curHp > 0)
        {

            yield return null;
        }
        flash.SetTrigger("Flash");
        aus.Play();
        Camera.main.GetComponent<AudioSource>().Stop();
        p2Music.mute = true;
        p2Music.Stop();
        Destroy(f.gameObject);
        foreach (GameObject go in visuals)
        {
            go.SetActive(true);
        }
        p.dashBar.gameObject.SetActive(false);
        p.dashCooldown = 99999;
        p.invincible = true;
        hpBarAnim.gameObject.SetActive(false);
        bozoTime = true;
        Camera.main.GetComponent<CamScript>().player = transform;
        StartDialogue("Oh, wow!");
        yield return new WaitForSeconds(dStayTime);
        p.invincible = true;
        StartDialogue("Hehehe... Congrats.");
        yield return new WaitForSeconds(dStayTime);
        p.invincible = true;
        StartDialogue("You beat me, fair and square.");
        yield return new WaitForSeconds(dStayTime);
        p.invincible = true;
        StartDialogue("You're pretty cool, <color=#00FFD7>" + userName + "</color>.");
        yield return new WaitForSeconds(dStayTime);
        StartDialogue("I hope that someday, you and I can meet again...");
        yield return new WaitForSeconds(dStayTime);
        StartDialogue("Hehehe...");
        yield return new WaitForSeconds(dStayTime);
        StartDialogue("But for now, this is goodbye.");
        yield return new WaitForSeconds(dStayTime);
        StartDialogue("Enjoy your ending. You've truly earned it.");
        yield return new WaitForSeconds(dStayTime);
        StartDialogue("Goodbye, my friend!");
        yield return new WaitForSeconds(dStayTime*2);
        flash.SetTrigger("Flash");
        aus.Play();
        SceneManager.LoadScene("TrueEnding");
        yield return null;
    }

    IEnumerator DamageEffect()
    {
        sr.color = Color.red;
        yield return new WaitForSeconds(0.15f);
        sr.color = Color.white;

        yield return null;
    }
    public void TakeDamage(float amount)
    {
        hit.pitch = Random.Range(0.85f,1);
        hit.Play();
        curHp -= amount;
        if (curHp < 0)
        {
            curHp = 0;
        }
        hpBar.fillAmount = curHp / maxHp;
        hpBarAnim.SetTrigger("Hit");
        StartCoroutine(DamageEffect());
    }
    public void Heal()
    {
        laugh.Play();
        if (hpBar.gameObject.activeInHierarchy)
            GameManager.Instance.SendNotification("Chaos has healed itself!");
        curHp += healAmt;
        if (curHp > maxHp)
        {
            curHp = maxHp;
        }
        hpBar.fillAmount = curHp / maxHp;

        f.Heal(35);
    }

    public void StartDialogue(string _dialogue)
    {
        dPortrait.sprite = bossSprite;
        dialogue.text = _dialogue;
        dAnim.SetTrigger("Dialogue");
        if (dCo != null)
        {
            dCo = null;
        }
        dCo = StartCoroutine(Typewriter());
    }
    public void BunnyDialogue(string _dialogue)
    {
        dPortrait.sprite = bunnySprite;
        dialogue.text = _dialogue;
        dAnim.SetTrigger("Dialogue");
        if (dCo != null)
        {
            dCo = null;
        }
        dCo = StartCoroutine(Typewriter());
    }
    IEnumerator Typewriter()
    {
        dialogue.maxVisibleCharacters = 0;
        while (dialogue.maxVisibleCharacters < dialogue.text.Length) {
            dialogue.maxVisibleCharacters++;
            
            speak.pitch = Random.Range(0.8f, 1.1f);
            speak.Play();

            yield return new WaitForSeconds(dTime);
            speak.Stop();
            yield return null;
        }
        dialogue.maxVisibleCharacters = dialogue.text.Length;
        yield return null;
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
            GameObject bs = Instantiate(GameManager.Instance.bSplatter, transform.position + Vector3.forward, lDir);

            bs.transform.position = (Vector2)transform.position + dir.normalized;
            if (collision.GetComponent<SwordScript>().isSlashing)
            {
                
                TakeDamage(3 + GameManager.Instance.dmgUpgrades*0.5f);

            }
            else
            {
                TakeDamage(1 + GameManager.Instance.dmgUpgrades * 0.15f);
                if (sword.curTarget == sword.playerChar || sword.curTarget == sword.playerChar.gameObject.GetComponent<Player>().orbiter) { sword.curTarget = this.gameObject.transform; sword.Idling = false; sword.SIforNPC(); }
            }


        }
    }


}
