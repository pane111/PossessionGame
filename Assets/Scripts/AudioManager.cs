using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AK.Wwise;
using Event = AK.Wwise.Event;

public class AudioManager : MonoBehaviour
{
    [Range(0, 15f)] public int masterVolume;
    public float bpm;
    private float bpmMod;

    [Header("States")] //game over not connected, paused doesnt exist
    public State GS_Titlescreen;
    public State GS_NormalMode;
    public State GS_Paused;
    [SerializeField] private State GS_DemonMode;
    public State GS_GameOver;

    [Header("RTPC")] //connected!
    public RTPC possessionRTPC;
    public RTPC healthRTPC;
    public RTPC volume;

    [Header("Events")] //nothing connected except demonMode and startMusic
    public Event SwordSlash;
    public Event SwordBigSlash;
    public Event SwordDeflect;
    public Event SwordDetect;
    public Event Pull;
    public Event Repell;
    public Event PlayerDmgTaken;
    public Event Dash;
    public Event Player_Footstep;
    public Event NPC_Footstep;
    public Event Death;
    public Event GameOver;
    public Event Revive;
    public Event NPCDeath;
    public Event NPCDeathHEART;
    public Event NPCTakeDmg;
    public Event CrystalHit;
    public Event CrystalBroken;
    public Event Purify;
    public Event OrbShotBig;
    public Event OrbShotSmall;
    public Event NPCHeartHit;
    public Event LaserStart;
    public Event LaserStop;
    [SerializeField] private Event DemonModeOn;
    [SerializeField] private Event DemonModeOff;
    public Event StartTicking;
    public Event StartMusic;
    public Event Pause;
    public Event Resume;
    public Event ButtonClick;

    private void Start()
    {
        healthRTPC.SetGlobalValue(100);
        possessionRTPC.SetGlobalValue(0);
        GS_Titlescreen.SetValue();
        StartMusic.Post(gameObject);
        bpmMod = bpm / 60;
    }

    private void Update()
    {
        volume.SetGlobalValue(masterVolume);
    }

    #region Methods
    public void EnterDemonMode()
    {
        DemonModeOn.Post(gameObject);
        GS_DemonMode.SetValue();
    }
    public void ExitDemonMode()
    {
        DemonModeOff.Post(gameObject);
        GS_NormalMode.SetValue();
    }
    public void Footstep_Player()
    {
        Player_Footstep.Post(gameObject);
    }
    public void Footstep_NPC(GameObject go)
    {
        NPC_Footstep.Post(go);
    }
    public void ButtonPress()
    {
        ButtonClick.Post(gameObject);
    }
    #endregion

    #region Singleton
    public static AudioManager Instance { get; private set; }
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
    #endregion
}
