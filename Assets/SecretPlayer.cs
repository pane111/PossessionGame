using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SecretPlayer : MonoBehaviour
{
    public Animator anim;
    public bool canMove;
    public float actionPoints;
    public bool blocking;
    public bool parrying;
    public float apRegen;
    public Image apBar;
    public Image hpBar;
    public float curHp;
    bool canReleaseBlock = false;
    public TextMeshProUGUI apText;
    public ParticleSystem parry;
    public Animator sAtk;
    public TextMeshProUGUI nameText;
    VerySecretBoss boss;
    public bool canDie;
    public GameObject redFlash;
    void Start()
    {
        boss = FindObjectOfType<VerySecretBoss>();
        curHp = 100;
        actionPoints = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (canMove)
        {
            if (Input.GetButtonDown("Fire2") && actionPoints > 100)
            {
                anim.SetTrigger("Attack");
                sAtk.gameObject.SetActive(true);
                sAtk.SetTrigger("Attack");
                actionPoints -= 100;
                canMove = false;
            }
            if (Input.GetButtonDown("Jump") && actionPoints >= 0)
            {
                anim.SetTrigger("Block");
                canMove = false;
            }
        }

        if (canMove)
        {
            actionPoints += apRegen;
            if (actionPoints > 300)
            {
                actionPoints = 300;
            }
        }

        if (canReleaseBlock)
        {
            if (!Input.GetButton("Jump") || actionPoints < 0)
            {
                anim.SetTrigger("BlockRelease");
                canMove = true;
                canReleaseBlock = false;
                StopBlock();
            }
        }
        apText.text = ((int)actionPoints/100).ToString("F0");
        if (actionPoints < 0)
        {
            apText.color = Color.red;
        }
        else
        {
            apText.color = Color.white;
        }
        apBar.fillAmount = actionPoints / 300f;
        hpBar.fillAmount = curHp / 100f;
    }
    public void SetName(string name)
    {
        nameText.text = name;
    }
    public void TakeDamage(float amount)
    {
        if (parrying)
        {

            boss.parried = true;

            parry.Play();
            actionPoints += 60;
            if (actionPoints > 300)
                actionPoints = 300;

            curHp += 5;
            if (curHp > 100)
                curHp = 100;
        }
        else if (blocking)
        {
            curHp -= amount / 3;
            actionPoints -= 30;
            if (actionPoints < 0)
                actionPoints = -55;
        }
        else
        {
            curHp -= amount;
            if (curHp <= 0)
            {
                if (canDie)
                {
                    curHp = 100;
                    redFlash.SetActive(true);
                    boss.Part2();
                }
                else
                {
                    curHp = 0;
                }
            }
                

            
            anim.SetTrigger("Damage");
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Projectile"))
        {
            Destroy(collision.gameObject);
            TakeDamage(5);
        }
        if (collision.CompareTag("DZone"))
        {
            TakeDamage(10);
        }
    }

    public void StartBlock()
    {
        parrying = true;
        canReleaseBlock = false;
        blocking = true;
    }
    public void StopParry()
    {
        parrying = false;
    }
    public void BlockReset()
    {
        canReleaseBlock = true;
    }
    public void StopBlock()
    {
        blocking = false;
    }
    public void StopMovement()
    {
        canMove = false;
    }
    public void StartMovement()
    {
        canMove = true;
    }
}
