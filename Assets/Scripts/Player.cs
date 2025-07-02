using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    public Rigidbody2D rb;
    public SpriteRenderer sr;
    [Header("Health")]
    public Image healthBar;
    public float maxHealth;
    private float _curHealth;
    public float CurHealth
    {
        get => _curHealth;
        set
        {
            StartCoroutine(onDamageTaken());
            if (value <= 0)
            {
                value = 0;
                GameManager.Instance.OnDeath();
                StartCoroutine(SetInvincible(8));
            }
            AudioManager.Instance.healthRTPC.SetGlobalValue(value);
            _curHealth = value;
        }
    }
    public ParticleSystem blood;
    [Header("Movement")]
    public float speed;
    public float demonSpeed;
    float initSpeed;
    Vector2 moveDirection = Vector2.zero;
    public float maxStepTimer = 0.2f;
    public float stepTimer = 0.2f;
    int stepCounter;
    public int stepSoundCount;
    [Header("Dash")]
    public Image dashImg;
    public float dashForce;
    public float maxDashTimer = 0.2f;
    float dashTimer = 0.2f;
    public float dashCooldown = 0;
    public bool isDashing;
    public float maxDashCooldown = 1;
    public Transform dashBar;
    [Header("Corruption")]
    public float _corruption = 0;
    public float Corruption
    {
        get => _corruption;
        set
        {
            if (value < 0) { value = 0; }
            if (AudioManager.Instance!=null)
            {
                AudioManager.Instance.possessionRTPC.SetGlobalValue(value);
            }
            
            _corruption = value;
        }
    }
    public float corruptionLossOnPurify = 10;
    public Vector2 maxCorrChange;
    public Image corruptionImage;
    public TextMeshProUGUI corruptionText;
    public bool resizeCorrText;
    public Image swordGlow;
    public Image overlay;
    public ParticleSystem afterimage;
    public bool canMove = true;
    public LineRenderer leash;
    public Transform sword;
    private SwordScript swordScript;
    public bool invincible;
    bool canRepell = true;

    public Sprite demonSprite;
    public float demonModeDuration;
    public bool demonModeActive;

    public float orbitSpeed;
    float curDeg=0;
    public Transform rotator;
    public Transform orbiter;

    public float juicedUp=1; //Increase when juiced
    public float juicedMult;
    public float juicedDuration;
    float curJuiceDur;
    bool juiceTutorialTriggered = false;
    public Animator anim;
    float emDmgMult=1;
    public float timeTaken = 0;
    public int kills = 0;
    public int npckills = 0;
    public int deaths = 0;
    public float totalCorr = 0;

    public System.Action onDeath;
    public Animator camTarget;

    bool charging = false;
    float chargeTime = 0;
    public float maxChargeTime=2;
    public float chargeMult = 0.75f;
    void Start()
    {
        if (PlayerPrefs.HasKey("Corruption"))
        {
            Corruption = PlayerPrefs.GetFloat("Corruption");
            LoadData();
            
        }
        if (GameManager.Instance.expertMode)
        {
            emDmgMult = 1.2f;
            demonModeDuration = demonModeDuration * 0.9f;
            maxCorrChange.y = maxCorrChange.y * 1.2f;
            maxCorrChange.x = maxCorrChange.x * 0.95f;
           
        }
        anim = GetComponent<Animator>();
        stepCounter = stepSoundCount;
        _curHealth = maxHealth;
        stepTimer = maxStepTimer;
        swordScript = sword.GetComponent<SwordScript>();
        AudioManager.Instance.GS_NormalMode.SetValue();
        initSpeed = speed;
    }
    public void SaveData()
    {
        print("Saving data - Deaths " + deaths + " Kills " + kills + " NPC kills" + npckills + " TotalCorr " + totalCorr + " Time " + timeTaken);
        PlayerPrefs.SetInt("Deaths", deaths);
        PlayerPrefs.SetInt("Kills", kills);
        PlayerPrefs.SetInt("NPCKills", npckills);
        PlayerPrefs.SetFloat("Corruption", Corruption);
        PlayerPrefs.SetFloat("TotalCorr", totalCorr);
        PlayerPrefs.SetFloat("TimeTaken", timeTaken);

    }
    public void LoadData()
    {
        deaths = PlayerPrefs.GetInt("Deaths");
        kills = PlayerPrefs.GetInt("Kills");
        npckills = PlayerPrefs.GetInt("NPCKills");
        totalCorr = PlayerPrefs.GetInt("TotalCorr");
        timeTaken = PlayerPrefs.GetInt("TimeTaken");
        print("Loaded data - Deaths " + deaths + " Kills " + kills + " NPC kills" + npckills + " TotalCorr " + totalCorr + " Time " + timeTaken);
    }
    public void ResetData()
    {
        print("Reset data");
        PlayerPrefs.SetInt("Deaths", 0);
        PlayerPrefs.SetInt("Kills", 0);
        PlayerPrefs.SetInt("NPCKills", 0);
        PlayerPrefs.SetFloat("TotalCorr", 0);
        PlayerPrefs.SetFloat("TimeTaken", 0);
    }

    // Update is called once per frame
    void Update()
    {
        timeTaken += Time.fixedUnscaledDeltaTime;
        var vertical = Input.GetAxisRaw("Vertical");
        var horizontal = Input.GetAxisRaw("Horizontal");
        curDeg += Time.deltaTime * orbitSpeed;
        rotator.rotation = Quaternion.Euler(0,0,curDeg);
        if (curDeg >= 360)
        {
            curDeg = 0;
        }
        moveDirection.x = horizontal;
        moveDirection.y = vertical;
        if (Input.GetKeyDown(KeyCode.P) && SceneManager.GetActiveScene().name == "SampleScene")
        {
            Unstuck();
        }
        if (moveDirection.magnitude > 0 )
        {

            anim.SetFloat("X", horizontal);
            anim.SetFloat("Y", vertical);
            anim.SetFloat("Speed", 0.2f*juicedUp * speed * (1 + (GameManager.Instance.MSUpgrades * GameManager.Instance.MSIncrease)));

            //stepTimer -= Time.deltaTime;
            //if (stepTimer <= 0 )
            //{
                //if(stepCounter > 0) { stepCounter--; }
                //else if (stepCounter==0) { AudioManager.Instance.Player_Footstep.Post(gameObject); stepCounter = stepSoundCount; }
                //sr.flipX = !sr.flipX;
                //stepTimer = maxStepTimer;
            //}
            if (Input.GetButton("Jump") && canMove && dashCooldown <= 0 && !charging)
            {
                charging = true;

                
                
            }
            if (Input.GetButtonUp("Jump") && charging)
            {
                charging = false;
                if (chargeTime >= 1.5f)
                {
                    StartCoroutine(ChargeAtk(moveDirection.normalized));
                }
                
                AudioManager.Instance.Dash.Post(gameObject);
                dashImg.color = new Color(dashImg.color.r, dashImg.color.g, dashImg.color.b, 0.2f);
                isDashing = true;
                if (chargeTime * chargeMult < 1)
                { chargeTime = 1/chargeMult; }
                rb.AddForce(moveDirection.normalized * dashForce * (chargeMult * chargeTime), ForceMode2D.Impulse);
                chargeTime = 0;
                invincible = true;
                StartCoroutine(SetInvincible(maxDashTimer));
                afterimage.Play();
                dashTimer = maxDashTimer;
                dashCooldown = maxDashCooldown;
            }

        }
        else
        {
            anim.SetFloat("Speed", 0);
        }

        if (Input.GetButtonDown("Fire2") && canMove && canRepell && Vector2.Distance(transform.position, sword.position) < swordScript.repellRange)
        {
            StartCoroutine(Repell());
            swordScript.RepellFunction();
        }

        if (Input.GetButtonDown("Fire3") && canMove)
        {
            StartCoroutine(pullEffect());
            swordScript.Pull();
        }

        Vector3 v = sword.position - transform.position;
        
        leash.SetPosition(1,v);


        if (!isDashing && canMove)
        {
            if (!charging)
            {
                if (!demonModeActive)
                {

                    rb.velocity = moveDirection.normalized * juicedUp * speed * (1 + (GameManager.Instance.MSUpgrades * GameManager.Instance.MSIncrease));
                }
                else
                {
                    rb.velocity = moveDirection.normalized * speed * juicedUp * 1.3f * (1 + (GameManager.Instance.MSUpgrades * GameManager.Instance.MSIncrease * 1.2f));
                }
            }
            else
            {
                chargeTime += Time.deltaTime;
                float barTime = chargeTime;
                if (barTime > 1.5f)
                    barTime = 1.5f;
                if (chargeTime > maxChargeTime)
                {
                    chargeTime = maxChargeTime;
                }

                dashBar.localScale = new Vector3 (barTime / 1.5f, 0.2f, 1);
                rb.velocity = moveDirection.normalized * speed * 0.1f;
                anim.SetFloat("Speed", 0.2f * speed * 0.35f);
            }
            

            dashCooldown -= Time.deltaTime;
            if (dashCooldown < 0)
            { 
                dashCooldown = 0;
                dashImg.color = new Color(dashImg.color.r, dashImg.color.g, dashImg.color.b, 1f);
                if (!charging)
                {
                    dashBar.localScale = new Vector3(0, 0.2f, 1);
                }
                
            }
            else
            {
                dashBar.localScale = new Vector3((dashCooldown / maxDashCooldown), 0.2f, 1);
            }
        }
        else
        {
            dashTimer -= Time.deltaTime;
            if (dashTimer <= 0 )
            {
                afterimage.Stop();
                isDashing = false;
            }
        }

        corruptionText.text = Corruption.ToString("F0") +"%";
        corruptionText.color = Color.Lerp(Color.white, Color.red, Corruption / 100);
        corruptionImage.color = new Color(1, 1, 1, Corruption / 100);
        swordGlow.color = new Color(1, 1, 1, Corruption / 100);

        if (resizeCorrText)
        {
            corruptionText.transform.localScale = Vector3.one * (0.45f + Corruption / 100);
        }
        
            overlay.color = new Color(1, 1, 1, Corruption / 100);

    }
    IEnumerator ChargeAtk(Vector2 dir)
    {
        canRepell = false;
        sword.GetComponent<SwordScript>().controlled = true;
        leash.enabled = true;
        
        Vector3 v = sword.position - transform.position;
        Vector3 sPos = sword.position;
        float elapsedTime = 0;
        while (v.magnitude > 0.1f)
        {
            sword.position = Vector3.Lerp(sPos, transform.position, elapsedTime*8);
            v = sword.position - transform.position;
            elapsedTime+= Time.deltaTime;
            yield return null;
        }
        sword.position = transform.position;
        sword.parent = transform;
        yield return new WaitForSeconds(maxDashTimer - elapsedTime);
        sword.parent = null;
        leash.enabled = false;
        sword.GetComponent<SwordScript>().SlashInDirection(dir);
        sword.GetComponent<SwordScript>().controlled = false;



        yield return null;
    }

    public void OnPurify()
    {
        Corruption -= corruptionLossOnPurify * emDmgMult;
    }
    public void TakeDamage()
    {
        if (!invincible)
        {
            camTarget.SetTrigger("Shake");
            AudioManager.Instance.PlayerDmgTaken.Post(gameObject);
            if (!blood.isPlaying)
                blood.Play();
            sr.color = Color.red;
            Invoke("StopBleeding", 0.2f);
            float amount = 0.5f;
            amount = amount * (1 - GameManager.Instance.defUpgrades * GameManager.Instance.defIncrease) * emDmgMult;
            if (amount <= 0.25f)
            {
                amount = 0.25f;
            }
            CurHealth -= amount;
            healthBar.fillAmount = CurHealth / maxHealth;
        }
        
    }

    IEnumerator onDamageTaken()
    {
        healthBar.color = Color.red;
        yield return new WaitForSeconds(0.15f);
        healthBar.color = Color.white;


        yield return null;
    }
    public void TakeForcedDamage(float amount)
    {
        camTarget.SetTrigger("Shake");
        AudioManager.Instance.PlayerDmgTaken.Post(gameObject);
        if (!blood.isPlaying)
            blood.Play();
        sr.color = Color.red;
        Invoke("StopBleeding", 0.2f);
        amount = amount * (1 - GameManager.Instance.defUpgrades * GameManager.Instance.defIncrease) * emDmgMult;
        if (amount <= 1)
        {
            amount = 1;
        }
        CurHealth -= amount;

        healthBar.fillAmount = CurHealth / maxHealth;
        if (CurHealth <= 0)
        {
            CurHealth = maxHealth;
            healthBar.fillAmount = 0;

            StartCoroutine(SetInvincible(8));
        }
        else
        {
            StartCoroutine(SetInvincible(0.5f));
        }
    }
    public void TakeCustomDamage(float amount)
    {
        if (!invincible)
        {
            camTarget.SetTrigger("Shake");
            AudioManager.Instance.PlayerDmgTaken.Post(gameObject);
            if (!blood.isPlaying)
                blood.Play();
            sr.color = Color.red;
            Invoke("StopBleeding", 0.2f);
            amount = amount * (1 - GameManager.Instance.defUpgrades * GameManager.Instance.defIncrease) * emDmgMult;
            if (amount <= 1)
            {
                amount = 1;
            }
            CurHealth -= amount;

            healthBar.fillAmount = CurHealth / maxHealth;
            if (CurHealth <= 0)
            {
                CurHealth = maxHealth;
                healthBar.fillAmount = CurHealth / maxHealth;
                
                StartCoroutine(SetInvincible(8));
            }
            else
            {
                StartCoroutine(SetInvincible(0.5f));
            }
        }
        
    }
    IEnumerator SetInvincible(float dur)
    {
        invincible = true;
        sr.color = new Color(0, 0, 0, 0.75f);
        yield return new WaitForSeconds(dur);
        sr.color = Color.white;
        invincible = false;
        yield return null;
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        /*
        if (collision.gameObject.layer == 6)
        {
            TakeDamage();
            if (!blood.isPlaying)
                blood.Play();
            sr.color = Color.red;
        }
        */
    }

    IEnumerator Repell()
    {
        AudioManager.Instance.Repell.Post(gameObject);
        canRepell = false;
        yield return new WaitForSeconds(swordScript.repellCooldown);
        canRepell = true;
        swordScript.repellImg.color = new Color(swordScript.repellImg.color.r, swordScript.repellImg.color.g, swordScript.repellImg.color.b, 1f);
        yield return null;
    }

    IEnumerator pullEffect()
    {
        leash.enabled = true;
        yield return new WaitForSeconds(swordScript.pullCooldown);
        leash.enabled = false;
        swordScript.pullImg.color = new Color(swordScript.pullImg.color.r, swordScript.pullImg.color.g, swordScript.pullImg.color.b, 1f);
        yield return null;
    }
    public void FBDemonMode()
    {
        //anim.SetTrigger("TriggerDM");
    }

    IEnumerator DemonMode()
    {
        anim.SetTrigger("TriggerDM");
        demonModeActive = true;
        speed = demonSpeed;
        Sprite s = sr.sprite;
        sr.sprite = demonSprite;
        GameManager.Instance.OnDemonModeEnter();
        yield return new WaitForSeconds(demonModeDuration);
        GameManager.Instance.OnDemonModeExit();
        anim.SetTrigger("TriggerDM");
        speed = initSpeed;
        demonModeActive = false;
        sr.sprite = s;
        yield return null;
    }
    public void TriggerKB(Vector2 from)
    {
        StartCoroutine(knockback(from));
    }
    IEnumerator knockback(Vector2 knockFrom)
    {
        canMove = false;
        rb.velocity = Vector2.zero;
        Vector2 dir = (Vector2)transform.position - knockFrom;
        rb.AddForce(dir.normalized * 50, ForceMode2D.Impulse);
        yield return new WaitForSeconds(0.15f);
        
        rb.velocity = Vector2.zero;
        canMove = true;

        yield return null;
    }

    IEnumerator OnJuiced()
    {
        juicedUp = juicedMult;
        curJuiceDur = juicedDuration;
        while (curJuiceDur > 0) {
            curJuiceDur -= Time.deltaTime;
            yield return null;
        
        }
        juicedUp = 1;
        yield return null;
    }

    public void StopBleeding()
    {
        blood.Stop();
        sr.color = Color.white;
    }
    public void AddCorruptionVoid(float amount)
    {
        print("Added corruption + " + amount);
        totalCorr += amount;
        StartCoroutine(AddCorr(amount));
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("DZone"))
        {

            TakeCustomDamage(20);
        }
        if (collision.gameObject.CompareTag("DZone2"))
        {

            TakeCustomDamage(35);
        }

        if (collision.gameObject.CompareTag("Juice"))
        {
            if (juicedUp==1)
            {
                StartCoroutine(OnJuiced());
            }
            else
            {
                curJuiceDur += juicedDuration;
            }

            Destroy(collision.gameObject);
            if (!juiceTutorialTriggered)
            {
                juiceTutorialTriggered = true;
            }
        }

        if (collision.gameObject.CompareTag("Portal"))
        {
            GameManager.Instance.GoToBossFight();
        }

        if (collision.gameObject.CompareTag("Projectile"))
        {

            TakeCustomDamage(5);
        }
        else if (collision.gameObject.CompareTag("Knockback"))
        {
            if (!invincible)   
                StartCoroutine(knockback(collision.gameObject.transform.position));
            TakeCustomDamage(5);
            


        }
        else if (collision.gameObject.CompareTag("DemonTrigger"))
        {
            if (!demonModeActive)
            {
                StartCoroutine(DemonMode());
            }
            
        }
        else if (collision.gameObject.GetComponent<Enemy>() != null)
        {
            if (collision.gameObject.GetComponent<Enemy>().purified)
            {
                
                GameManager.Instance.saved++;
                collision.gameObject.GetComponent<Enemy>().Rescue();
            }
        }
        else if (collision.gameObject.CompareTag("End"))
        {
            GameManager.Instance.TriggerEnding();
        }
        else if (collision.gameObject.CompareTag("FinalSecretDoor"))
        {
            GameManager.Instance.OnDemonModeExit();
            ResetData();
            SceneManager.LoadScene("Outside");
        }
    }

    public void Unstuck()
    {
        canMove = true;
        rb.velocity = Vector3.zero;
        transform.position = new Vector3(-36.5f, -12, -1);
    }
    public void knockbackFailsafe(float d)
    {
        Invoke("failsafe", d);
    }
    void failsafe()
    {
        this.enabled = true;
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.layer == 6)
        {
            StopBleeding();
        }
    }
    public void SetCorruption(float amount)
    {
        Corruption = amount;
    }
    public float GetCorruption()
    {
        return Corruption;
    }

    public void Revive()
    {
        if (onDeath!=null)
        {
            onDeath.Invoke();
        }
        
        print("Revived");
        if (demonModeActive) { AudioManager.Instance.StartTicking.Post(gameObject); }
        AudioManager.Instance.Revive.Post(gameObject);
        maxHealth = 100 + GameManager.Instance.hpUpgrades * GameManager.Instance.hpIncrease;
        CurHealth = maxHealth;
        healthBar.fillAmount = CurHealth / maxHealth;
        deaths++;
        AddCorruptionVoid(Random.Range(maxCorrChange.x, maxCorrChange.y));
        
        SetInvincible(8);
    }
    IEnumerator AddCorr(float amount)
    {
        
        float initC = Corruption;
        float elapsedTime = 0;
        float addedAmt = 0;
        
        while (addedAmt < amount)
        {
            elapsedTime += Time.fixedUnscaledDeltaTime;
            Corruption += elapsedTime/35;
            addedAmt += elapsedTime/35;
            
            yield return null;
        }
        
        Corruption = initC + amount;
        

        yield return null;
    }
}
