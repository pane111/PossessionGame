using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bozo : MonoBehaviour
{
    public SpreadShot left;
    public SpreadShot right;
    public Transform player;
    public SpriteRenderer sr;
    public Animator anim;
    public int moves;
    public Vector2 offset;
    public float jumpDur;

    void Start()
    {
        player = GameObject.Find("Player").transform;
        StartCoroutine(DoThings());
    }

    public void ShootPlayer()
    {
        Vector2 dir = player.position - transform.position;
        if (dir.x > 0)
        {
            sr.flipX = false;
            right.OnShoot();
        }
        else
        {
            sr.flipX = true;
            left.OnShoot();
        }
    }

    IEnumerator DoThings()
    {
        yield return new WaitForSeconds(5);
        float r = Random.Range(0, 100);
        if (r <= 50)
        {
            offset.x = -offset.x;
        }

        Vector3 targetPos = player.position + (Vector3)offset;

        Vector3 initPos = transform.position;
        float elapsedTime = 0;
        anim.SetTrigger("Jump");
        while (elapsedTime < jumpDur) {

            transform.position = Vector3.Lerp(initPos, targetPos, elapsedTime/jumpDur);
            elapsedTime += Time.deltaTime;
            yield return null;

        }
        anim.SetTrigger("Attack");
        ShootPlayer();
        
        moves--;
        if (moves <=0)
        {
            yield return new WaitForSeconds(3);
            anim.SetTrigger("End");
            Destroy(gameObject, 5);
        }
        else
        {
            StartCoroutine(DoThings());
        }


        yield return null;
    }
}
