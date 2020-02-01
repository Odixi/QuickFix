using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlatformingPlayer : MonoBehaviour
{
    public float AccelerationForce = 10f;
    public float MaxSpeed = 5f;
    public float JumpForce = 1f;
    public float MovementFloatiness = 1f;
    public float ThrowForce = 5f;
    public float MaxPickupDistance = 1f;
    public float KickDistance = 1f;
    public float KickForce = 5f;
    public float DropForce = 2f;
    public float ThrowableMinVelocityToDrop = 2f;
    public float MaxVelocity = 20f;
    public float WallMinDistanceToMove = 0.3f;
    public float FeetCheckRadius = 0.5f;
    public int PlayerNumber;
    public Transform Feet;
    public Transform Hands;
    public GameObject CarriedPart = null;
    public bool IsPlacingPart = false;
    public Rigidbody2D rigidbody;
    private bool jumpUsed;
    private bool jumpButtonReleased = true;
    private bool kickButtonReleased = true;
    private bool pickupButtonReleasedAfterLastAction = true;
    private Vector2 lastFacingDirection = Vector2.right;
    private Animator animator;

    private bool isGrounded => Physics2D.CircleCast(transform.position, FeetCheckRadius, Vector2.down, Vector2.Distance(transform.position, Feet.position)).collider != null;

    void Start()
    {
        rigidbody = gameObject.GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (IsPlacingPart)
        {
            return;
        }

        float inputY = Input.GetAxis("P" + PlayerNumber + "Jump");
        float inputX = Input.GetAxis("P" + PlayerNumber + "Horizontal");

        bool ascending = !isGrounded && rigidbody.velocity.y > 1f;
        bool descending = !isGrounded && rigidbody.velocity.y < -1f;
        bool moving = !ascending && !descending && inputX != 0;
        animator.SetBool("Ascending", ascending);
        animator.SetBool("Descending", descending);

        animator.SetBool("Moving", moving);
        animator.speed = moving ? Mathf.Max(0.5f, Mathf.Abs(rigidbody.velocity.x) / 4) : 1;
        int playerFlip = PlayerNumber == 1 ? 1 : -1;
        if (inputX != 0) gameObject.GetComponent<SpriteRenderer>().flipX = inputX * playerFlip > 0;

        if (inputY == 0) jumpButtonReleased = true;
        if (isGrounded)
        {
            if (inputY > 0 && !jumpUsed) Jump();
            if (jumpButtonReleased) jumpUsed = false;
        }
        if (Input.GetAxis("P" + PlayerNumber + "Kick") > 0)
        {
            if (kickButtonReleased) Kick();
            kickButtonReleased = false;
        } else kickButtonReleased = true;
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
        rigidbody.velocity = Vector2.ClampMagnitude(rigidbody.velocity, MaxVelocity);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag != "Ship Part") return;
        if (collision.gameObject.GetComponent<Rigidbody2D>().velocity.magnitude < ThrowableMinVelocityToDrop) return;
        if (!collision.gameObject.GetComponent<Throwable>().WasThrown) return;
        DropCarriedPart();
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

    void Kick()
    {
        GameObject target = GetKickTarget();
        if (target == null) return;
        Vector2 kickDirection = (target.transform.position - transform.position);
        kickDirection.y = 0;
        kickDirection.Normalize();
        kickDirection.y = 0.2f;
        kickDirection.Normalize();
        target.transform.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        target.transform.GetComponent<Rigidbody2D>().AddForce(kickDirection * KickForce, ForceMode2D.Impulse);
        if (target.tag == "Player") target.GetComponent<PlatformingPlayer>().DropCarriedPart();
        if (target.tag == "Ship Part") target.GetComponent<Throwable>().WasThrown = true;
    }

    List<GameObject> KickableObjects()
    {
        List<GameObject> list = new List<GameObject>();
        list.AddRange(GameObject.FindGameObjectsWithTag("Player"));
        list.AddRange(GameObject.FindGameObjectsWithTag("Ship Part"));
        return list.Where(x => IsKickable(x)).ToList();
    }

    GameObject ClosestKickable()
    {
        List<GameObject> objects = KickableObjects();
        if (objects.Count == 0) return null;
        GameObject closest = objects[0];
        foreach (GameObject obj in objects)
        {
            float closestDistance = Vector2.Distance(transform.position, closest.transform.position);
            float currentDistance = Vector2.Distance(transform.position, obj.transform.position);
            // Prefer kicking players
            if (closest.tag == "Player" && obj.tag != "Player") continue;
            if (currentDistance < closestDistance) closest = obj;
        }
        return closest;
    }

    bool IsKickable(GameObject gameObject)
    {
        if (Vector2.Distance(gameObject.transform.position, transform.position) > KickDistance) return false;
        Vector2 horizontalDirection = gameObject.transform.position - transform.position;
        horizontalDirection.y = 0;
        horizontalDirection.Normalize();
        return horizontalDirection == lastFacingDirection;
    }

    GameObject GetKickTarget()
    {
        return ClosestKickable();
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
            rigidbody.AddForce(new Vector2(inputX, 0) * AccelerationForce);
        }
        if (inputX == 0) rigidbody.velocity -= (rigidbody.velocity - new Vector2(0, rigidbody.velocity.y)) * Time.deltaTime / MovementFloatiness;
    }
    
    void PickupPart(GameObject part)
    {
        CarriedPart = part;
        CarriedPart.GetComponent<Rigidbody2D>().isKinematic = true;
        CarriedPart.GetComponent<BoxCollider2D>().enabled = false;
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
        if (closestPart == null) return;
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
        var rb = CarriedPart.GetComponent<Rigidbody2D>();
        rb.isKinematic = false;
        rb.velocity = Vector2.zero;
        rb.AddForce(lastFacingDirection * ThrowForce, ForceMode2D.Impulse);
        CarriedPart.GetComponent<Throwable>().WasThrown = true;
        CarriedPart.GetComponent<BoxCollider2D>().enabled = true;

        CarriedPart = null;
    }

    void DropCarriedPart()
    {
        if (CarriedPart == null) return;
        CarriedPart.transform.SetParent(transform.parent);
        CarriedPart.GetComponent<Rigidbody2D>().isKinematic = false;
        CarriedPart.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        CarriedPart.GetComponent<Rigidbody2D>().AddForce(Vector2.up * DropForce, ForceMode2D.Impulse);
        CarriedPart.GetComponent<BoxCollider2D>().enabled = true;
        CarriedPart = null;
    }
}
