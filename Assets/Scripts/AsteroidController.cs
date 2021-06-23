using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsteroidController : MonoBehaviour
{
    // Movement:
    private Rigidbody rbAsteroid;
    public float fRelativeMomentumBenchmark = 250000f;
    private float fRelativeMomentum;

    // Damage:
    public int iDamageBenchmark = 10;
    private int iDamage;

    // From other objects:
    private Health healthCollision;

    // ------------------------------------------------------------------------------------------------

    void Start()
    {
        rbAsteroid = GetComponent<Rigidbody>();

        rbAsteroid.AddForce(
            UnityEngine.Random.Range(-1e6f, +1e6f),
            0f,
            UnityEngine.Random.Range(-1e6f, -1e7f)
        );

        rbAsteroid.AddTorque(
            UnityEngine.Random.Range(-1e6f, +1e6f),
            UnityEngine.Random.Range(-1e6f, +1e6f),
            UnityEngine.Random.Range(-1e6f, +1e6f)
        );
    }

    // ------------------------------------------------------------------------------------------------

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            healthCollision = collision.gameObject.GetComponent<Health>();
            if (healthCollision)
            {
                // The approximate extremes we are looking at here for a stationary player are:
                //   1,000 kg asteroid with relative speed of 1 ms^-1 => relative momentum = 1 kg ms^-1
                //   5,000 kg asteroid with relative speed of 50 ms^-1 => relative momentum = 250,000 kg ms^-1
                fRelativeMomentum = rbAsteroid.mass * collision.relativeVelocity.magnitude;
                iDamage = (int) Math.Ceiling(iDamageBenchmark * fRelativeMomentum / fRelativeMomentumBenchmark);
                healthCollision.Change(-iDamage);
            }
        }
    }

    // ------------------------------------------------------------------------------------------------

}
