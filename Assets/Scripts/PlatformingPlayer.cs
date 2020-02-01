using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformingPlayer : MonoBehaviour
{
    public float AccelerationForce = 10f;
    public float MaxSpeed = 5f;
    public float JumpForce = 1f;
    public float MovementFloatiness = 1f;
    public float ThrowForce = 5f;
    public float MaxPickupDistance = 1f;
    public int PlayerNumber;
    public Transform Feet;
    public Transform Hands;
    private Rigidbody2D rigidbody;
    private bool jumpUsed;
    private bool jumpButtonReleased = true;
    private bool pickupButtonReleasedAfterLastAction = true;
    private GameObject carriedPart = null;
    private Vector2 lastMoveDirection = Vector2.right;

    private bool isGrounded => Physics2D.Raycast(Feet.position, Vector2.down, 0.02f).collider != null;

    void Start()
    {
        PlayerNumber = 1;
        rigidbody = gameObject.GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (rigidbody.velocity.x != 0)
        {
            lastMoveDirection = rigidbody.velocity.x > 0 ? Vector2.right : Vector2.left;
        }
        float inputY = Input.GetAxis("P" + PlayerNumber + "Vertical");

        if (inputY == 0) jumpButtonReleased = true;
        if (isGrounded)
        {
            if (inputY > 0 && !jumpUsed) Jump();
            if (jumpButtonReleased) jumpUsed = false;
        }
        Move();
        if (carriedPart != null) CarryPart(carriedPart);
        if (Input.GetAxis("P" + PlayerNumber + "Action") > 0) {
            if (pickupButtonReleasedAfterLastAction)
            {
                pickupButtonReleasedAfterLastAction = false;
                if (carriedPart == null) PickupClosestPart();
                else ThrowCarriedPart();
            }
        } else pickupButtonReleasedAfterLastAction = true;
    }

    void Jump()
    {
        rigidbody.velocity = rigidbody.velocity - new Vector2(0, rigidbody.velocity.y);
        rigidbody.AddForce(Vector2.up * JumpForce, ForceMode2D.Impulse);
        jumpUsed = true;
        jumpButtonReleased = false;
    }

    void CarryPart(GameObject part)
    {
        part.transform.position = Vector3.Lerp(part.transform.position, Hands.position, Time.deltaTime * 50);
    }

    void Move()
    {
        float inputX = Input.GetAxis("P" + PlayerNumber + "Horizontal");
        if ((rigidbody.velocity.x < MaxSpeed && inputX > 0) || (rigidbody.velocity.x > -MaxSpeed && inputX < 0))
        {
            rigidbody.AddForce(new Vector2(inputX * AccelerationForce, 0));
        }
        if (inputX == 0) rigidbody.velocity -= (rigidbody.velocity - new Vector2(0, rigidbody.velocity.y)) * Time.deltaTime / MovementFloatiness;
    }

    void PickupPart(GameObject part)
    {
        carriedPart = part;
        carriedPart.GetComponent<Rigidbody2D>().isKinematic = true;
        part.transform.SetParent(Hands);
    }

    void PickupClosestPart()
    {
        GameObject closestPart = null;
        foreach (GameObject part in GameObject.FindGameObjectsWithTag("Ship Part"))
        {
            if (closestPart == null)
            {
                closestPart = part;
                continue;
            }
            float partDistance = Vector3.Distance(part.transform.position, transform.position);
            float closestPartDistance = Vector3.Distance(closestPart.transform.position, transform.position);
            if (partDistance < closestPartDistance) closestPart = part;
        }
        if (Vector2.Distance(transform.position, closestPart.transform.position) < MaxPickupDistance)
        {
            PickupPart(closestPart);
        }
    }

    void ThrowCarriedPart()
    {
        carriedPart.transform.SetParent(transform.parent);
        carriedPart.GetComponent<Rigidbody2D>().isKinematic = false;
        carriedPart.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        carriedPart.GetComponent<Rigidbody2D>().AddForce(lastMoveDirection * ThrowForce, ForceMode2D.Impulse);
        carriedPart = null;
    }
}
