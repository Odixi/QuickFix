using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PartType
{
    Body = 0,
    Thruster = 1,
    Cannon = 2,
    Tip = 3,
    Wing = 4,
    WingMirrored = 5
}

public class Throwable : MonoBehaviour
{
    public bool WasThrown = false;
    // Reset WasThrown if speed is less than this
    public float ThrownResetSpeed = 1f;

    public PartType type;

    private void Update()
    {
        if (gameObject.GetComponent<Rigidbody2D>().velocity.magnitude < ThrownResetSpeed) WasThrown = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player") WasThrown = false;

        if (GameController.Instance.State == GameState.Platform)
        {
            AudioSource aud;
            if (TryGetComponent<AudioSource>(out aud))
            {
                aud.volume = collision.relativeVelocity.magnitude / 20f;
                aud.Play();
            }

        }

    }
}
