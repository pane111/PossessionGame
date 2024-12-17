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
    public void OnShoot()
    {

        for (int i = 0; i < bulletAmount; i++)
        {
            if (randomRotation)
            {
                float angle = Random.Range(-maxDeg, maxDeg);
                GameObject shotBullet = Instantiate(toShoot, transform.position, Quaternion.identity);
                shotBullet.transform.localRotation = Quaternion.Euler(0, 0, transform.localRotation.eulerAngles.z + maxDeg / 2 - angle);
                shotBullet.GetComponent<Rigidbody2D>().AddForce(shotBullet.transform.right * shotForce, ForceMode2D.Impulse);
                Destroy(shotBullet, 5);
            }
            else
            {
                float angle = (maxDeg / bulletAmount) * i;
                GameObject shotBullet = Instantiate(toShoot, transform.position, Quaternion.identity);
                shotBullet.transform.localRotation = Quaternion.Euler(0, 0, transform.localRotation.eulerAngles.z + maxDeg / 2 - angle);
                shotBullet.GetComponent<Rigidbody2D>().AddForce(shotBullet.transform.right * shotForce, ForceMode2D.Impulse);
                Destroy(shotBullet, 5);
            }

        }
    }
}
