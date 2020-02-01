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
    GameObject CarriedPart = null;
    private Rigidbody2D rigidbody;
    private bool jumpUsed;
    private bool jumpButtonReleased = true;
    private bool pickupButtonReleasedAfterLastAction = true;
    private Vector2 lastFacingDirection = Vector2.right;

    private bool isGrounded => Physics2D.Raycast(transform.position, Vector2.down * Vector2.Distance(transform.position, Feet.position), 0.5f).collider != null;

    void Start()
    {
        rigidbody = gameObject.GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        float inputY = Input.GetAxis("P" + PlayerNumber + "Jump");

        if (inputY == 0) jumpButtonReleased = true;
        if (isGrounded)
        {
            if (inputY > 0 && !jumpUsed) Jump();
            if (jumpButtonReleased) jumpUsed = false;
        }
        Move();
        if (CarriedPart != null) CarryPart(CarriedPart);
        if (Input.GetAxis("P" + PlayerNumber + "Action") > 0) {
            if (pickupButtonReleasedAfterLastAction)
            {
                pickupButtonReleasedAfterLastAction = false;
                if (CarriedPart == null) PickupClosestPart();
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
        if (inputX != 0)
        {
            lastFacingDirection = inputX > 0 ? Vector2.right : Vector2.left;
        }

        if ((rigidbody.velocity.x < MaxSpeed && inputX > 0) || (rigidbody.velocity.x > -MaxSpeed && inputX < 0))
        {
            rigidbody.AddForce(new Vector2(inputX * AccelerationForce, 0));
        }
        if (inputX == 0) rigidbody.velocity -= (rigidbody.velocity - new Vector2(0, rigidbody.velocity.y)) * Time.deltaTime / MovementFloatiness;
    }

    void PickupPart(GameObject part)
    {
        CarriedPart = part;
        CarriedPart.GetComponent<Rigidbody2D>().isKinematic = true;
        part.transform.SetParent(Hands);
    }

    void PickupClosestPart()
    {
        GameObject closestPart = null;
        foreach (GameObject part in GameObject.FindGameObjectsWithTag("Ship Part"))
        {
            if (IsCarriedByPlayer(part)) continue;
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

    bool IsCarriedByPlayer(GameObject part)
    {
        foreach (GameObject player in GameObject.FindGameObjectsWithTag("Player"))
        {
            if (player.GetComponent<PlatformingPlayer>().CarriedPart == part) return true;
        }
        return false;
    }

    void ThrowCarriedPart()
    {
        CarriedPart.transform.SetParent(transform.parent);
        CarriedPart.GetComponent<Rigidbody2D>().isKinematic = false;
        CarriedPart.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        CarriedPart.GetComponent<Rigidbody2D>().AddForce(lastFacingDirection * ThrowForce, ForceMode2D.Impulse);
        CarriedPart = null;
    }
}
