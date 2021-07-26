using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileController : MonoBehaviour
{
    // Movement:
    public float fForceMove = 200f; // Player: 200; Enemy: 100
    private Rigidbody rbProjectile;

    // Health:
    public bool bTriggeredDestroy = false;

    // Damage:
    public int iDamage = 10; // Player: 10; Enemy: 10

    // ------------------------------------------------------------------------------------------------

    void Start()
    {
        rbProjectile = GetComponent<Rigidbody>();
        rbProjectile.AddRelativeForce(fForceMove * Vector3.forward, ForceMode.Impulse);
    }

    // ------------------------------------------------------------------------------------------------

}
