using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartPlacingIndicator : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;

    public void SetAllowedState(bool allow)
    {
        spriteRenderer.color = allow ? Color.green : Color.red;
    }
}
