using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverScreen : MonoBehaviour
{
    public float loadTime;
    void Start()
    {
        Invoke("Next", loadTime);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Next();
        }
    }

    void Next()
    {
        SceneManager.LoadScene("TitleScreen");
    }
}
