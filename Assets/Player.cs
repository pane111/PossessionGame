using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

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
            if (CurHealth <= 0)
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
    Vector2 moveDirection = Vector2.zero;
    public float maxStepTimer = 0.2f;
    public float stepTimer = 0.2f;
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
            AudioManager.Instance.possessionRTPC.SetGlobalValue(value);
            _corruption = value;
        }
    }
    public float corruptionLossOnPurify = 40;
    public Image corruptionImage;
    public TextMeshProUGUI corruptionText;
    public Image swordGlow;
    public Image overlay;
    public ParticleSystem afterimage;

    public LineRenderer leash;
    public Transform sword;
    private SwordScript swordScript;
    bool invincible = false;
    bool canRepell = true;

    public Sprite demonSprite;
    public float demonModeDuration;
    public bool demonModeActive;

    public float orbitSpeed;
    float curDeg=0;
    public Transform rotator;
    public Transform orbiter;
    void Start()
    {
        stepTimer = maxStepTimer;
        swordScript = sword.GetComponent<SwordScript>();
        AudioManager.Instance.GS_NormalMode.SetValue();
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
                sr.flipX = !sr.flipX;
                stepTimer = maxStepTimer;
            }
            if (Input.GetKeyDown(KeyCode.Space) && dashCooldown <= 0)
            {
                dashImg.color = new Color(dashImg.color.r, dashImg.color.g, dashImg.color.b, 0.2f);
                isDashing = true;
                rb.AddForce(moveDirection.normalized * dashForce, ForceMode2D.Impulse);
                StartCoroutine(SetInvincible(dashTimer));
                afterimage.Play();
                dashTimer = maxDashTimer;
                dashCooldown = maxDashCooldown;

            }

        }

        if (Input.GetKeyDown(KeyCode.E) && canRepell && Vector2.Distance(transform.position, sword.position) < swordScript.repellRange)
        {
            StartCoroutine(Repell());
            swordScript.RepellFunction();
        }

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            StartCoroutine(pullEffect());
            swordScript.Pull();
        }

        Vector3 v = sword.position - transform.position;
        
        leash.SetPosition(1,v);


        if (!isDashing)
        {
            if (!demonModeActive)
            {
                rb.velocity = moveDirection.normalized * speed;
            }
            else
            {
                rb.velocity = moveDirection.normalized * speed * 1.3f;
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

        corruptionText.text = Corruption.ToString() +"%";
        corruptionText.color = Color.Lerp(Color.white, Color.red, Corruption / 100);
        corruptionImage.color = new Color(1, 1, 1, Corruption / 100);
        swordGlow.color = new Color(1, 1, 1, Corruption / 100);

        corruptionText.transform.localScale = Vector3.one * (0.45f + Corruption / 100);
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
            if (!blood.isPlaying)
                blood.Play();
            sr.color = Color.red;
            Invoke("StopBleeding", 0.2f);
            CurHealth -= 0.5f;
            healthBar.fillAmount = CurHealth / maxHealth;
        }
        
    }
    public void TakeCustomDamage(float amount)
    {
        if (!invincible)
        {
            if (!blood.isPlaying)
                blood.Play();
            sr.color = Color.red;
            Invoke("StopBleeding", 0.2f);
            CurHealth -= amount;
            healthBar.fillAmount = CurHealth / maxHealth;
            if (CurHealth <= 0)
            {
                CurHealth = maxHealth;
                healthBar.fillAmount = CurHealth / maxHealth;
                GameManager.Instance.OnDeath();
                StartCoroutine(SetInvincible(5));
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
        if (collision.gameObject.layer == 6)
        {
            TakeDamage();
            if (!blood.isPlaying)
                blood.Play();
            sr.color = Color.red;
        }
    }

    IEnumerator Repell()
    {
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

    IEnumerator DemonMode()
    {
        demonModeActive = true;
        
        Sprite s = sr.sprite;
        sr.sprite = demonSprite;
        GameManager.Instance.OnDemonModeEnter();
        yield return new WaitForSeconds(demonModeDuration);
        GameManager.Instance.OnDemonModeExit();
        demonModeActive = false;
        sr.sprite = s;
        yield return null;
    }

    public void StopBleeding()
    {
        blood.Stop();
        sr.color = Color.white;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Projectile"))
        {

            TakeCustomDamage(15);
        }
        if (collision.gameObject.CompareTag("DemonTrigger"))
        {

            StartCoroutine(DemonMode());
        }

        if (collision.gameObject.GetComponent<Enemy>() != null)
        {
            if (collision.gameObject.GetComponent<Enemy>().purified)
            {
                
                GameManager.Instance.saved++;
                collision.gameObject.GetComponent<Enemy>().Rescue();
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.layer == 6)
        {
            StopBleeding();
        }
    }

    public void Revive()
    {
        CurHealth = maxHealth;
        healthBar.fillAmount = CurHealth / maxHealth;
        Corruption += 35;
    }
}
