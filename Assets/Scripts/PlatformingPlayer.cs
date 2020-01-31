using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformingPlayer : MonoBehaviour
{
    public float Acceleration = 10f;
    public float JumpForce = 1f;
    public float MovementFloatiness = 1f;
    public int PlayerNumber;
    public Transform Feet;
    private Rigidbody2D rigidbody;
    private bool jumpUsed;
    private bool jumpButtonReleased = true;

    private bool isGrounded => Physics2D.Raycast(Feet.position, Vector2.down, 0.02f).collider != null;

    void Start()
    {
        PlayerNumber = 1;
        rigidbody = gameObject.GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        float inputY = Input.GetAxis("P" + PlayerNumber + "Vertical");

        if (inputY == 0) jumpButtonReleased = true;
        if (isGrounded)
        {
            if (inputY > 0 && !jumpUsed) Jump();
            if (jumpButtonReleased) jumpUsed = false;
        }
        Move();
    }

    void Jump()
    {
        rigidbody.velocity = rigidbody.velocity - new Vector2(0, rigidbody.velocity.y);
        rigidbody.AddForce(Vector2.up * JumpForce, ForceMode2D.Impulse);
        jumpUsed = true;
        jumpButtonReleased = false;
    }

    void Move()
    {
        float inputX = Input.GetAxis("P" + PlayerNumber + "Horizontal");

        rigidbody.AddForce(new Vector2(inputX * Acceleration, 0));
        if (inputX == 0)
        {
            rigidbody.velocity -= rigidbody.velocity * Time.deltaTime * (1 / MovementFloatiness);
        }
    }
}
