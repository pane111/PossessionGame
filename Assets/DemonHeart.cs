using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.ShaderGraph;
using UnityEngine;

public class DemonHeart : MonoBehaviour
{
    public CursedWeapon weapon;
    public bool heartExposed = false;
    [Header("Visuals")]
    public GameObject crystal;
    public ParticleSystem crystalHit;
    public GameObject hitEffect;
    public ParticleSystem blood;
    public ParticleSystem bloodSpray;
    public GameObject bloodstain;
    SpriteRenderer sr;
    SwordScript sword;
    Deflector d;
    Color c = Color.black;

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
        d.deflectionActive = false;
        heartExposed = true;
        crystal.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.name == "PlayerSword")
        {
            if (!heartExposed)
            {
                AudioManager.Instance.CrystalHit.Post(gameObject);
            }
            else
            {
                AudioManager.Instance.NPCHeartHit.Post(gameObject);
                GetComponent<Collider2D>().enabled = false;
                sr.enabled = false;

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
    }
}
