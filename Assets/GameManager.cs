using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Gradient colorGradient;
    public GameObject bSplatter;
    public Animator deathAnim;
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
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Color randomColor()
    {
        float rnd = Random.Range(0f, 1f);
        Color c = colorGradient.Evaluate(rnd);

        return c;
    }

    public void OnDeath()
    {
        deathAnim.SetTrigger("Death");
    }
}
