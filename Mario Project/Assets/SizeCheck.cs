using System;
using UnityEngine;

public class SizeCheck : MonoBehaviour
{
    public SpriteRenderer SpriteRenderer;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Get the SpriteRenderer component (for a sprite)
        Bounds bounds = SpriteRenderer.bounds;
        Debug.Log(bounds.size);

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
