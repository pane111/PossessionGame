using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public Gradient colorGradient;
    public GameObject bSplatter;
    public Animator deathAnim;
    public Animator heartAnim;
    public float bpm = 1;

    public int kills = 0;

    public TextMeshProUGUI statsText;
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
        if (Input.GetKeyDown(KeyCode.F6))
        {
            SetBPM(2);
        }
    }

    public void AddKill()
    {
        kills++;
        statsText.text="time\n99:99\nkills\n"+kills.ToString();
    }
    public void SetBPM(float _bpm)
    {
        bpm = _bpm;
        heartAnim.SetFloat("BPM", bpm);
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
