using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Tilemaps;
using UnityEngine.SceneManagement;
using System;
using UnityEngine.EventSystems;

public class GameManager : MonoBehaviour
{

    public bool setCorrTo0;

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
    public TextMeshProUGUI popupTutText;
    public Image popupImg;
    public GameObject popupTut;
    Color bgColor;
    public Color demonModeColor;
    public Gradient lineG;
    public Gradient lineG2;
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

    public TextMeshProUGUI notificationText;
    public Animator notif;
    public int cTutorialCount = 3;
    public bool crystalTutorial;


    [Header("Upgrades")]
    public int MSUpgrades;
    public float MSIncrease;
    public int dmgUpgrades;
    public float dmgIncrease;
    public int hpUpgrades;
    public float hpIncrease;
    public int defUpgrades;
    public float defIncrease;
    public TextMeshProUGUI ch1;
    public TextMeshProUGUI ch2;
    int choice1=0;
    int choice2=1;
    public List<string> possibleChoices = new List<string>();

    public bool npctutorial = false;
    public bool displayTutorials=true;
    public bool expertMode=false;

    public CanvasGroup pauseMenu;
    public GameObject pauseSelect;
    bool paused = false;
    public static GameManager Instance { get; private set; }
    private void Awake()
    {
        if (PlayerPrefs.GetInt("EM") == 1)
        {
            expertMode = true;
        }
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
        if (player == null)
            player = FindObjectOfType<Player>();


        if (setCorrTo0)
        {
            player.SetCorruption(0);
            player.ResetData();
            PlayerPrefs.SetInt("HadBoons", 1);
            PlayerPrefs.SetFloat("Corruption", 0);
        }
        if (PlayerPrefs.GetInt("Tutorials")==0)
        {
            displayTutorials = false;
        }

        

    }
    void Start()
    {

        bgColor = Camera.main.backgroundColor;
        

        OnDemonModeExit();
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.LeftAlt))
        {
            if (Input.GetKeyDown(KeyCode.C))
            {
                TriggerEnding();
            }
            if (Input.GetKeyDown(KeyCode.B))
            {
                GoToBossFight();
            }
            if (Input.GetKeyDown(KeyCode.P))
            {
                dmgUpgrades += 100;
            }
            if (Input.GetKeyDown(KeyCode.K))
            {
                player.CurHealth -= 9999;
            }
            if (Input.GetKeyDown(KeyCode.N))
            {
                player.Corruption=0;
            }
            if (Input.GetKeyDown(KeyCode.M))
            {
                player.Corruption += 25;
            }
            if (Input.GetKeyDown(KeyCode.O))
            {
                SceneManager.LoadScene("Outside");
            }
        }

        if (Input.GetButtonDown("Pause"))
        {
            if (paused)
                UnpauseGame();
            else
                PauseGame();
        }
    }

    public void PauseGame()
    {
        pauseMenu.interactable = true;
        pauseMenu.alpha = 1;
        EventSystem.current.SetSelectedGameObject(pauseSelect);
        Time.timeScale = 0;
        paused = true;

    }
    public void UnpauseGame()
    {
        pauseMenu.interactable = false;
        pauseMenu.alpha = 0;
        Time.timeScale = storedTS;
        paused = false;
    }

    public void ReturnToTitle()
    {
        UnpauseGame();
        player.ResetData();
        SceneManager.LoadScene("TitleScreen");
    }
    public void QuickRestart()
    {
        UnpauseGame();
        SceneManager.LoadScene("SampleScene");
    }

    public void SendNotification(string message)
    {
        if (notificationText != null)
        {
            notificationText.text = message;
        }
        else
        {
            notificationText.text = "Error! Message was empty.";
        }

        notif.SetTrigger("Message");

    }

    public void AddKill()
    {
        player.npckills++;
        player.AddCorruptionVoid(UnityEngine.Random.Range(40, 60));
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
        if (!expertMode)
        {
            GetChoices();
            AudioManager.Instance.GS_GameOver.SetValue();
            AudioManager.Instance.Death.Post(gameObject);
            deathAnim.SetTrigger("Death");
            Time.timeScale = 0;
            deathMenu.SetActive(true);
        }
        else
        {
            AudioManager.Instance.Death.Post(gameObject);
            deathAnim.SetTrigger("Death");
            player.Revive();
        }
        
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
        if(stopDM!=null) stopDM.Invoke();
        AudioManager.Instance.ExitDemonMode();
        character.sprite = cSprite;
        armor.enabled = true;
        dArmor.enabled = false;
        Camera.main.backgroundColor = bgColor;
        InvertColor();
    }

    public void GetChoices()
    {
        choice1 = UnityEngine.Random.Range(0,possibleChoices.Count);
        ch1.text = possibleChoices[choice1];

        choice2 = UnityEngine.Random.Range(0, possibleChoices.Count);
        while (choice2==choice1) //We don't want the same choice twice, so we roll until we get a new one
        {
            choice2 = UnityEngine.Random.Range(0, possibleChoices.Count);
        }
        ch2.text= possibleChoices[choice2];



    }
    

    public void PopupTutorial(string t, Sprite s)
    {
        if (displayTutorials)
        {
            popupTutText.text = t;
            popupTut.SetActive(true);
            popupImg.sprite = s;
            Time.timeScale = 0;
            player.canMove = false;
            //Audio pause effect
        }
        
    }
    public void RemovePopup()
    {
        //audio pause effect vorbei
        player.canMove = true;
        popupTut.SetActive(false);
        Time.timeScale = storedTS;
    }

    public void Continue()  //This is CHOICE 2
    {
        getStatUpgrade(choice2);
        
        Time.timeScale = storedTS;
        deathMenu.SetActive(false);
        player.Revive();
    }
    public void GiveUp() //This is CHOICE 1
    {
        getStatUpgrade(choice1);

        Time.timeScale = storedTS;
        deathMenu.SetActive(false);
        player.Revive();
        /*
        AudioManager.Instance.GameOver.Post(gameObject);
        Time.timeScale = 1;
        Time.fixedDeltaTime = 0.02F * Time.timeScale;
        SceneManager.LoadScene("GameOver");
        */
    }

    void getStatUpgrade(int c)
    {
        switch(c)
        {
            case 0:
                MSUpgrades++;
                break;
            case 1:
                dmgUpgrades++;
                break;
            case 2:
                defUpgrades++;
                break;
                case 3:
                hpUpgrades++;
                break;

        }
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
    public void GoToBossFight()
    {
        player.SaveData();
        PlayerPrefs.SetFloat("Corruption", player.Corruption);
        if (defUpgrades > 0 || dmgUpgrades > 0 || hpUpgrades > 0 || MSUpgrades > 0) {
            PlayerPrefs.SetInt("HadBoons", 1);
        }
        else
        {
            PlayerPrefs.SetInt("HadBoons", 0);
        }
        SceneManager.LoadScene("Teleport");
    }

    public void TriggerEnding()
    {
        player.SaveData();
        PlayerPrefs.SetFloat("Corruption", player.Corruption);
        if (player.Corruption >= 100)
        {
            SceneManager.LoadScene("GameOver");
        }
        else
        {
            if(expertMode)
            {
                
                SceneManager.LoadScene("SecretBossTransition");
            }
            else
            {
                SceneManager.LoadScene("Ending");
            }
            
        }
    }
}
