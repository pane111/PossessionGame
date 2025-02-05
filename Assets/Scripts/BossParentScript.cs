using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BossParentScript : MonoBehaviour
{
    public float maxHealth;
    private float curHealth;
    public float CurHealth
    {
        get { return curHealth; }
        set
        {
            curHealth = value;
            healthBar.rectTransform.sizeDelta = new Vector2(3600 * (curHealth / maxHealth),200);

            if ((curHealth / maxHealth)*100 <= dPercentages[curDialogue])
            {
                TriggerDialogue();
            }
        }
    }
    public Image healthBar;
    public TextMeshProUGUI dialogueText;
    public Animator dialogAnim;
    public Animator hpBarAnim;

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
    }
}
