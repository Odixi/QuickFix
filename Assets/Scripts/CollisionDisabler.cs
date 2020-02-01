using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionDisabler : MonoBehaviour
{
    public int PlayerNumber = 1;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        JumpThrough script = collision.GetComponent<JumpThrough>();
        if (script == null) return;
        if (PlayerNumber == 1) script.player1CollisionDisablerIn = true;
        else script.player2CollisionDisablerIn = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        JumpThrough script = collision.GetComponent<JumpThrough>();
        if (script == null) return;
        if (PlayerNumber == 1) script.player1CollisionDisablerIn = false;
        else script.player2CollisionDisablerIn = false;
    }
}
