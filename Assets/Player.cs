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

    void Start()
    {
        stepTimer = maxStepTimer;
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
                afterimage.Play();
                dashTimer = maxDashTimer;
                dashCooldown = maxDashCooldown;

            }

        }
        




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
}
