using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoaderScript : MonoBehaviour
{
    public string scene;
    bool canSkip=false;
    private void Start()
    {

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
