using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnockbackObject : MonoBehaviour
{
    public float KnockbackForce = 10f;
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            collision.rigidbody.velocity = Vector3.zero;
            Vector2 direction = (collision.transform.position - transform.position).normalized;
            Vector3 force = direction * KnockbackForce;
            collision.gameObject.GetComponent<PlatformingPlayer>().Knockback(force);
        }
    }
}
