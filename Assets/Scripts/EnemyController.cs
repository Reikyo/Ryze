using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    private GameManager gameManager;

    // Movement:
    private NavMeshAgent navEnemy;
    public float fAngSpeedMove = 5f;

    // Appearance:
    private Material matEnemy;

    // Health:
    private Health healthEnemy;
    public float fTimeFlashDamaged = 0.1f;

    // Damage:
    public GameObject goProjectile;
    private GameObject goGunMiddleProjectileSpawnPoint;
    private float fTimeNextFire;
    public float fTimeNextFireDelta = 0.5f;

    // From other objects:
    private GameObject goPlayer;

    // ------------------------------------------------------------------------------------------------

    void Start()
    {
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();

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
        // Movement:

        if (navEnemy.remainingDistance <= navEnemy.stoppingDistance)
        {
            Vector3 v3PositionRandom = new Vector3(gameManager.v3MoveLimitLowerLeft.x-10f, 0f, gameManager.v3MoveLimitLowerLeft.z-10f);

            while ( (   (v3PositionRandom.x < gameManager.v3MoveLimitLowerLeft.x)
                    ||  (v3PositionRandom.x > gameManager.v3MoveLimitUpperRight.x) )
                &&  (   (v3PositionRandom.z < gameManager.v3MoveLimitLowerLeft.z)
                    ||  (v3PositionRandom.z > gameManager.v3MoveLimitUpperRight.z) ) )
            {
                Vector2 v2PositionRandom = Random.insideUnitCircle;
                v3PositionRandom =
                    goPlayer.transform.position
                    + Random.Range(20f, 80f) * (new Vector3(v2PositionRandom.x, 0f, v2PositionRandom.y));
            }

            navEnemy.SetDestination(v3PositionRandom);
        }

        // ------------------------------------------------------------------------------------------------

        // Look:

        Vector3 v3PositionRelativeLook = goPlayer.transform.position - transform.position;
        Vector3 v3PositionRelativeLookNow = Vector3.RotateTowards(transform.forward, v3PositionRelativeLook, fAngSpeedMove * Time.deltaTime, 0f);
        transform.rotation = Quaternion.LookRotation(v3PositionRelativeLookNow);

        // ------------------------------------------------------------------------------------------------

        // Attack:

        if (v3PositionRelativeLook.magnitude <= 200f)
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
