using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionController : MonoBehaviour
{
    public ParticleSystem explosionParticleSystem;

    private float timeAlive = 0;

    // Update is called once per frame
    void Update()
    {
        timeAlive += Time.deltaTime;
        if (timeAlive > explosionParticleSystem.startLifetime + explosionParticleSystem.duration)
        {
            Destroy(gameObject);
        }
    }
}
