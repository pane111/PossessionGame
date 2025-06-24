using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flipbook : MonoBehaviour
{
    public List<Sprite> sprites;
    public SpriteRenderer sr;
    public float delay;
    float curDelay = 0;
    public bool canAnimate;
    int curFrame = 0;
    void Update()
    {
        if (canAnimate)
        {
            sr.sprite = sprites[curFrame];
            curDelay += Time.deltaTime;
            if (curDelay > delay)
            {
                curFrame++;
                if (curFrame == sprites.Count)
                {
                    curFrame = 0;
                }
                curDelay = 0;
            }
        }
    }
}
