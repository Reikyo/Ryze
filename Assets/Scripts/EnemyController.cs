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
    public float fAngSpeedMove = 5f;
    private NavMeshAgent navEnemy;

    public enum MoveMode {constant, constanthover, oscillatehorz, oscillatevert, pattern, random};
    public MoveMode moveMode;

    public Vector3 v3PositionConstant = new Vector3(0f, 0f, 0f);
    private Vector2 v2PositionRandom;
    private Vector3 v3PositionRandom;
    private bool bDirectionLowerLeft = true;
    private List<(Vector3, float)> v3fListPositionPattern = new List<(Vector3, float)>(){
        (new Vector3(-50f, 0f,  0f), 2f),
        (new Vector3(+50f, 0f,  0f), 2f),
        (new Vector3(+50f, 0f,+50f), 2f),
        (new Vector3(-50f, 0f,+50f), 2f)
    };
    private int iIdx_v3fListPositionPattern = 0;
    private bool bDestinationSet = false;
    private bool bDestinationArrived = false;
    private float fTimeDestinationArrived;

    public enum LookMode {constant, pattern, player};
    public LookMode lookMode;

    // Looking down from the positive y-axis, angles are clockwise from the positive z-axis
    public float fDegAngYRotationConstant = 0f;
    private float fRadAngYRotationConstant = 0f;
    private float fDegAngYRotationTarget = 0f;
    private float fRadAngYRotationTarget = 0f;
    private float fStoppingAngle = 0.1f;
    private Vector3 v3PositionRelativeLook;
    private Vector3 v3PositionRelativeLookNow;
    private List<(float, float)> ffListRotationPattern = new List<(float, float)>(){
        (0f, 2f),
        (90f, 2f),
        (180f, 2f),
        (270f, 2f)
    };
    private int iIdx_ffListRotationPattern = 0;
    private bool bOrientationSet = false;
    private bool bOrientationArrived = false;
    private float fTimeOrientationArrived;

    // Appearance:
    private Material matEnemy;
    private List<LineRenderer> lineListEngine = new List<LineRenderer>();
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
    private GameObject goLaser;
    private RaycastHit rayLaser;
    private LineRenderer lineLaser;
    private float fPosZBase_lineLaser;
    private GameObject goGunMiddleProjectileSpawnPoint;

    public enum AttackMode {projectile, laser};
    public AttackMode attackMode;

    private int iNumAttack = 0;
    private int iNumAttackBurst;
    public int iNumMaxAttackBurst = 5;
    private float fTimeNextAttack;
    public float fTimeDeltaAttack = 0.5f;
    public float fTimeDeltaMinAttackBurst = 2f;
    public float fTimeDeltaMaxAttackBurst = 5f;

    private int iDamageLaser = 1;
    private float fDistToPositionRelativeLookLaserEnable = 100f;
    private bool bRayLaserHitLastFrame = false;
    private bool bRayLaserHitThisFrame = false;
    private bool bRayLaserHitPlayerLastFrame = false;
    private bool bRayLaserHitPlayerThisFrame = false;
    private bool bLaserDamagePlayerLastFrame = false;
    private bool bLaserDamagePlayerThisFrame = false;

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
        foreach (Transform trn in transform.Find("Engine"))
        {
            lineListEngine.Add(trn.Find("Line : Engine").gameObject.GetComponent<LineRenderer>());
        }
        fPosZLower_lineEngine = fPosZBase_lineEngine - fPosZDelta_lineEngine;
        fPosZUpper_lineEngine = fPosZBase_lineEngine + fPosZDelta_lineEngine;

        healthEnemy = gameObject.AddComponent<Health>();
        healthEnemy.sliHealth = transform.Find("Canvas : Health/Slider : Health").GetComponent<Slider>();
        healthEnemy.Change(healthEnemy.iHealthMax);
        rtCanvasHealth = transform.Find("Canvas : Health").GetComponent<RectTransform>();

        if (transform.Find("Weapons/Line : Laser"))
        {
            goLaser = transform.Find("Weapons/Line : Laser").gameObject;
            lineLaser = goLaser.GetComponent<LineRenderer>();
            fPosZBase_lineLaser = lineLaser.GetPosition(1).z;
        }
        goGunMiddleProjectileSpawnPoint = transform.Find("Weapons/GunMiddleProjectileSpawnPoint").gameObject;
        iNumAttackBurst = Random.Range(1, iNumMaxAttackBurst+1);
        fTimeNextAttack = Time.time + Random.Range(fTimeDeltaMinAttackBurst, fTimeDeltaMaxAttackBurst);

        goPlayer = GameObject.FindWithTag("Player");

        SetDestination();
        SetOrientation();
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

        float fRemainingAngle = Mathf.Abs(transform.rotation.eulerAngles.y - fDegAngYRotationTarget);
        if (    (   (lookMode != LookMode.constant)
                &&  (fRemainingAngle <= fStoppingAngle) )
            ||  (lookMode == LookMode.player) )
        {
            SetOrientation();
        }

        // Using v3PositionRelativeLook in Quaternion.LookRotation() gives instant rotation towards the target
        // Using v3PositionRelativeLookNow in Quaternion.LookRotation() gives delayed rotation towards the target
        v3PositionRelativeLookNow = Vector3.RotateTowards(transform.forward, v3PositionRelativeLook, fAngSpeedMove * Time.deltaTime, 0f);
        transform.rotation = Quaternion.LookRotation(v3PositionRelativeLookNow);

        // ------------------------------------------------------------------------------------------------

        // Appearance:

        // Randomise the engine exhaust for a flickering effect:
        foreach (LineRenderer line in lineListEngine)
        {
            line.SetPosition(1, new Vector3(0f, 0f, Random.Range(fPosZLower_lineEngine, fPosZUpper_lineEngine)));
        }

        // ------------------------------------------------------------------------------------------------

        // Health:

        rtCanvasHealth.position = transform.position + new Vector3(0f, 0f, 10f);
        rtCanvasHealth.LookAt(transform.position + new Vector3(0f, 0f, 20f));

        // ------------------------------------------------------------------------------------------------

        // Damage:

        if (    (attackMode == AttackMode.projectile)
            &&  (goProjectile) )
        {
            SetAttackProjectile(v3PositionRelativeLook);
        }
        else if (   (attackMode == AttackMode.laser)
                &&  (goLaser) )
        {
            SetAttackLaser(v3PositionRelativeLook);
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
        if (moveMode == MoveMode.pattern)
        {
            if (!bDestinationSet)
            {
                navEnemy.SetDestination(v3fListPositionPattern[iIdx_v3fListPositionPattern].Item1);
                bDestinationSet = true;
            }
            else if (!bDestinationArrived)
            {
                fTimeDestinationArrived = Time.time;
                bDestinationArrived = true;
            }
            else if (Time.time > fTimeDestinationArrived + v3fListPositionPattern[iIdx_v3fListPositionPattern].Item2)
            {
                if (iIdx_v3fListPositionPattern < v3fListPositionPattern.Count-1)
                {
                    iIdx_v3fListPositionPattern++;
                }
                else
                {
                    iIdx_v3fListPositionPattern = 0;
                }
                bDestinationSet = false;
                bDestinationArrived = false;
            }
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

    private void SetOrientation()
    {
        if (lookMode == LookMode.constant)
        {
            fRadAngYRotationConstant = fDegAngYRotationConstant * Mathf.PI/180f;
            v3PositionRelativeLook = new Vector3(
                1000f * Mathf.Sin(fRadAngYRotationConstant),
                0f,
                1000f * Mathf.Cos(fRadAngYRotationConstant)
            );
            fDistToPositionRelativeLookLaserEnable = 1500f;
            return;
        }
        if (lookMode == LookMode.pattern)
        {
            if (!bOrientationSet)
            {
                fDegAngYRotationTarget = ffListRotationPattern[iIdx_ffListRotationPattern].Item1;
                fRadAngYRotationTarget = fDegAngYRotationTarget * Mathf.PI/180f;
                v3PositionRelativeLook = new Vector3(
                    1000f * Mathf.Sin(fRadAngYRotationTarget),
                    0f,
                    1000f * Mathf.Cos(fRadAngYRotationTarget)
                );
                bOrientationSet = true;
            }
            else if (!bOrientationArrived)
            {
                fTimeOrientationArrived = Time.time;
                bOrientationArrived = true;
            }
            else if (Time.time > fTimeOrientationArrived + ffListRotationPattern[iIdx_ffListRotationPattern].Item2)
            {
                if (iIdx_ffListRotationPattern < ffListRotationPattern.Count-1)
                {
                    iIdx_ffListRotationPattern++;
                }
                else
                {
                    iIdx_ffListRotationPattern = 0;
                }
                bOrientationSet = false;
                bOrientationArrived = false;
            }
            fDistToPositionRelativeLookLaserEnable = 1500f;
            return;
        }
        if (lookMode == LookMode.player)
        {
            v3PositionRelativeLook = goPlayer.transform.position - transform.position;
            fDistToPositionRelativeLookLaserEnable = 100f;
            return;
        }
    }

    // ------------------------------------------------------------------------------------------------

    private void SetAttackProjectile(Vector3 v3PositionRelativeLook)
    {
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
    }

    // ------------------------------------------------------------------------------------------------

    private void SetAttackLaser(Vector3 v3PositionRelativeLook)
    {
        bRayLaserHitThisFrame = Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out rayLaser);

        if (bRayLaserHitThisFrame)
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * rayLaser.distance, Color.yellow);
            lineLaser.SetPosition(1, new Vector3(0f, 0f, rayLaser.distance + 1f)); // Add 1 here to go further in than the collider edge and so get closer to the mesh
            bRayLaserHitPlayerThisFrame = rayLaser.collider.attachedRigidbody.gameObject.CompareTag("Player");
        }
        else
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * 1000f, Color.white);
            if (bRayLaserHitLastFrame)
            {
                lineLaser.SetPosition(1, new Vector3(0f, 0f, fPosZBase_lineLaser));
            }
            if (bRayLaserHitPlayerLastFrame)
            {
                bRayLaserHitPlayerThisFrame = false;
            }
        }

        if (v3PositionRelativeLook.magnitude <= fDistToPositionRelativeLookLaserEnable)
        {
            if (!lineLaser.enabled)
            {
                lineLaser.enabled = true;
                InvokeRepeating("PlaySfxLaser", 0f, 0.1f);
            }
        }
        else
        {
            if (lineLaser.enabled)
            {
                lineLaser.enabled = false;
                CancelInvoke("PlaySfxLaser");
            }
        }

        if (    (bRayLaserHitPlayerThisFrame)
            &&  (lineLaser.enabled) )
        {
            bLaserDamagePlayerThisFrame = true;
        }
        else
        {
            bLaserDamagePlayerThisFrame = false;
        }

        if (    (!bLaserDamagePlayerLastFrame)
            &&  (bLaserDamagePlayerThisFrame) )
        {
            goPlayer.GetComponent<PlayerController>().SetHealthDeltaPerSec(-iDamageLaser);
        }
        else if (   (bLaserDamagePlayerLastFrame)
                &&  (!bLaserDamagePlayerThisFrame) )
        {
            goPlayer.GetComponent<PlayerController>().SetHealthDeltaPerSec(+iDamageLaser);
        }

        bRayLaserHitLastFrame = bRayLaserHitThisFrame;
        bRayLaserHitPlayerLastFrame = bRayLaserHitPlayerThisFrame;
        bLaserDamagePlayerLastFrame = bLaserDamagePlayerThisFrame;
    }

    // ------------------------------------------------------------------------------------------------

    IEnumerator FlashDamaged()
    {
        matEnemy.EnableKeyword("_EMISSION");
        yield return new WaitForSeconds(fTimeFlashDamaged);
        matEnemy.DisableKeyword("_EMISSION");
    }

    // ------------------------------------------------------------------------------------------------

    private void PlaySfxLaser()
    {
        audioManager.sfxclpvolLaserEnemy.PlayOneShot();
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
                    if (bLaserDamagePlayerThisFrame)
                    {
                        goPlayer.GetComponent<PlayerController>().SetHealthDeltaPerSec(+iDamageLaser);
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
