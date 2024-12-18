using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleScreenManager : MonoBehaviour
{
    public List<Sprite> tutorialSprites = new List<Sprite>();
    public List<string> tutorialNames = new List<string>();
    public List<string> descriptions = new List<string>();
    public int curTutorial = 0;

    public GameObject tWindow;
    public Image tutorialPic;
    public TextMeshProUGUI tutorialName;
    public TextMeshProUGUI tutorialDesc;
    void Start()
    {
        LoadTutorial();
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

    public void toggleTWindow()
    {
        curTutorial = 0;
        tWindow.SetActive(!tWindow.activeSelf);
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

    public void PlayGame()
    {
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
}
