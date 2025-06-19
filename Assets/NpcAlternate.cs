using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NpcAlternate : MonoBehaviour
{
    // Start is called before the first frame update
    public SpriteRenderer sr;
    public GameObject bloodstain;
    public Sprite corpse;
    public List<Sprite> sprites = new List<Sprite>();
    PlayerSimple p;
    bool dead = false;
    public AudioClip kClip;
    public AudioSource killSound;
    void Start()
    {
        killSound.clip = kClip;
        p = FindObjectOfType<PlayerSimple>();
        int rSprite = Random.Range(0, sprites.Count);
        sr.sprite = sprites[rSprite];
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag=="PAura" && !dead)
        {
            killSound.Play();
            sr.sprite = corpse;
            sr.color = Color.red;
            bloodstain.SetActive(true);
            p.ScreenFlash();
            dead = true;
        }
    }

}
