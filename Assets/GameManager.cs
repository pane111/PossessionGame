using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public Gradient colorGradient;
    public GameObject bSplatter;
    public Animator deathAnim;
    public Animator heartAnim;
    public float bpm = 1;
    public Player player;
    public int kills = 0;
    public int saved = 0;
    public TextMeshProUGUI statsText;

    [Header("Sprites")]
    public float usedAbilityOpacity; //not connected rn
    public Image character;
    public Image armor;
    public Sprite cSprite;
    public Sprite cDSprite;
    public Image dArmor;
    public static GameManager Instance { get; private set; }
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }
    void Start()
    {
        player = FindObjectOfType<Player>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F6))
        {
            SetBPM(2);
        }
    }

    public void AddKill()
    {
        kills++;
        player.corruption += 10;
        statsText.text="time\n99:99\nkills\n"+kills.ToString();
    }
    public void SetBPM(float _bpm)
    {
        bpm = _bpm;
        heartAnim.SetFloat("BPM", bpm);
    }

    public Color randomColor()
    {
        float rnd = Random.Range(0f, 1f);
        Color c = colorGradient.Evaluate(rnd);

        return c;
    }

    public void OnDeath()
    {
        deathAnim.SetTrigger("Death");
    }

    public void OnDemonModeEnter()
    {
        character.sprite = cDSprite;
        armor.enabled = false;
        dArmor.enabled = true;
        
    }
    public void OnDemonModeExit()
    {
        character.sprite = cSprite;
        armor.enabled = true;
        dArmor.enabled = false;
    }
}
