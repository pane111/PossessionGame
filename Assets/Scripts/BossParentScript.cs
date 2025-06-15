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
    public SpriteRenderer sr;
    public GameObject bgEffect;
    public float CurHealth
    {
        get { return curHealth; }
        set
        {
            curHealth = value;
            dPortrait.sprite = portrait;
            healthBar.enabled = true;
            if (curHealth>=0)
                healthBar.rectTransform.sizeDelta = new Vector2(3600 * (curHealth / maxHealth),200);
            else
            {
                healthBar.rectTransform.sizeDelta = new Vector2(0, 200);
            }
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
    public IEnumerator damageEffect()
    {
        sr.color = Color.red;
        yield return new WaitForSeconds(0.15f);
        sr.color = Color.white;
        yield return null;
    }

    public virtual void TakeDamage(float amount)
    {
        amount += 0.125f * GameManager.Instance.dmgUpgrades;
        CurHealth -= amount;
        if (CurHealth < maxHealth)
        {
            StartCoroutine(damageEffect());
            hpBarAnim.SetTrigger("Hit");
        }
        
        if (CurHealth <= 0) {
            Time.timeScale = 1;
            GameManager.Instance.storedTS = 1;
            Time.fixedDeltaTime = 0.02F * Time.timeScale;
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
