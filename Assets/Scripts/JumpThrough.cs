using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpThrough : MonoBehaviour
{
    public bool player1CollisionDisablerIn = false;
    public bool player1CollisionEnablerIn = false;
    public bool player2CollisionDisablerIn = false;
    public bool player2CollisionEnablerIn = false;

    private void Update()
    {
        if (player1CollisionDisablerIn && player2CollisionDisablerIn) gameObject.layer = 10;
        else if (player1CollisionDisablerIn) gameObject.layer = 8;
        else if (player2CollisionDisablerIn) gameObject.layer = 9;
        else if (player1CollisionEnablerIn || player2CollisionEnablerIn) gameObject.layer = 0;
    }
}
