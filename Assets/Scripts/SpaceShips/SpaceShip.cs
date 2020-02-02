using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceShip : MonoBehaviour
{
    public float colliderSize = ShipGenerator.ShipPartSize;

    public ShipBase BasePart;
    public int PlayerNumber => BasePart.PlayerNumber;
    // Are we building the ship or using it?
    private bool isFunctional = false;
    public bool IsFunctional
    {
        get => isFunctional;
        set
        {
            isFunctional = value;
            if (value)
            {
                var rb = GetComponent<Rigidbody2D>();
                rb.freezeRotation = false;
                rb.constraints = RigidbodyConstraints2D.None;
                foreach(var c in colliders)
                {
                    if (c != null)
                    {
                        c.enabled = true;
                    }
                }
            }
        }
    }
    // 4,4 is the center part
    private ShipPart[,] parts = new ShipPart[9,9];
    private BoxCollider2D[,] colliders = new BoxCollider2D[9, 9];

    private Rigidbody2D rigidbody;

    private void Start()
    {
        parts[4, 4] = BasePart;
        colliders[4, 4] = gameObject.AddComponent<BoxCollider2D>();
        colliders[4, 4].size = new Vector2(colliderSize, colliderSize);
        colliders[4, 4].offset = new Vector3(0, 0);
        BasePart.MotherShip = this;
        BasePart.OnDestroyed += delegate { RemovePart(4, 4); };
        rigidbody = GetComponent<Rigidbody2D>();
    }

    public void ApplyThrust(float amount, Vector2 position, Vector2 direction)
    {
        rigidbody.AddForceAtPosition(direction.normalized * amount, position);
    }

    public void AddPart(int x, int y, ShipPart part)
    {
        if (ValidatePartPosition(x, y, part.ConnectionPoints))
        {
            parts[x, y] = part;
            part.MotherShip = this;
            part.OnDestroyed += delegate { RemovePart(x, y); };
            colliders[x, y] = gameObject.AddComponent<BoxCollider2D>();
            colliders[x, y].size = new Vector2(colliderSize, colliderSize);
            colliders[x,y].offset = new Vector3((x - 4) * colliderSize, (y - 4) * colliderSize );
            colliders[x, y].enabled = false;
            part.transform.SetParent(transform);
        }
    }

    // Only called by callbacks from the parts when they die
    private void RemovePart(int x, int y)
    {
        parts[x, y] = null;
        Destroy(colliders[x, y]);
        // Center is the core!
        if (x == 4 && y == 4)
        {
            DestroyWholeShip();
        }
        else
        {
            CheckIntegrityOfShip();
        }
    }

    private void DestroyWholeShip()
    {
        foreach(var p in parts)
        {
            if (p != null)
            {
                p.Explode();
            }
        }
        GameController.Instance.PlayerWin(PlayerNumber == 1 ? GameController.Team.Red : GameController.Team.Blue);
        Destroy(gameObject);
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
            parts[x,y] == null && (
            (y < 8 &&          
        points.HasFlag(ConnectionPoints.Up) &&  
        parts[x,     y + 1] != null && parts[x,     y + 1].CanHostPoints.HasFlag(ConnectionPoints.Down)) ||
            (y < 8 && x < 8 && 
        points.HasFlag(ConnectionPoints.UpRight) && 
        parts[x + 1, y + 1] != null && parts[x + 1, y + 1].CanHostPoints.HasFlag(ConnectionPoints.DownLeft)) ||
            (x < 8 && 
        points.HasFlag(ConnectionPoints.Right) && 
        parts[x + 1, y    ] != null && parts[x + 1, y    ].CanHostPoints.HasFlag(ConnectionPoints.Left)) ||
            (y > 0 && x < 8 && 
        points.HasFlag(ConnectionPoints.DownRight) && 
        parts[x + 1, y - 1] != null && parts[x + 1, y - 1].CanHostPoints.HasFlag(ConnectionPoints.UpLeft)) ||
            (y > 0 && 
        points.HasFlag(ConnectionPoints.Down) && 
        parts[x,     y - 1] != null && parts[x,     y - 1].CanHostPoints.HasFlag(ConnectionPoints.Up)) ||
            (y > 0 && x > 0 &&
        points.HasFlag(ConnectionPoints.DownLeft) && 
        parts[x - 1, y - 1] != null && parts[x - 1, y - 1].CanHostPoints.HasFlag(ConnectionPoints.UpRight)) ||
            (x > 0 &&
        points.HasFlag(ConnectionPoints.Left) && 
        parts[x - 1, y    ] != null && parts[x - 1, y    ].CanHostPoints.HasFlag(ConnectionPoints.Right)) ||
            (y < 8 && x > 0 &&
        points.HasFlag(ConnectionPoints.UpLeft) && 
        parts[x - 1, y + 1] != null && parts[x - 1, y + 1].CanHostPoints.HasFlag(ConnectionPoints.DownRight)));

    private void OnCollisionEnter2D(Collision2D collision)
    {
        for (int i = 0; i < 9; i++)
        {
            for (int j = 0; j < 9; j++)
            {
                if (colliders[i, j] == collision.otherCollider)
                {
                    parts[i, j].TakeDamage(1);
                    goto BREAK;
                }
            }
        }
        print("not found");
    BREAK:;
    }

}
