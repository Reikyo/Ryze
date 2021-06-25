using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    // Movement:
    private NavMeshAgent navEnemy;

    // Appearance:
    private Material matEnemy;

    // Health:
    private Health healthEnemy;
    public float fTimeFlashDamaged = 0.1f;

    // Damage:
    public GameObject goProjectile;
    private GameObject goGunMiddleProjectileSpawnPoint;
    private float fTimeNextFire;
    public float fTimeNextFireDelta = 0.2f;

    // From other objects:
    private GameObject goPlayer;

    // ------------------------------------------------------------------------------------------------

    void Start()
    {
        navEnemy = GetComponent<NavMeshAgent>();
        navEnemy.SetDestination(new Vector3(-50f, 0f, 20f));

        matEnemy = transform.Find("Chasis").gameObject.GetComponent<Renderer>().material;

        healthEnemy = GetComponent<Health>();

        goGunMiddleProjectileSpawnPoint = transform.Find("Chasis/GunMiddleProjectileSpawnPoint").gameObject;
        fTimeNextFire = Time.time;

        goPlayer = GameObject.FindWithTag("Player");
    }

    // ------------------------------------------------------------------------------------------------

    void Update()
    {
        Vector3 v3PosRelativePlayer = goPlayer.transform.position - transform.position;
        Vector3 v3DirectionLook = Vector3.RotateTowards(transform.forward, v3PosRelativePlayer, 180f * Time.deltaTime, 0f);
        transform.rotation = Quaternion.LookRotation(v3DirectionLook);

        if (v3PosRelativePlayer.magnitude <= 200f)
        {
            if (Time.time >= fTimeNextFire)
            {
                Instantiate(goProjectile, goGunMiddleProjectileSpawnPoint.transform.position, goGunMiddleProjectileSpawnPoint.transform.rotation);
                fTimeNextFire = Time.time + fTimeNextFireDelta;
            }
        }
    }

    // ------------------------------------------------------------------------------------------------

    IEnumerator FlashDamaged()
    {
        matEnemy.EnableKeyword("_EMISSION");
        yield return new WaitForSeconds(fTimeFlashDamaged);
        matEnemy.DisableKeyword("_EMISSION");
    }

    // ------------------------------------------------------------------------------------------------

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("PlayerProjectile"))
        {
            healthEnemy.Change(-collision.gameObject.GetComponent<ProjectileController>().iDamage);
            if (healthEnemy.iHealth == 0)
            {
                Destroy(gameObject);
            }
            StartCoroutine(FlashDamaged());
        }
    }

    // ------------------------------------------------------------------------------------------------

}
