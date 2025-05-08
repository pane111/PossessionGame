using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;

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
                StartCoroutine(SetInvincible(5));
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
    bool juiceTutorialTriggered = false;
    public Animator anim;
    float emDmgMult=1;
    void Start()
    {

        if (PlayerPrefs.HasKey("Corruption"))
        {
            Corruption = PlayerPrefs.GetFloat("Corruption");
            
        }
        if (GameManager.Instance.expertMode)
        {
            emDmgMult = 1.35f;
            demonModeDuration = demonModeDuration * 0.9f;
            maxCorrChange *= 1.15f;
           
        }
        anim = GetComponent<Animator>();
        stepCounter = stepSoundCount;
        _curHealth = maxHealth;
        stepTimer = maxStepTimer;
        swordScript = sword.GetComponent<SwordScript>();
        AudioManager.Instance.GS_NormalMode.SetValue();
        initSpeed = speed;
    }

    // Update is called once per frame
    void Update()
    {

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
        
        if (moveDirection.magnitude > 0 )
        {
            stepTimer -= Time.deltaTime;
            if (stepTimer <= 0 )
            {
                if(stepCounter > 0) { stepCounter--; }
                else if (stepCounter==0) { AudioManager.Instance.Player_Footstep.Post(gameObject); stepCounter = stepSoundCount; }
                sr.flipX = !sr.flipX;
                stepTimer = maxStepTimer;
            }
            if (Input.GetButtonDown("Jump") && canMove && dashCooldown <= 0)
            {
                AudioManager.Instance.Dash.Post(gameObject);
                dashImg.color = new Color(dashImg.color.r, dashImg.color.g, dashImg.color.b, 0.2f);
                isDashing = true;
                rb.AddForce(moveDirection.normalized * dashForce, ForceMode2D.Impulse);
                invincible = true;
                StartCoroutine(SetInvincible(maxDashTimer));
                afterimage.Play();
                dashTimer = maxDashTimer;
                dashCooldown = maxDashCooldown;

            }

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
            if (!demonModeActive)
            {
                rb.velocity = moveDirection.normalized * juicedUp * speed * (1 + (GameManager.Instance.MSUpgrades*GameManager.Instance.MSIncrease));
            }
            else
            {
                rb.velocity = moveDirection.normalized * speed * juicedUp * 1.3f * (1 + (GameManager.Instance.MSUpgrades * GameManager.Instance.MSIncrease*1.2f));
            }

            dashCooldown -= Time.deltaTime;
            if (dashCooldown < 0)
            { 
                dashCooldown = 0;
                dashImg.color = new Color(dashImg.color.r, dashImg.color.g, dashImg.color.b, 1f);
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
    public void OnPurify()
    {
        Corruption -= corruptionLossOnPurify;
    }
    public void TakeDamage()
    {
        if (!invincible)
        {
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
            GameManager.Instance.OnDeath();

            StartCoroutine(SetInvincible(5));
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
                GameManager.Instance.OnDeath();
                
                StartCoroutine(SetInvincible(5));
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
        yield return new WaitForSeconds(dur);
        
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
        anim.SetTrigger("TriggerDM");
    }

    IEnumerator DemonMode()
    {
        
        demonModeActive = true;
        speed = demonSpeed;
        Sprite s = sr.sprite;
        sr.sprite = demonSprite;
        GameManager.Instance.OnDemonModeEnter();
        yield return new WaitForSeconds(demonModeDuration);
        GameManager.Instance.OnDemonModeExit();
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
        yield return new WaitForSeconds(juicedDuration);
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

        if (collision.gameObject.CompareTag("Juice") && juicedUp==1)
        {
            StartCoroutine(OnJuiced());
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
        if (demonModeActive) { AudioManager.Instance.StartTicking.Post(gameObject); }
        AudioManager.Instance.Revive.Post(gameObject);
        maxHealth = 100 + GameManager.Instance.hpUpgrades * GameManager.Instance.hpIncrease;
        CurHealth = maxHealth;
        healthBar.fillAmount = CurHealth / maxHealth;
        StartCoroutine(AddCorr(Random.Range(maxCorrChange.x, maxCorrChange.y)));
        SetInvincible(5);
    }
    IEnumerator AddCorr(float amount)
    {
        
        float initC = Corruption;
        float elapsedTime = 0;
        float addedAmt = 0;
        /*
        while (addedAmt < amount)
        {
            elapsedTime += Time.fixedUnscaledDeltaTime;
            Corruption += elapsedTime/35;
            addedAmt += elapsedTime/35;
            
            yield return null;
        }
        */
        Corruption = initC + amount;

        yield return null;
    }
}
