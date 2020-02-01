using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrusterAnimation : MonoBehaviour
{
    public float TargetSize { get; set; } // the thrust force shall modify this
    public float SizeMultipler = 1.5f;
    public float idleSize = 0.2f;
    public SpriteRenderer spriteRenderer;
    public float AnimationSpeed;
    public Sprite[] Frames;
    public float sizeVariationPercent = 20;

    private int currentFrame;
    private float timeFromLastFrame = 0;

    private void Update()
    {
        timeFromLastFrame += Time.deltaTime;
        if (timeFromLastFrame > (1 / AnimationSpeed))
        {
            currentFrame = currentFrame + 1 == Frames.Length ? 0 : currentFrame + 1;
            spriteRenderer.sprite = Frames[currentFrame];
            transform.localScale = Vector3.one*idleSize + Vector3.one * SizeMultipler * (TargetSize - sizeVariationPercent/200 + Random.value * sizeVariationPercent/100 );
        }
    }
}
