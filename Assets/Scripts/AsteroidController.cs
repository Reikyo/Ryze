using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsteroidController : MonoBehaviour
{
    private SpawnManager spawnManager;

    // Movement:
    private Rigidbody rbAsteroid;

    // Appearance:
    private Material matAsteroid;

    // Health:
    private Health healthAsteroid;
    public float fTimeFlashDamaged = 0.1f;

    // Damage:
    public int iDamage = 10;

    // ------------------------------------------------------------------------------------------------

    void Start()
    {
        spawnManager = GameObject.Find("Spawn Manager").GetComponent<SpawnManager>();

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

    private void OnTriggerEnter(Collider collider)
    {
        if (    collider.gameObject.CompareTag("ProjectilePlayer")
            ||  collider.gameObject.CompareTag("ProjectileEnemy") )
        {
            healthAsteroid.Change(-collider.gameObject.GetComponent<ProjectileController>().iDamage);
            Destroy(collider.gameObject);
            if (healthAsteroid.iHealth == 0)
            {
                spawnManager.SpawnExplosionAsteroid(transform.position);
                Destroy(gameObject);
            }
            StartCoroutine(FlashDamaged());
        }
    }

    // ------------------------------------------------------------------------------------------------

}
