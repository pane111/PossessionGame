using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoaderScript : MonoBehaviour
{
    public string scene;
    bool canSkip=false;
    public GameObject boonTxt;
    private void Start()
    {
        if (boonTxt != null)
        {
            if (PlayerPrefs.GetInt("HadBoons")==1)
            {
                boonTxt.SetActive(true);
            }
        }
        canSkip = PlayerPrefs.HasKey("CanSkipCredits");
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (canSkip)
            {
                LoadTheScene();
            }
        }
    }

    public void LoadTheScene()
    {
        SceneManager.LoadScene(scene);
    }
}
