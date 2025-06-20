using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverScreen : MonoBehaviour
{
    public float loadTime;
    public bool saveFunnyStats;
    void Start()
    {
        Invoke("Next", loadTime);
        if (saveFunnyStats )
        {
            PlayerPrefs.SetInt("Deaths", 99999);
            PlayerPrefs.SetInt("Kills", 99999);
            PlayerPrefs.SetInt("NPCKills", -2);
            PlayerPrefs.SetFloat("Corruption", 999);
            PlayerPrefs.SetFloat("TotalCorr", 9999);
            PlayerPrefs.SetFloat("TimeTaken", 0);
           // PlayerPrefs.SetInt("FinalSecret", 1);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Next();
        }
    }

    void Next()
    {
        SceneManager.LoadScene("Credits");
    }
}
