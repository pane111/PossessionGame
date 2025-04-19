using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloodSplatterScript : MonoBehaviour
{
    public float fadeDelay;
    public float fadeTime;
    public List<Sprite> sprites = new List<Sprite>();
    void Start()
    {
        int rand = Random.Range(0, sprites.Count);
        float randSize = Random.Range(0.8f, 1.6f);
        transform.localScale = Vector3.one * randSize;
        GetComponent<SpriteRenderer>().sprite = sprites[rand];
        GetComponent<SpriteRenderer>().color = GameManager.Instance.randomColor();
        transform.position = new Vector3(transform.position.x,transform.position.y, 0);
        
    }
}
