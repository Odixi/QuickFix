using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float Speed = 10;
    public int Damage = 1;
    public float TimeAlive = 3;

    private float timeLived = 0;

    private void Start()
    {
        GetComponent<Rigidbody2D>().velocity = transform.up * Speed;
    }

    private void Update()
    {
        timeLived += Time.deltaTime;
        if (timeLived > TimeAlive)
        {
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // TODO handle damage
        Destroy(gameObject);
    }
}
