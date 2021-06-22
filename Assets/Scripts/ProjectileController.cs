using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileController : MonoBehaviour
{
    // Movement:
    public float fForceMove = 500f;
    private Rigidbody rbProjectile;

    // Attack:
    public int iDamage = 10;

    // From other objects:
    private Health healthCollision;

    // ------------------------------------------------------------------------------------------------

    void Start()
    {
        rbProjectile = GetComponent<Rigidbody>();
        rbProjectile.AddForce(fForceMove * Vector3.forward, ForceMode.Impulse);
    }

    // ------------------------------------------------------------------------------------------------

    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.CompareTag("Player"))
        {
            healthCollision = collision.gameObject.GetComponent<Health>();
            if (healthCollision)
            {
                healthCollision.Change(-iDamage);
            }
            Destroy(gameObject);
        }
    }

    // ------------------------------------------------------------------------------------------------

}
