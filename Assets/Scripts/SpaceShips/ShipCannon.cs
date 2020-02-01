using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipCannon : ShipPart
{
    public float ShootDelay = 0.2f;
    private float timeFromLastShot = 0.2f;

    public GameObject BulletPrefab;

    private void Update()
    {
        if (MotherShip != null && MotherShip.IsFunctional)
        {
            if (Input.GetButton($"P{MotherShip.PlayerNumber}Action"))
            {
                Shoot();
            }
        }
    }

    private void Shoot()
    {
        if (timeFromLastShot > ShootDelay)
        {
            var bullet = Instantiate(BulletPrefab);
            bullet.transform.rotation = transform.rotation;
        }
    }

}
