using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomSprite : MonoBehaviour
{
    private SpriteRenderer renderer;
    public Sprite[] sprites;
    void Start()
    {
        renderer = GetComponent<SpriteRenderer>();
        renderer.sprite = sprites[Random.Range(0, sprites.Length)];
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
