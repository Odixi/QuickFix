using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipThruster : ShipPart
{
    public float ThrustForce = 10;

    public Vector2 Direction
    {
        get
        {
            return Vector2.zero;
        }
    }

    private void Update()
    {
        if (MotherShip != null && MotherShip.IsFunctional)
        {
            var hor = Input.GetAxis($"P{MotherShip.PlayerNumber}Horizontal");
            var ver = Input.GetAxis($"P{MotherShip.PlayerNumber}Vertical");
            Vector2 force = new Vector2(hor, ver);
            force = Vector2.ClampMagnitude(force, 1f);


        }
    }
}
