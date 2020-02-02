using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionController : MonoBehaviour
{
    public Animator ExplosionAnimator;

    private float timeAlive = 0;

    // Update is called once per frame
    void Update()
    {

        timeAlive += Time.deltaTime;
        if (timeAlive > 0.5f)
        {
            Destroy(gameObject);
        }
    }
}
