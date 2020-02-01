using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipThruster : ShipPart
{
    public float ThrustForce = 20;
    public ThrusterAnimation animation;

    public Vector2 Direction
    {
        get
        {
            if (MotherShip != null)
            {
                return MotherShip.transform.InverseTransformDirection(transform.up).normalized;
            }
            return Vector2.up;
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
            var dot = Mathf.Max(Vector2.Dot(Direction, force), 0);

            MotherShip.ApplyThrust(dot * ThrustForce, transform.position, transform.up);
            animation.TargetSize = dot;
        }
    }
}
