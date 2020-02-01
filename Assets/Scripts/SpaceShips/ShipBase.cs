using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The base of the ship. Allways in [4,4] in ship part grid
/// </summary>
public class ShipBase : ShipPart
{
    void Start()
    {
        ConnectionPoints = ConnectionPoints.All;
    }

    // TODO destroy ship when this part blows


}
