using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipGenerator : MonoBehaviour
{
    private const float ActionInterval = 0.25f;
    private const float ShipPartSize = 5;

    public ShipPart testPart;

    [SerializeField]
    private int PlayerNumber;
    [SerializeField]
    private SpaceShip ship;
    private int selectedX = -1;
    private int selectedY = -1;

    private ShipPart CurrentPlacingPart = null;

    private bool isPlacing = false;

    private float timeFromHorizontalAction = ActionInterval;
    private float timeFromVerticalAction = ActionInterval;

    private void Start()
    {
        // testing
        StartPlacingPart(Instantiate(testPart));
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

            if (Mathf.Abs(hor) > 0.4f &&  timeFromHorizontalAction > ActionInterval)
            {
                var newX = hor > 0 ? Mathf.Min(selectedX+1, 8) : Mathf.Max(selectedX-1, 0);
                if (ship.ValidatePartPosition(newX, selectedY, CurrentPlacingPart.ConnectionPoints))
                {
                    selectedX = newX;
                }
                timeFromHorizontalAction = 0;
            }
            if (Mathf.Abs(ver) > 0.4f && timeFromVerticalAction > ActionInterval)
            {
                var newY = ver > 0 ? Mathf.Min(selectedY + 1, 8) : Mathf.Max(selectedY - 1, 0);
                if (ship.ValidatePartPosition(selectedX, newY, CurrentPlacingPart.ConnectionPoints))
                {
                    selectedY = newY;
                }
                timeFromVerticalAction = 0;
            }

            if (CurrentPlacingPart != null)
            {
                CurrentPlacingPart.transform.position = transform.position + new Vector3(
                    (selectedX-4)*ShipPartSize, (selectedY - 4) * ShipPartSize
                    );
            }

            if (Input.GetButton($"P{PlayerNumber}Action"))
            {
                ship.AddPart(selectedX, selectedY, CurrentPlacingPart);
                isPlacing = false;

                // testing
                StartPlacingPart(Instantiate(testPart));
            }
        }
    }

    public void StartPlacingPart(ShipPart partToPlace) // callback when ready?
    {
        CurrentPlacingPart = partToPlace;
        isPlacing = true;
    }

    private void ChangeSelection(int x, int y)
    {
        selectedX = x;
        selectedY = y;
        // update higlight
    }
}
