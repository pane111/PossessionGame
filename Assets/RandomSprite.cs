using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomSprite : MonoBehaviour
{
    public List<Sprite> sprites;    
    void Start()
    {
        int r = Random.Range(0, sprites.Count);
        GetComponent<SpriteRenderer>().sprite = sprites[r];
        GetComponent<Rigidbody2D>().velocity *= Random.Range(0.7f, 1.1f);
        transform.rotation = Quaternion.Euler(0, 0, Random.Range(0, 360));
    }
}
