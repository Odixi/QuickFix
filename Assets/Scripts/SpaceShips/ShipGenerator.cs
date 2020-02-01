using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipGenerator : MonoBehaviour
{
    private const float ActionInterval = 0.15f;
    private float ShipPartSize = 0.7f;

    public ShipPart[] testParts;

    [SerializeField]
    private int PlayerNumber;
    [SerializeField]
    private SpaceShip ship;
    private int selectedX = -1;
    private int selectedY = -1;

    private ShipPart CurrentPlacingPart = null;
    [SerializeField]
    private PartPlacingIndicator partPlacingIndicator;

    private bool isPlacing = false;

    private float timeFromHorizontalAction = ActionInterval;
    private float timeFromVerticalAction = ActionInterval;

    private void Start()
    {
        // testing
        var p = Instantiate(testParts[0]);
        p.transform.parent = ship.transform;
        StartPlacingPart(p);

    }

    // Update is called once per frame
    void Update()
    {
        // Handle input
        if (isPlacing)
        {

            if (selectedX == -1)    selectedX = 4;
            if (selectedY == -1)    selectedY = 4;

            timeFromHorizontalAction += Time.deltaTime;
            timeFromVerticalAction += Time.deltaTime;
            var hor = Input.GetAxis($"P{PlayerNumber}Horizontal");
            var ver = Input.GetAxis($"P{PlayerNumber}Vertical");

            if (Mathf.Abs(hor) > 0.12f &&  timeFromHorizontalAction > ActionInterval)
            {
                var newX = hor > 0 ? Mathf.Min(selectedX+1, 8) : Mathf.Max(selectedX-1, 0);
                selectedX = newX;
                
                timeFromHorizontalAction = 0;
            }
            if (Mathf.Abs(ver) > 0.12f && timeFromVerticalAction > ActionInterval)
            {
                var newY = ver > 0 ? Mathf.Min(selectedY + 1, 8) : Mathf.Max(selectedY - 1, 0);
                selectedY = newY;
                
                timeFromVerticalAction = 0;
            }
            if (Input.GetButtonDown($"P{PlayerNumber}Action2"))
            {
                if (CurrentPlacingPart != null)
                {
                    if (CurrentPlacingPart.CanRotate45)
                    {
                        CurrentPlacingPart.Rotate45(true);
                    }
                    else
                    {
                        CurrentPlacingPart.Rotate90(true);
                    }
                }
            }

            if (CurrentPlacingPart != null)
            {
                Vector3 nPos = transform.position + new Vector3(
                    (selectedX - 4) * ShipPartSize, (selectedY - 4) * ShipPartSize
                    );
                CurrentPlacingPart.transform.position = nPos;
                partPlacingIndicator.transform.position = nPos;
                partPlacingIndicator.SetAllowedState(ship.ValidatePartPosition(selectedX, selectedY, CurrentPlacingPart.ConnectionPoints));
            }


            if (Input.GetButtonDown($"P{PlayerNumber}Action") && ship.ValidatePartPosition(selectedX, selectedY, CurrentPlacingPart.ConnectionPoints))
            {
                ship.AddPart(selectedX, selectedY, CurrentPlacingPart);
                isPlacing = false;

                // testing
                var part = testParts[(int)(Random.value * testParts.Length)];
                var p = Instantiate(part);
                p.transform.parent = ship.transform;
                StartPlacingPart(p);
            }

            if (Input.GetKeyDown(KeyCode.A))
            {
                Destroy(CurrentPlacingPart.gameObject);
                Destroy(partPlacingIndicator.gameObject);
                isPlacing = false;
                ship.IsFunctional = true;
                ship.SetPiecesNonStatic();
            }
        }
    }

    public void StartPlacingPart(ShipPart partToPlace) // callback when ready?
    {
        CurrentPlacingPart = partToPlace;
        CurrentPlacingPart.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
        isPlacing = true;
    }

    private void ChangeSelection(int x, int y)
    {
        selectedX = x;
        selectedY = y;
        // update higlight
    }
}
