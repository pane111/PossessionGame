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
    public bool isMoving;
    public float maxStepTimer = 0.2f;
    public float stepTimer = 0.2f;

    public float corruption = 0;
    public Image corruptionImage;
    public TextMeshProUGUI corruptionText;
    public Image overlay;
    void Start()
    {
        stepTimer = maxStepTimer;
    }

    // Update is called once per frame
    void FixedUpdate()
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
        }
        rb.velocity = moveDirection.normalized * speed;

        corruptionText.text = corruption.ToString() +"%";
        corruptionText.color = Color.Lerp(Color.white, Color.red, corruption / 100);
        corruptionImage.color = new Color(1, 1, 1, corruption / 100);

        corruptionText.transform.localScale = Vector3.one * (0.45f + corruption / 100);
            overlay.color = new Color(1, 1, 1, corruption / 100);

    }
}
