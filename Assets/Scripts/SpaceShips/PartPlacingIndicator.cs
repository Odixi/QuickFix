using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartPlacingIndicator : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;

    public void SetAllowedState(bool allow)
    {
        spriteRenderer.color = allow ? new Color(1,1,1,0.34f): new Color(1,0,0,0.4f);
    }
}
