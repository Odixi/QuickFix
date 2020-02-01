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
            timeFromLastShot += Time.deltaTime;
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
            bullet.transform.position = transform.position + transform.up* 0.5f;
            timeFromLastShot = 0;
        }
    }

}
