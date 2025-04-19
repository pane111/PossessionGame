using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpreadShot : MonoBehaviour
{
    public GameObject toShoot;
    public int bulletAmount;
    public float maxDeg;
    public bool randomRotation;
    public float shotForce;
    public bool triggerOnStart;
    public bool triggerOnDeath;
    public float addedDeg;
    public float retriggerDelay;
    public bool retrigger;
    public bool addDegAfterRetrigger;
    public bool isNotBullet = false;
    public bool canShoot=true;
    public bool destroyBulletAuto = true;
    float ad2;
    private void Start()
    {
        ad2 = addedDeg;
        if (triggerOnStart)
        {
            OnShoot();
        }
    }

    private void OnDestroy()
    {
        if (triggerOnDeath)
        {
            OnShoot();
        }
    }
    public void OnShoot()
    {
        if (canShoot)
        {


            for (int i = 0; i < bulletAmount; i++)
            {
                if (randomRotation)
                {
                    float angle = Random.Range(-maxDeg, maxDeg);
                    GameObject shotBullet = Instantiate(toShoot, transform.position, Quaternion.identity);
                    if (!isNotBullet)
                    {
                        shotBullet.transform.localRotation = Quaternion.Euler(0, 0, (transform.localRotation.eulerAngles.z + maxDeg / 2 - angle) + addedDeg);
                        shotBullet.GetComponent<Rigidbody2D>().AddForce(shotBullet.transform.right * shotForce, ForceMode2D.Impulse);
                    }
                    if (destroyBulletAuto)
                        Destroy(shotBullet, 5);
                }
                else
                {
                    float angle = (maxDeg / bulletAmount) * i;
                    GameObject shotBullet = Instantiate(toShoot, transform.position, Quaternion.identity);
                    if (!isNotBullet)
                    {
                        shotBullet.transform.localRotation = Quaternion.Euler(0, 0, (transform.localRotation.eulerAngles.z + maxDeg / 2 - angle) + addedDeg);
                        shotBullet.GetComponent<Rigidbody2D>().AddForce(shotBullet.transform.right * shotForce, ForceMode2D.Impulse);
                    }
                    if (destroyBulletAuto)
                        Destroy(shotBullet, 5);
                }

            }
        }

        if (retrigger)
        {
            if (addDegAfterRetrigger)
            {
                addedDeg += ad2;
            }
            Invoke("OnShoot", retriggerDelay);
        }
    }
}
