using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using AK.Wwise;
using Event = AK.Wwise.Event;
using UnityEngine.EventSystems;

public class TitleScreenManager : MonoBehaviour
{
    public List<Sprite> tutorialSprites = new List<Sprite>();
    public List<string> tutorialNames = new List<string>();
    [TextArea(6, 10)]
    public List<string> descriptions = new List<string>();



    [TextArea(6, 10)]
    public List<string> logs = new List<string>();

    public int curTutorial = 0;
    public int curLog = 0;

    public TextMeshProUGUI logText;
    public GameObject lWindow;

    public GameObject tWindow;
    public Image tutorialPic;
    public TextMeshProUGUI tutorialName;
    public TextMeshProUGUI tutorialDesc;
    public GameObject bonus;
    public Event ButtonClick;
    public GameObject bonus2;
    public Toggle tutorials;

    public GameObject firstSelected;
    public GameObject emButton;
    private void Awake()
    {
        if (!PlayerPrefs.HasKey("Tutorials"))
        {
            PlayerPrefs.SetInt("Tutorials", 1);
        }
        else
        {
            if (PlayerPrefs.GetInt("Tutorials") == 0)
            {
                tutorials.isOn = false;
            }
            else
            {
                tutorials.isOn = true;
            }
        }
    }
    void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        EventSystem.current.SetSelectedGameObject(firstSelected);

        
        
        LoadTutorial();
        if (!PlayerPrefs.HasKey("GameBeaten"))
        {
            PlayerPrefs.SetInt("GameBeaten", 0);
            
        }
        if (PlayerPrefs.HasKey("EMBeaten"))
        {
            bonus2.SetActive(true);
        }

        if (PlayerPrefs.GetInt("GameBeaten")==1)
        {
            bonus.SetActive(true);
            PlayerPrefs.SetInt("CanSkipCredits", 1);
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void LoadTutorial()
    {
        tutorialPic.sprite = tutorialSprites[curTutorial];
        tutorialName.text = tutorialNames[curTutorial].ToString();
        tutorialDesc.text = descriptions[curTutorial].ToString();
    }

    void LoadLog()
    {
        logText.text = "Research Log " + curLog + "\n"+logs[curLog];
    }

    public void toggleTWindow()
    {
        curTutorial = 0;
        tWindow.SetActive(!tWindow.activeSelf);
        if (!tWindow.activeInHierarchy)
        {
            EventSystem.current.SetSelectedGameObject(firstSelected);
        }
    }
    public void toggleLWindow()
    {
        curLog = 0;
        lWindow.SetActive(!lWindow.activeSelf);
        if (!lWindow.activeInHierarchy)
        {
            EventSystem.current.SetSelectedGameObject(firstSelected);
        }
    }

    public void NextTutorial()
    {
        if (curTutorial < tutorialSprites.Count-1)
        {
            curTutorial++;
        }
        else
        {
            toggleTWindow();
        }
        LoadTutorial() ;
    }
    public void NextLog()
    {
        if (curLog < logs.Count - 1)
        {
            curLog++;
        }
        else
        {
            toggleLWindow();
        }
        LoadLog();
    }
    public void TutToggle(bool toggle)
    {
        if (toggle)
        {
            PlayerPrefs.SetInt("Tutorials", 1);
        }
        else
        {
            PlayerPrefs.SetInt("Tutorials", 0);
        }
    }
    public void PrevTutorial()
    {
        if (curTutorial > 0)
        {
            curTutorial--;
        }
        else
        {
            toggleTWindow();
        }
        LoadTutorial();
    }
    public void PrevLog()
    {
        if (curLog > 0)
        {
            curLog--;
        }
        else
        {
            toggleLWindow();
        }
        LoadLog();
    }
    public void SelectDefaultButton()
    {
        EventSystem.current.SetSelectedGameObject(firstSelected);
    }
    public void SelectEMBtn()
    {
        EventSystem.current.SetSelectedGameObject(emButton);
    }
    public void PlayGame()
    {
        PlayerPrefs.SetInt("EM", 0);
        SceneManager.LoadScene("SampleScene");
    }
    public void Review()
    {
        SceneManager.LoadScene("Results");
    }
    public void PlayGameEM()
    {
        PlayerPrefs.SetInt("EM", 1);
        SceneManager.LoadScene("SampleScene");
    }
    public void StopGame()
    {
        Application.Quit();
    }
    public void Help()
    {
        toggleTWindow();
    }

    public void ButtonPress()
    {
        ButtonClick.Post(gameObject);
    }
}
