using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloodSplatterScript : MonoBehaviour
{
    public float fadeDelay;
    public float fadeTime;
    bool isFading = false;
    Color c;
    public List<Sprite> sprites = new List<Sprite>();
    void Start()
    {
        int rand = Random.Range(0, sprites.Count);
        float randSize = Random.Range(0.8f, 1.6f);
        transform.localScale = Vector3.one * randSize;
        GetComponent<SpriteRenderer>().sprite = sprites[rand];
        GetComponent<SpriteRenderer>().color = GameManager.Instance.randomColor();
        Invoke("StartFade", fadeDelay);
        c = GetComponent<SpriteRenderer>().color;
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isFading)
        {
            GetComponent<SpriteRenderer>().color = new Color(c.r, c.g, c.b,1-fadeTime*Time.deltaTime);


        }
    }

    void StartFade()
    {
        isFading = true;
        Destroy(gameObject, 5);
    }
}
