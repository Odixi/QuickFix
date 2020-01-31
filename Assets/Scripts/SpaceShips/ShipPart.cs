using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[Flags]
public enum ConnectionPoints{
    Up = 1,
    UpRight = 2,
    Right = 4,
    DownRight = 8,
    Down = 16,
    DownLeft = 32,
    Left = 64,
    UpLeft = 128,
    All = Up | UpRight | Right | DownRight | Down | DownLeft | Left | UpLeft
}

public class ShipPart : MonoBehaviour
{
    public ConnectionPoints ConnectionPoints = 0;
    public int X = -1;
    public int Y = -1;

    public EventHandler OnDestroyed;

    public void Explode()
    {
        // TODO blow up
        Destroy(this);
    }

    void OnDestroy()
    {
        OnDestroyed?.Invoke(this, EventArgs.Empty);
    }
}
