using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceShip : MonoBehaviour
{
    [SerializeField]
    private ShipBase BasePart;
    // 4,4 is the center part
    private ShipPart[,] parts = new ShipPart[9,9];

    private void Start()
    {
        parts[4, 4] = BasePart;
    }

    public void AddPart(int x, int y, ShipPart part)
    {
        if (ValidatePartPosition(x, y, part.ConnectionPoints))
        {
            parts[x, y] = part;
            part.OnDestroyed += delegate { RemovePart(x, y); };
        }
    }

    // Only called by callbacks from the parts when they die
    private void RemovePart(int x, int y)
    {
        parts[x, y] = null;
        // TODO check wether others should die as well
        // TODO check if was center part
    }

    // Check wether the ship has floating parts and destroy them
    public void CheckIntegrityOfShip()
    {
        List<(int,int)> validParts = new List<(int,int)>();
        var centerPart = ( 4, 4 );
        validParts.Add(centerPart);
        CheckNeighbours(validParts, 4, 4, ConnectionPoints.All);
        for (int i = 0; i < 9; i++)
        {
            for (int j = 0; j < 9; j++)
            {
                if (!validParts.Contains((i, j)))
                {
                    if (parts[i,j] != null)
                    {
                        parts[i, j].Explode();
                    }
                }
            }
        }
    }

    public void CheckNeighbours(List<(int,int)> alreadyChecked, int x, int y, ConnectionPoints connections)
    {
        if (connections.HasFlag(ConnectionPoints.Up))
        {
            if (!alreadyChecked.Contains((x,y + 1)) && y < 8 && parts[x, y + 1] != null && parts[x, y + 1].ConnectionPoints.HasFlag(ConnectionPoints.Down))
            {
                alreadyChecked.Add((x, y + 1));
                CheckNeighbours(alreadyChecked, x, y + 1, parts[x, y + 1].ConnectionPoints);
            } 
        }
        if (connections.HasFlag(ConnectionPoints.UpRight))
        {
            if (!alreadyChecked.Contains((x + 1, y + 1)) && y < 8 && x < 8 && parts[x + 1, y + 1] != null && parts[x + 1, y + 1].ConnectionPoints.HasFlag(ConnectionPoints.DownLeft))
            {
                alreadyChecked.Add((x + 1, y + 1));
                CheckNeighbours(alreadyChecked, x + 1, y + 1, parts[x + 1, y + 1].ConnectionPoints);
            }
        }
        if (connections.HasFlag(ConnectionPoints.Right))
        {
            if (!alreadyChecked.Contains((x + 1, y ))  && x < 8 && parts[x + 1, y] != null && parts[x + 1, y].ConnectionPoints.HasFlag(ConnectionPoints.Left))
            {
                alreadyChecked.Add((x + 1, y));
                CheckNeighbours(alreadyChecked, x + 1, y, parts[x + 1, y].ConnectionPoints);
            }
        }
        if (connections.HasFlag(ConnectionPoints.DownRight))
        {
            if (!alreadyChecked.Contains((x + 1, y - 1)) && y > 0 && x < 8 && parts[x + 1, y - 1] != null && parts[x + 1, y - 1].ConnectionPoints.HasFlag(ConnectionPoints.UpLeft))
            {
                alreadyChecked.Add((x + 1, y - 1));
                CheckNeighbours(alreadyChecked, x + 1, y - 1, parts[x + 1, y - 1].ConnectionPoints);
            }
        }
        if (connections.HasFlag(ConnectionPoints.Down))
        {
            if (!alreadyChecked.Contains((x , y - 1)) && y > 0 && parts[x , y - 1] != null && parts[x, y - 1].ConnectionPoints.HasFlag(ConnectionPoints.Up))
            {
                alreadyChecked.Add((x , y - 1));
                CheckNeighbours(alreadyChecked, x , y - 1, parts[x , y - 1].ConnectionPoints);
            }
        }
        if (connections.HasFlag(ConnectionPoints.DownLeft))
        {
            if (!alreadyChecked.Contains((x - 1, y - 1)) && y > 0 && x > 0 && parts[x - 1, y - 1] != null && parts[x - 1, y - 1].ConnectionPoints.HasFlag(ConnectionPoints.UpRight))
            {
                alreadyChecked.Add((x - 1, y - 1));
                CheckNeighbours(alreadyChecked, x - 1, y - 1, parts[x - 1, y - 1].ConnectionPoints);
            }
        }
        if (connections.HasFlag(ConnectionPoints.Left))
        {
            if (!alreadyChecked.Contains((x - 1, y)) && x > 0 && parts[x - 1, y] != null && parts[x - 1, y].ConnectionPoints.HasFlag(ConnectionPoints.Right))
            {
                alreadyChecked.Add((x - 1, y));
                CheckNeighbours(alreadyChecked, x - 1, y, parts[x - 1, y].ConnectionPoints);
            }
        }
        if (connections.HasFlag(ConnectionPoints.UpLeft))
        {
            if (!alreadyChecked.Contains((x - 1, y + 1)) && y < 8 && x > 0 && parts[x - 1, y + 1] != null && parts[x - 1, y + 1].ConnectionPoints.HasFlag(ConnectionPoints.DownRight))
            {
                alreadyChecked.Add((x - 1, y + 1));
                CheckNeighbours(alreadyChecked, x - 1, y + 1, parts[x - 1, y + 1].ConnectionPoints);
            }
        }
    }


    // Validate if position is valid
    public bool ValidatePartPosition(int x, int y, ConnectionPoints points) =>
            parts[x,y] == null &&
            (y < 8 &&          parts[x,     y + 1] != null && parts[x,     y + 1].ConnectionPoints.HasFlag(ConnectionPoints.Down)) ||
            (y < 8 && x < 8 && parts[x + 1, y + 1] != null && parts[x + 1, y + 1].ConnectionPoints.HasFlag(ConnectionPoints.DownLeft)) ||
            (x < 8 &&          parts[x + 1, y    ] != null && parts[x + 1, y    ].ConnectionPoints.HasFlag(ConnectionPoints.Left)) ||
            (y > 0 && x < 8 && parts[x + 1, y - 1] != null && parts[x + 1, y - 1].ConnectionPoints.HasFlag(ConnectionPoints.UpLeft)) ||
            (y > 0 &&          parts[x,     y - 1] != null && parts[x,     y - 1].ConnectionPoints.HasFlag(ConnectionPoints.Up)) ||
            (y > 0 && x > 0 && parts[x - 1, y - 1] != null && parts[x - 1, y - 1].ConnectionPoints.HasFlag(ConnectionPoints.UpRight)) ||
            (x > 0 &&          parts[x - 1, y    ] != null && parts[x - 1, y    ].ConnectionPoints.HasFlag(ConnectionPoints.Right)) ||
            (y < 8 && x > 0 && parts[x - 1, y + 1] != null && parts[x - 1, y + 1].ConnectionPoints.HasFlag(ConnectionPoints.DownRight));

    

}
