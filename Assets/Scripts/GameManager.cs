using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Tilemaps;
using UnityEngine.SceneManagement;
using System;

public class GameManager : MonoBehaviour
{
    public Gradient colorGradient;
    public Gradient NpcGradient;
    public GameObject bSplatter;
    public Animator deathAnim;
    public Animator heartAnim;
    public float bpm = 1;
    public Player player;
    public int kills = 0;
    public int saved = 0;
    public TextMeshProUGUI statsText;
    Color bgColor;
    public Color demonModeColor;
    public GameObject deathMenu;
    [Header("Sprites")]
    public float usedAbilityOpacity; //not connected rn
    public Image character;
    public Image armor;
    public Sprite cSprite;
    public Sprite cDSprite;
    public Image dArmor;
    public List<Sprite> npcSprites = new List<Sprite>();

    [Header("Tiles")]
    public Tilemap floor;
    public Tilemap wall;



    public float storedTS=1;
    
    public Action startDM;
    public Action stopDM;
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
        bgColor = Camera.main.backgroundColor;
        player = FindObjectOfType<Player>();
        OnDemonModeExit();
    }

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
        player.Corruption += 5;
        statsText.text="time\n99:99\nkills\n"+kills.ToString();
    }
    public void SetBPM(float _bpm)
    {
        bpm = _bpm;
        heartAnim.SetFloat("BPM", bpm);
    }

    public Color randomColor()
    {
        float rnd = UnityEngine.Random.Range(0f, 1f);
        Color c = colorGradient.Evaluate(rnd);

        return c;
    }
    public Color randomNpcColor()
    {
        float rnd = UnityEngine.Random.Range(0f, 1f);
        Color c = NpcGradient.Evaluate(rnd);

        return c;
    }
    public Sprite randomSprite()
    {
        int r = UnityEngine.Random.Range(0,npcSprites.Count);


        return npcSprites[r];
    }

    public void OnDeath()
    {
        
        AudioManager.Instance.GS_GameOver.SetValue();
        AudioManager.Instance.Death.Post(gameObject);
        deathAnim.SetTrigger("Death");
        Time.timeScale = 0;
        deathMenu.SetActive(true);
    }

    public void OnDemonModeEnter()
    {
        startDM.Invoke();
        AudioManager.Instance.EnterDemonMode();
        character.sprite = cDSprite;
        armor.enabled = false;
        dArmor.enabled = true;
        Camera.main.backgroundColor = demonModeColor;
        InvertColor();
        
    }
    public void OnDemonModeExit()
    {
        stopDM.Invoke();
        AudioManager.Instance.ExitDemonMode();
        character.sprite = cSprite;
        armor.enabled = true;
        dArmor.enabled = false;
        Camera.main.backgroundColor = bgColor;
        InvertColor();
    }

    public void Continue()
    {
        
        Time.timeScale = storedTS;
        deathMenu.SetActive(false);
        player.Revive();
    }
    public void GiveUp()
    {
        AudioManager.Instance.GameOver.Post(gameObject);
        Time.timeScale = 1;
        Time.fixedDeltaTime = 0.02F * Time.timeScale;
        SceneManager.LoadScene("GameOver");
    }

    public void InvertColor()
    {
        Vector3 hsvFloor = Vector3.zero;
        Color.RGBToHSV(floor.color, out hsvFloor.x, out hsvFloor.y, out hsvFloor.z);
        floor.color = Color.HSVToRGB(0,0,100 - hsvFloor.z);

        Vector3 hsvWall = Vector3.zero;
        Color.RGBToHSV(floor.color, out hsvWall.x, out hsvWall.y, out hsvWall.z);
        wall.color = Color.HSVToRGB(0, 0, 100 - hsvWall.z);
    }

    public void TriggerEnding()
    {
        if (player.Corruption >= 100)
        {
            SceneManager.LoadScene("GameOver");
        }
        else
        {
            SceneManager.LoadScene("Ending");
        }
    }
}
