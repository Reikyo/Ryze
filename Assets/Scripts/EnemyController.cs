using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class EnemyController : MonoBehaviour
{
    private GameManager gameManager;
    private SpawnManager spawnManager;
    private AudioManager audioManager;

    // Movement:
    private NavMeshAgent navEnemy;
    public Vector3 v3PositionConstant = new Vector3(0f, 0f, 0f);
    private Vector2 v2PositionRandom;
    private Vector3 v3PositionRandom;
    private bool bDirectionLowerLeft = true;
    public enum MoveMode {random, constant, constanthover, oscillatehorz, oscillatevert};
    public MoveMode moveMode;
    public float fAngSpeedMove = 5f;

    // Appearance:
    private Material matEnemy;
    private LineRenderer lineEngine1;
    private LineRenderer lineEngine2;
    private LineRenderer lineEngine3;
    public float fPosZBase_lineEngine = -2f;
    public float fPosZDelta_lineEngine = 0.25f;
    private float fPosZLower_lineEngine;
    private float fPosZUpper_lineEngine;

    // Health:
    private Health healthEnemy;
    private RectTransform rtCanvasHealth;
    public float fTimeFlashDamaged = 0.1f;
    private bool bTriggeredDestroy = false;

    // Damage:
    public GameObject goProjectile;
    private GameObject goGunMiddleProjectileSpawnPoint;
    private int iNumAttack = 0;
    private int iNumAttackBurst;
    public int iNumMaxAttackBurst = 5;
    private float fTimeNextAttack;
    public float fTimeDeltaAttack = 0.5f;
    public float fTimeDeltaMinAttackBurst = 2f;
    public float fTimeDeltaMaxAttackBurst = 5f;

    // Score:
    public int iScoreDelta = 10;

    // From other objects:
    private GameObject goPlayer;

    // ------------------------------------------------------------------------------------------------

    void Start()
    {
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
        spawnManager = GameObject.Find("Spawn Manager").GetComponent<SpawnManager>();
        audioManager = GameObject.Find("Audio Manager").GetComponent<AudioManager>();

        navEnemy = GetComponent<NavMeshAgent>();

        matEnemy = transform.Find("Chasis").gameObject.GetComponent<Renderer>().material;
        lineEngine1 = transform.Find("Engine Emission (1)/Line : Engine").gameObject.GetComponent<LineRenderer>();
        lineEngine2 = transform.Find("Engine Emission (2)/Line : Engine").gameObject.GetComponent<LineRenderer>();
        lineEngine3 = transform.Find("Engine Emission (3)/Line : Engine").gameObject.GetComponent<LineRenderer>();
        fPosZLower_lineEngine = fPosZBase_lineEngine - fPosZDelta_lineEngine;
        fPosZUpper_lineEngine = fPosZBase_lineEngine + fPosZDelta_lineEngine;

        healthEnemy = gameObject.AddComponent<Health>();
        healthEnemy.sliHealth = transform.Find("Canvas : Health/Slider : Health").GetComponent<Slider>();
        healthEnemy.Change(healthEnemy.iHealthMax);
        rtCanvasHealth = transform.Find("Canvas : Health").GetComponent<RectTransform>();

        goGunMiddleProjectileSpawnPoint = transform.Find("Chasis/GunMiddleProjectileSpawnPoint").gameObject;
        iNumAttackBurst = Random.Range(1, iNumMaxAttackBurst+1);
        fTimeNextAttack = Time.time + Random.Range(fTimeDeltaMinAttackBurst, fTimeDeltaMaxAttackBurst);

        goPlayer = GameObject.FindWithTag("Player");

        SetDestination();
    }

    // ------------------------------------------------------------------------------------------------

    void Update()
    {

        // ------------------------------------------------------------------------------------------------

        // Movement:

        if (    (moveMode != MoveMode.constant)
            &&  (navEnemy.remainingDistance <= navEnemy.stoppingDistance) )
        {
            SetDestination();
        }
        // Randomise the engine exhaust for a flickering effect:
        lineEngine1.SetPosition(1, new Vector3(0f, 0f, Random.Range(fPosZLower_lineEngine, fPosZUpper_lineEngine)));
        lineEngine2.SetPosition(1, new Vector3(0f, 0f, Random.Range(fPosZLower_lineEngine, fPosZUpper_lineEngine)));
        lineEngine3.SetPosition(1, new Vector3(0f, 0f, Random.Range(fPosZLower_lineEngine, fPosZUpper_lineEngine)));

        // ------------------------------------------------------------------------------------------------

        Vector3 v3PositionRelativeLook = goPlayer.transform.position - transform.position;
        Vector3 v3PositionRelativeLookNow = Vector3.RotateTowards(transform.forward, v3PositionRelativeLook, fAngSpeedMove * Time.deltaTime, 0f);
        transform.rotation = Quaternion.LookRotation(v3PositionRelativeLookNow);

        // ------------------------------------------------------------------------------------------------

        // Health:

        rtCanvasHealth.position = transform.position + new Vector3(0f, 0f, 10f);
        rtCanvasHealth.LookAt(transform.position + new Vector3(0f, 0f, 20f));

        // ------------------------------------------------------------------------------------------------

        // Damage:

        if (v3PositionRelativeLook.magnitude <= 200f)
        {
            if (Time.time >= fTimeNextAttack)
            {
                GameObject goProjectileClone = Instantiate(
                    goProjectile,
                    goGunMiddleProjectileSpawnPoint.transform.position,
                    goGunMiddleProjectileSpawnPoint.transform.rotation
                );
                audioManager.sfxclpvolListProjectileEnemy[Random.Range(0, audioManager.sfxclpvolListProjectileEnemy.Count)].PlayOneShot();
                iNumAttack += 1;
                if (iNumAttack < iNumAttackBurst)
                {
                    fTimeNextAttack = Time.time + fTimeDeltaAttack;
                }
                else
                {
                    iNumAttack = 0;
                    iNumAttackBurst = Random.Range(1, iNumMaxAttackBurst+1);
                    fTimeNextAttack = Time.time + Random.Range(fTimeDeltaMinAttackBurst, fTimeDeltaMaxAttackBurst);
                }
            }
        }

        // ------------------------------------------------------------------------------------------------

    }

    // ------------------------------------------------------------------------------------------------

    private void SetDestination()
    {
        if (moveMode == MoveMode.constant)
        {
            navEnemy.SetDestination(v3PositionConstant);
            return;
        }
        if (moveMode == MoveMode.constanthover)
        {
            v2PositionRandom = Random.insideUnitCircle.normalized;
            v3PositionRandom =
                v3PositionConstant
                + Random.Range(0f, 11f) * (new Vector3(v2PositionRandom.x, 0f, v2PositionRandom.y));
            navEnemy.SetDestination(v3PositionRandom);
            return;
        }
        if (moveMode == MoveMode.oscillatehorz)
        {
            if (bDirectionLowerLeft)
            {
                navEnemy.SetDestination(new Vector3(gameManager.v3MoveLimitLowerLeft.x, 0f, v3PositionConstant.z));
            }
            else
            {
                navEnemy.SetDestination(new Vector3(gameManager.v3MoveLimitUpperRight.x, 0f, v3PositionConstant.z));
            }
            bDirectionLowerLeft = !bDirectionLowerLeft;
            return;
        }
        if (moveMode == MoveMode.oscillatevert)
        {
            if (bDirectionLowerLeft)
            {
                navEnemy.SetDestination(new Vector3(v3PositionConstant.x, 0f, gameManager.v3MoveLimitLowerLeft.z));
            }
            else
            {
                navEnemy.SetDestination(new Vector3(v3PositionConstant.x, 0f, gameManager.v3MoveLimitUpperRight.z));
            }
            bDirectionLowerLeft = !bDirectionLowerLeft;
            return;
        }
        if (moveMode == MoveMode.random)
        {
            v3PositionRandom = new Vector3(gameManager.v3MoveLimitLowerLeft.x-10f, 0f, gameManager.v3MoveLimitLowerLeft.z-10f);

            while ( (v3PositionRandom.x < gameManager.v3MoveLimitLowerLeft.x)
                ||  (v3PositionRandom.x > gameManager.v3MoveLimitUpperRight.x)
                ||  (v3PositionRandom.z < gameManager.v3MoveLimitLowerLeft.z)
                ||  (v3PositionRandom.z > gameManager.v3MoveLimitUpperRight.z) )
            {
                v2PositionRandom = Random.insideUnitCircle.normalized;
                v3PositionRandom =
                    goPlayer.transform.position
                    + Random.Range(20f, 80f) * (new Vector3(v2PositionRandom.x, 0f, v2PositionRandom.y));
            }

            navEnemy.SetDestination(v3PositionRandom);
            return;
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

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.CompareTag("ProjectilePlayer"))
        {
            ProjectileController projectileController = collider.gameObject.GetComponent<ProjectileController>();
            if (!projectileController.bTriggeredDestroy)
            {
                projectileController.bTriggeredDestroy = true;
                healthEnemy.Change(-projectileController.iDamage);
                spawnManager.SpawnSparksVehicle(
                    collider.gameObject.transform.position,
                    Quaternion.LookRotation(-collider.gameObject.transform.forward),
                    transform
                );
                Destroy(collider.gameObject);
                if (    (healthEnemy.iHealth == 0)
                    &&  (!bTriggeredDestroy) )
                {
                    bTriggeredDestroy = true;
                    gameManager.ChangeScore(iScoreDelta);
                    spawnManager.iNumEnemy -= 1;
                    spawnManager.SpawnExplosionEnemy(transform.position);
                    if (Random.Range(0, 10) >= 3)
                    {
                        if (Random.Range(0, 10) >= 5)
                        {
                            spawnManager.SpawnPowerUpHealth(transform.position);
                        }
                        else
                        {
                            spawnManager.SpawnPowerUpCharge(transform.position);
                        }
                    }
                    Destroy(gameObject);
                }
                StartCoroutine(FlashDamaged());
            }
            return;
        }
    }

    // ------------------------------------------------------------------------------------------------

    // void OnDrawGizmos()
    // {
    //     Gizmos.color = Color.yellow;
    //     Gizmos.DrawSphere(v3PositionRandom, 2);
    // }

    // ------------------------------------------------------------------------------------------------

}
