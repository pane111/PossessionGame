using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BossParentScript : MonoBehaviour
{
    public float maxHealth;
    private float curHealth;
    public GameObject additional;
    public Sprite portrait;
    public Image dPortrait;
    public float CurHealth
    {
        get { return curHealth; }
        set
        {
            curHealth = value;
            dPortrait.sprite = portrait;
            healthBar.enabled = true;
            healthBar.rectTransform.sizeDelta = new Vector2(3600 * (curHealth / maxHealth),200);
            if (curDialogue < dPercentages.Count - 1)
            {
                if ((curHealth / maxHealth) * 100 <= dPercentages[curDialogue])
                {
                    TriggerDialogue();
                }
            }
            
        }
    }
    public Image healthBar;
    public TextMeshProUGUI dialogueText;
    public Animator dialogAnim;
    public Animator hpBarAnim;
    public GameObject transition;

    [TextArea(6, 10)]
    public List<string> dialogues = new List<string>();

    public List<float> dPercentages = new List<float>();

    int curDialogue = 0;
    public void TriggerDialogue()
    {
        dialogueText.text = dialogues[curDialogue];
        curDialogue++;
        dialogAnim.SetTrigger("Dialogue");
    }

    public void TakeDamage(float amount)
    {
        CurHealth -= amount;
        hpBarAnim.SetTrigger("Hit");
        if (CurHealth <= 0) {
            healthBar.enabled = false;
            print("Boss died");
            transition.SetActive(true);
            transition.transform.position = transform.position;
            if (additional!=null)
            {
                Destroy(additional.gameObject);
            }
            Destroy(gameObject);
        }
    }
}
