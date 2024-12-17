using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Player : MonoBehaviour
{
    public float speed;
    Vector2 moveDirection = Vector2.zero;
    public Rigidbody2D rb;
    public SpriteRenderer sr;
    public float maxStepTimer = 0.2f;
    public float stepTimer = 0.2f;
    public float dashForce;
    public float maxDashTimer = 0.2f;
    float dashTimer = 0.2f;
    public float dashCooldown = 0;
    public bool isDashing;
    public float maxDashCooldown = 1;
    public float corruption = 0;
    public Image corruptionImage;
    public TextMeshProUGUI corruptionText;
    public Image swordGlow;
    public Image overlay;
    public ParticleSystem afterimage;

    public Image healthBar;
    public float maxHealth;
    public float curHealth;
    public ParticleSystem blood;

    public LineRenderer leash;
    public Transform sword;
    private SwordScript swordScript;
    bool invincible = false;
    public float corruptionLossOnPurify = 40;
    bool canRepell = true;

    void Start()
    {
        stepTimer = maxStepTimer;
        swordScript = sword.GetComponent<SwordScript>();
    }

    // Update is called once per frame
    void Update()
    {

        var vertical = Input.GetAxisRaw("Vertical");
        var horizontal = Input.GetAxisRaw("Horizontal");


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
                
                isDashing = true;
                rb.AddForce(moveDirection.normalized * dashForce, ForceMode2D.Impulse);
                StartCoroutine(Invincibility(dashTimer));
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


        if (!isDashing )
        {
            rb.velocity = moveDirection.normalized * speed;
            dashCooldown -= Time.deltaTime;
            if (dashCooldown < 0)
                dashCooldown = 0;
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

        corruptionText.text = corruption.ToString() +"%";
        corruptionText.color = Color.Lerp(Color.white, Color.red, corruption / 100);
        corruptionImage.color = new Color(1, 1, 1, corruption / 100);
        swordGlow.color = new Color(1, 1, 1, corruption / 100);

        corruptionText.transform.localScale = Vector3.one * (0.45f + corruption / 100);
            overlay.color = new Color(1, 1, 1, corruption / 100);

    }
    public void OnPurify()
    {
        corruption -= corruptionLossOnPurify;
        if (corruption < 0) { corruption = 0; }
    }
    public void TakeDamage()
    {

        


        if (!invincible)
        {
            if (!blood.isPlaying)
                blood.Play();
            sr.color = Color.red;
            Invoke("StopBleeding", 0.2f);
            curHealth -= 0.5f;
            healthBar.fillAmount = curHealth / maxHealth;
            if (curHealth <= 0)
            {
                curHealth = maxHealth;
                healthBar.fillAmount = curHealth / maxHealth;
                corruption += 35;
                GameManager.Instance.OnDeath();
                StartCoroutine(Invincibility(5));
            }
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
            curHealth -= amount;
            healthBar.fillAmount = curHealth / maxHealth;
            if (curHealth <= 0)
            {
                curHealth = maxHealth;
                healthBar.fillAmount = curHealth / maxHealth;
                corruption += 35;
                GameManager.Instance.OnDeath();
                StartCoroutine(Invincibility(5));
            }
        }
        
    }
    IEnumerator Invincibility(float dur)
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
        yield return null;
    }

    IEnumerator pullEffect()
    {
        leash.enabled = true;
        yield return new WaitForSeconds(swordScript.pullCooldown);
        leash.enabled = false;
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
}
