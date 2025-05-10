using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TheFinalBoss : MonoBehaviour
{
    public Transform player;
    public Animator dAnim;
    public TextMeshProUGUI dialogue;
    bool canDie = false;
    public float maxHealth;
    public float curHealth;
    public SpriteRenderer sr;
    public Animator hpBarAnim;
    public Image hpBar;
    Animator anim;
    int dmgInc = 0;
    public TextMeshProUGUI notif;
    public Animator notifAnim;
    bool charged = false;
    public ParticleSystem hit;
    bool dead = false;
    public Animator endingFlash;
    public GameObject projectile;
    public Transform shot;
    public GameObject canvas;
    void Start()
    {
        anim = GetComponent<Animator>();
        curHealth = maxHealth;
        player = GameObject.Find("Player").transform;
        
        StartCoroutine(Dialogue());
    }

    // Update is called once per frame
    void Update()
    {
        if (!dead)
            transform.LookAt(player);
    }
    public void Dial(string msg)
    {
        dmgInc++;
        dialogue.text = msg;
        dAnim.SetTrigger("Dialogue");
    }
    public void Shoot()
    {
        Instantiate(projectile, shot.position, Quaternion.identity);
        Instantiate(projectile, shot.position, Quaternion.identity);
        Instantiate(projectile, shot.position, Quaternion.identity);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Sword"))
        {
            StartCoroutine(OnDmgTaken());
            hpBar.rectTransform.sizeDelta = new Vector2(3600 * (curHealth / maxHealth), 200);
            curHealth-=dmgInc;
            hpBarAnim.SetTrigger("Hit");
            if (curHealth <= 0)
            {
                hpBar.rectTransform.sizeDelta = new Vector2(0, 200);
                curHealth = 0;
                if (canDie)
                {
                    
                    dead = true;
                    canvas.SetActive(false);
                    GetComponent<Collider>().enabled = false;
                    transform.rotation = Quaternion.Euler(Vector3.zero);
                    anim.SetTrigger("Death");
                    endingFlash.SetTrigger("End");
                    Invoke("GoToEnding", 10);
                    Dial("UAAAARRGHH.... I am... fading...");

                }
            }
        }
    }
    public void GoToEnding()
    {
        PlayerPrefs.SetInt("EMBeaten", 1);
        SceneManager.LoadScene("GoodEnding");
    }
    IEnumerator OnDmgTaken()
    {
        if (charged)
        {
            hit.Play();
        }
        sr.color = Color.red;
        yield return new WaitForSeconds(0.15f);
        sr.color = Color.white;
    }
    IEnumerator SwordAttack()
    {
        anim.SetTrigger("SwordAttack");
        yield return new WaitForSeconds(8);
        StartCoroutine(SwordAttack());
        yield return null;
    }
    IEnumerator MagAttack()
    {
        anim.SetTrigger("MagAttack");
        yield return new WaitForSeconds(10);
        StartCoroutine(MagAttack());
        yield return null;
    }
    void Supercharge()
    {
        dmgInc += 35;
        notif.text = "Newfound power surges within you!";
        notifAnim.SetTrigger("Message");
        charged = true;
    }
    IEnumerator Dialogue()
    {
        Dial("Ungh... What... what is this...?");
        yield return new WaitForSeconds(9);
        StartCoroutine(SwordAttack());
        Dial("But I... you killed me...?");
        yield return new WaitForSeconds(7);
        StartCoroutine(MagAttack());
        Dial("I know not what this is...");
        yield return new WaitForSeconds(8);
        Dial("My body moves on its own...");
        yield return new WaitForSeconds(8);
        Dial("Come, hero... Let us settle this...");
        yield return new WaitForSeconds(8);
        Dial("Once... and for all!");
        yield return new WaitForSeconds(8);
        Dial("Show me... humanity's will to survive!");
        yield return new WaitForSeconds(8);
        Dial("Yes... I understand now...");
        yield return new WaitForSeconds(8);
        Dial("Your kind is a resilient one...");
        yield return new WaitForSeconds(8);
        Dial("Ahahaha... Ahahahahaaaahhh!");
        yield return new WaitForSeconds(8);
        Dial("Hero... You will defeat me...");
        yield return new WaitForSeconds(8);
        Dial("And yet, I feel... content.");
        yield return new WaitForSeconds(8);
        Dial("Hahaha... What a strange feeling.");
        yield return new WaitForSeconds(8);
        Dial("For the first time, in a million years...");
        yield return new WaitForSeconds(8);
        Dial("I am... satisfied...");
        yield return new WaitForSeconds(8);
        Dial("Hahahaha... Ahahahahahaaa!!");
        yield return new WaitForSeconds(8);
        Dial("Oh, this is incredible...");
        yield return new WaitForSeconds(8);
        Dial("You've truly earned your title, hero...");
        yield return new WaitForSeconds(8);
        Dial("Now... Finish this...!");
        yield return new WaitForSeconds(8);
        Dial("Return me to the abyss from whence I came...!");
        yield return new WaitForSeconds(3);
        Supercharge();
        yield return new WaitForSeconds(5);
        Dial("Your name... shall be remembered... For eons!");
        yield return new WaitForSeconds(8);
        Dial("Aaahahahhaahaaaaaaahhh!!");
        yield return new WaitForSeconds(4);

        canDie = true;
        yield return null;
    }
}
