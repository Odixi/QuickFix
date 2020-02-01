using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The base of the ship. Allways in [4,4] in ship part grid
/// </summary>
public class ShipBase : ShipPart
{
    public int PlayerNumber = 1;
    public GameObject Player1PilotInside;
    public GameObject Player2PilotInside;

    public override void Start()
    {
        base.Start();
        ConnectionPoints = ConnectionPoints.All;
    }

    public void SetPilotsInside()
    {
        foreach(var g in GraphicsGameObjects)
        {
            g.SetActive(false);
        }
        Player1PilotInside.SetActive(PlayerNumber == 1);
        Player1PilotInside.SetActive(PlayerNumber == 2);

    }

    // TODO destroy ship when this part blows


}
