using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoaderScript : MonoBehaviour
{
    public string scene;
    public void LoadTheScene()
    {
        SceneManager.LoadScene(scene);
    }
}
