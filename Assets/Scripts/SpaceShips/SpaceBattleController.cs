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
    private Bounds levelBounds;

    private void Update()
    {
        // If spaceship goes ot of bounds it teleports to the other side
        CheckAndTeleportSpaceShipOutOfBounds(Player1);
        //CheckAndTeleportSpaceShipOutOfBounds(Player2);
    }

    private void CheckAndTeleportSpaceShipOutOfBounds(SpaceShip ship)
    {
        if (!levelBounds.Contains(ship.transform.position))
        {
            ship.transform.position = -1 * ship.transform.position;
            var distFromLevelBorder = Mathf.Sqrt( levelBounds.SqrDistance(ship.transform.position));
            ship.transform.Translate(-ship.transform.position * (distFromLevelBorder + 0.01f), Space.World);
        }
    }

}
