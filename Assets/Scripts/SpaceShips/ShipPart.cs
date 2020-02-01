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
    public SpaceShip MotherShip;
    public int Health = 3;
    public int CollisionDamage = 1;
    public bool CanRotate45 = false;
    public ConnectionPoints ConnectionPoints = 0;
    public int X = -1;
    public int Y = -1;

    public EventHandler OnDestroyed;

    public void Explode()
    {
        // TODO blow up
        Destroy(this);
    }

    public void Rotate90(bool clockwise)
    {
        float angle = clockwise ? 90 : -90;
        this.transform.Rotate(Vector3.forward, angle);
        int overflowBits = 0;
        // Some magical shit
        if (clockwise)
        {
            overflowBits = (int)((ConnectionPoints.Up & ConnectionPoints) | (ConnectionPoints.UpRight & ConnectionPoints));
            ConnectionPoints = (ConnectionPoints)(((int)ConnectionPoints) >> 2);
            ConnectionPoints = (ConnectionPoints)((int)ConnectionPoints | (overflowBits << 6));
        }
        else
        {
            overflowBits = (int)((ConnectionPoints.Left & ConnectionPoints) | (ConnectionPoints.UpLeft & ConnectionPoints));
            ConnectionPoints = (ConnectionPoints)(((int)ConnectionPoints) << 2);
            ConnectionPoints = (ConnectionPoints)((int)ConnectionPoints | (overflowBits >> 6));
        }
    }

    public void Rotate45(bool clockwise)
    {
        float angle = clockwise ? 45 : -45;
        this.transform.Rotate(Vector3.forward, angle);
        int overflowBits = 0;
        // Some magical shit
        if (clockwise)
        {
            overflowBits = (int)((ConnectionPoints.Up & ConnectionPoints));
            ConnectionPoints = (ConnectionPoints)(((int)ConnectionPoints) >> 1);
            ConnectionPoints = (ConnectionPoints)((int)ConnectionPoints | (overflowBits << 7));
        }
        else
        {
            overflowBits = (int)(ConnectionPoints.UpLeft & ConnectionPoints);
            ConnectionPoints = (ConnectionPoints)(((int)ConnectionPoints) << 1);
            ConnectionPoints = (ConnectionPoints)((int)ConnectionPoints | (overflowBits >> 7));
        }
    }

    void OnDestroy()
    {
        OnDestroyed?.Invoke(this, EventArgs.Empty);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Damage whatever was hit
    }
}
