using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Controller for Space battle scene
public class SpaceBattleController : MonoBehaviour
{

    [SerializeField]
    private SpaceShip Player1;
    //[SerializeField]
    //private SpaceShip Player2;

    [SerializeField]
    private float y;
    [SerializeField]
    private float x;

    private void Update()
    {
        // If spaceship goes ot of bounds it teleports to the other side
        CheckAndTeleportSpaceShipOutOfBounds(Player1);
        //CheckAndTeleportSpaceShipOutOfBounds(Player2);
    }

    private void CheckAndTeleportSpaceShipOutOfBounds(SpaceShip ship)
    {
        if (Mathf.Abs(ship.transform.position.y) > y / 2f)
        {
            ship.transform.position = new Vector2(ship.transform.position.x, -ship.transform.position.y * 0.9f);
        }
        if (Mathf.Abs(ship.transform.position.x) > x / 2f)
        {
            ship.transform.position = new Vector2(-ship.transform.position.x * 0.9f, ship.transform.position.y);
        }
    }

}
