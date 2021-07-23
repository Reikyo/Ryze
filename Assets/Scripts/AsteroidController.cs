using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsteroidController : MonoBehaviour
{
    // Movement:
    private Rigidbody rbAsteroid;

    // Appearance:
    private Material matAsteroid;

    // Health:
    private Health healthAsteroid;
    public float fTimeFlashDamaged = 0.1f;

    // Damage:
    public int iDamage = 1;

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

        matAsteroid = GetComponent<Renderer>().material;

        healthAsteroid = GetComponent<Health>();
    }

    // ------------------------------------------------------------------------------------------------

    IEnumerator FlashDamaged()
    {
        matAsteroid.EnableKeyword("_EMISSION");
        yield return new WaitForSeconds(fTimeFlashDamaged);
        matAsteroid.DisableKeyword("_EMISSION");
    }

    // ------------------------------------------------------------------------------------------------

    private void OnCollisionEnter(Collision collision)
    {
        if (    collision.gameObject.CompareTag("ProjectilePlayer")
            ||  collision.gameObject.CompareTag("ProjectileEnemy") )
        {
            healthAsteroid.Change(-collision.gameObject.GetComponent<ProjectileController>().iDamage);
            if (healthAsteroid.iHealth == 0)
            {
                Destroy(gameObject);
            }
            StartCoroutine(FlashDamaged());
        }
    }

    // ------------------------------------------------------------------------------------------------

}
