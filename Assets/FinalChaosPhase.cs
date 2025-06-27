using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FinalChaosPhase : MonoBehaviour
{
    public float curHp;
    public float maxHp;
    public Animator hpBarAnim;
    public Image hpBar;
    public SpriteRenderer sr;

    public GameObject handSpawner;
    public int spawnHandsAmount;
    public Transform player;

    public GameObject dancingSwords;
    public AudioSource hit;

    void Start()
    {
        player = GameObject.Find("Player").transform;
        curHp = maxHp;
        hpBar.fillAmount = 1;
        
    }
    public void SpawnDancingSwords()
    {
        Vector2 offset = new Vector2(Random.Range(-1, 10), Random.Range(-1,1));
        Instantiate(dancingSwords,player.position + (Vector3)offset,Quaternion.identity);
        Invoke("SpawnDancingSwords",Random.Range(25,35));
    }
    public void SpawnHandsVoid()
    {
        StartCoroutine(SpawnHands());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void TakeDamage(float amount)
    {
        hit.pitch = Random.Range(0.65f, 0.75f);
        hit.Play();
        curHp -= amount;
        hpBarAnim.SetTrigger("Hit");
        hpBar.fillAmount = curHp / maxHp;
        StartCoroutine(DmgFlash());

    }
    public void Heal(float amount)
    {
        

        curHp += amount;
        if (curHp > maxHp)
        {
            curHp = maxHp;
        }
        hpBar.fillAmount = curHp / maxHp;
    }

    IEnumerator SpawnHands()
    {
        for (int i = 0; i < spawnHandsAmount; i++)
        {
            Vector2 offset = new Vector2(Random.Range(-4,4),Random.Range(-4,4));
            Instantiate(handSpawner,player.position + (Vector3)offset, Quaternion.identity);
            yield return new WaitForSeconds(0.25f);
            yield return null;
        }
        yield return new WaitForSeconds(Random.Range(5, 18));
        StartCoroutine(SpawnHands());
        yield return null;
    }
    IEnumerator DmgFlash()
    {
        sr.color = Color.red;
        yield return new WaitForSeconds(0.05f);
        sr.color = Color.white;
        yield return new WaitForSeconds(0.05f);
        sr.color = Color.red;
        yield return new WaitForSeconds(0.05f);
        sr.color = Color.white;
        yield return new WaitForSeconds(0.05f);
        sr.color = Color.red;
        yield return new WaitForSeconds(0.05f);
        sr.color = Color.white;
        yield return new WaitForSeconds(0.05f);
        yield return null;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<SwordScript>() != null)
        {

            SwordScript sword = collision.GetComponent<SwordScript>();
            if (sword.curTarget == this.gameObject.transform) { sword.attacksCount++; }

            if (collision.GetComponent<SwordScript>().isSlashing)
            {

                TakeDamage(3 + GameManager.Instance.dmgUpgrades * 0.5f);

            }
            else
            {
                TakeDamage(1 + GameManager.Instance.dmgUpgrades * 0.15f);
                if (sword.curTarget == sword.playerChar || sword.curTarget == sword.playerChar.gameObject.GetComponent<Player>().orbiter) { sword.curTarget = this.gameObject.transform; sword.Idling = false; sword.SIforNPC(); }
            }


        }
    }
}
