using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Throwable : MonoBehaviour
{
    public bool WasThrown = false;
    // Reset WasThrown if speed is less than this
    public float ThrownResetSpeed = 1f;

    private void Update()
    {
        if (gameObject.GetComponent<Rigidbody2D>().velocity.magnitude < ThrownResetSpeed) WasThrown = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player") WasThrown = false;
    }
}
