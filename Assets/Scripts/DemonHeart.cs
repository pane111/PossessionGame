using System.Collections;
using UnityEngine;

public class DemonHeart : MonoBehaviour
{
    public CursedWeapon weapon;
    public bool heartExposed = false;
    [Header("Visuals")]
    public GameObject crystal;
    public ParticleSystem shatter;
    public ParticleSystem crystalHit;
    public GameObject hitEffect;
    public ParticleSystem blood;
    public ParticleSystem bloodSpray;
    public GameObject bloodstain;
    public GameObject lightBeam;
    SpriteRenderer sr;
    SwordScript sword;
    Deflector d;
    bool exploded = false;
    Color c = Color.black;
    int crystalHitAmt = 0;
    public bool dead=false;

    private void Start()
    {
        bloodstain.GetComponent<SpriteRenderer>().color = Color.black;
        bloodstain.SetActive(false);
        crystal.SetActive(true);
        d = GetComponent<Deflector>();
        d.deflectionActive = true;
        sr = GetComponent<SpriteRenderer>();
        sword = FindObjectOfType<SwordScript>();
    }
    public void ExposeHeart()
    {
        if (!exploded)
            shatter.Play();

        exploded = true;
        d.deflectionActive = false;
        heartExposed = true;
        crystal.SetActive(false);
        AudioManager.Instance.CrystalBroken.Post(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.name == "PlayerSword")
        {
            if (!heartExposed)
            {
                AudioManager.Instance.CrystalHit.Post(gameObject);
                crystalHitAmt++;
                if (crystalHitAmt >= 10)
                {
                    GameManager.Instance.PopupTutorial("You cannot break the crystal by attacking it! Find the demon tethered to the crystal and attack it until the crystal breaks!", crystal.GetComponent<SpriteRenderer>().sprite);
                    crystalHitAmt = -100;
                }
            }
            else
            {
                AudioManager.Instance.NPCHeartHit.Post(gameObject);
                GetComponent<Collider2D>().enabled = false;
                sr.enabled = false;
                FindObjectOfType<Player>().OnPurify();
                FindObjectOfType<Player>().kills++;
                weapon.OnDeath();
                dead = true;
                StartCoroutine(HeartHit());
            }
        }
    }

    IEnumerator HeartHit()
    {
        bloodstain.SetActive(true);
        ParticleSystem.MainModule ma = blood.main;
        ma.startColor = c;
        blood.Play();
        hitEffect.SetActive(true);
        yield return new WaitForSeconds(0.05f);
        hitEffect.SetActive(false);
        yield return new WaitForSeconds(1);
        AudioManager.Instance.Purify.Post(gameObject);
        GetComponent<SpriteRenderer>().enabled = false;
        gameObject.layer = 0;
        lightBeam.SetActive(true);
        yield return new WaitForSeconds(1);
        lightBeam.SetActive(false);
    }
}
