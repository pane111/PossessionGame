using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ResultsManager : MonoBehaviour
{
    public TextMeshProUGUI kills;
    public TextMeshProUGUI npckills;
    public TextMeshProUGUI deaths;
    public TextMeshProUGUI totalCorr;
    public TextMeshProUGUI timeTaken;
    public TextMeshProUGUI difficulty;
    void Start()
    {
        kills.text = "Demons Killed - " + PlayerPrefs.GetInt("Kills");
        npckills.text = "Innocents Killed - " + PlayerPrefs.GetInt("NPCKills");
        deaths.text = "Deaths - " + PlayerPrefs.GetInt("Deaths");
        totalCorr.text = "Total Corruption Gained - " + PlayerPrefs.GetFloat("TotalCorr").ToString("F2");
        var ts = TimeSpan.FromSeconds(PlayerPrefs.GetFloat("TimeTaken"));
        timeTaken.text = "Time Taken - " + string.Format("{0:00}:{1:00}", ts.Minutes, ts.Seconds);
        if (PlayerPrefs.GetInt("EM") == 1)
        {
            difficulty.text = "Difficulty - EXPERT";
        }
        else
        {
            difficulty.text = "Difficulty - NORMAL";
        }
    }

    public void Title()
    {
        SceneManager.LoadScene(0);
    }
}
