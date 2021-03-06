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
    private GameObject goPlayer;

    private float fDegreesToRadians = Mathf.PI / 180f;

    // ------------------------------------------------------------------------------------------------
    // - Movement -

    private NavMeshAgent navEnemy;

    public float fMetresPerSecMoveMaxDefault = 100f;
    public float fDegreesPerSecMoveMaxDefault = 360f;
    private float fRadiansPerSecMoveMaxDefault;
    private float fDegreesPerSecMoveMax;
    private float fRadiansPerSecMoveMax;
    private float fDegreesPerSecMove;

    private Vector3 v3TransformForwardLastFrame = new Vector3();
    private Vector3 v3PositionDeltaToPlayer = new Vector3();
    private Vector3 v3PositionDeltaToRotationTarget;
    private Vector3 v3PositionDeltaToRotationTargetNow;

    private float fMetresDeltaToPlayer;
    private float fDegreesDeltaRotationYToPlayer;
    private float fDegreesDeltaRotationYToRotationTarget;
    private float fDegreesDeltaRotationYStopMax = 1f;
    private float fTimeDeltaToPositionTarget;
    private float fTimeDeltaToRotationTarget;
    private float fTimeDeltaStopMax = 0.5f;

    public enum MoveMode {constant, constanthover, oscillatehorz, oscillatevert, pattern, player};
    public MoveMode moveMode;
    public enum LookMode {constant, pattern, player};
    public LookMode lookMode;
    public bool bSyncMoveLookPatterns = true;
    public bool bSyncMoveLookAttackPatterns = true;

    public Vector3 v3PositionConstant = new Vector3(0f, 0f, 0f);
    private Vector2 v2PositionRandom;
    private Vector3 v3PositionRandom;
    private bool bDirectionLowerLeft = true;
    // These tuples are ([fMetresPosX,fMetresPosY,fMetresPosZ,fMetresPerSec], fTimeDeltaWait)
    // Note that the position arrays are optional. If one is empty, then the previous non-empty one
    // remains in effect. As such, at least the first position array must be non-empty for a default.
    // Note that the speed is optional, so the position arrays can be 3 or 4 elements long.
    public List<(float[], float)> ListPositionPattern;
    private int iIdx_ListPositionPattern = 0;

    // Looking down from the positive y-axis, angles are clockwise from the positive z-axis
    public float fDegreesRotationYConstant = 0f;
    private float fRadiansRotationYConstant = 0f;
    private float fDegreesRotationYTarget = 0f;
    private float fRadiansRotationYTarget = 0f;
    // These tuples are ([fDegreesRotationY,fDegreesPerSec], fTimeDeltaWait)
    // Note that the rotation arrays are optional. If one is empty, then the previous non-empty one
    // remains in effect. As such, at least the first rotation array must be non-empty for a default.
    // Note that the speed is optional, so the rotation arrays can be 1 or 2 elements long.
    public List<(float[], float)> ListRotationPattern;
    private int iIdx_ListRotationPattern = 0;

    // private bool bTEST = true;
    // private int iTEST = 0;
    // // private List<(float[], float[], float)> TEST = new List<(float[], float[], float)>(){
    // //     (new float[]{-50f, 0f,  0f}, new float[]{180f}, 2f),
    // //     (new float[]{+50f, 0f,  0f}, new float[]{ 90f}, 2f),
    // //     (new float[]{+50f, 0f,+50f}, new float[]{  0f}, 2f),
    // //     (new float[]{-50f, 0f,+50f, 10f}, new float[]{270f}, 2f)
    // // };
    // private List<(float[], float[], float)> TEST = new List<(float[], float[], float)>(){
    //     (new float[]{-80f, 0f,+80f}, new float[]{180f}, 2f),
    //     (new float[]{}, new float[]{90f, 18f}, 2f),
    //     (new float[]{-40f, 0f,  0f}, new float[]{}, 2f),
    //     (new float[]{}, new float[]{-45f}, 2f),
    //     (new float[]{-40f, 0f,+80f}, new float[]{}, 2f),
    // };

    private bool bPositionTargetArrived = false;
    private bool bRotationTargetArrived = false;
    private bool bWaitAtPositionTarget = false;
    private bool bWaitAtRotationTarget = false;
    private bool bWaitForAttackPattern = false;
    private float fTimeDeltaWaitAtPositionTarget;
    private float fTimeDeltaWaitAtRotationTarget;
    private float fTimeDeltaWaitForAttackPattern;

    // ------------------------------------------------------------------------------------------------
    // - Appearance -

    private Material matEnemy;
    private List<LineRenderer> lineListEngine = new List<LineRenderer>();
    private float fPositionZBase_lineEngine = -2f;
    private float fPositionZDelta_lineEngine = 0.25f;
    private float fPositionZLower_lineEngine;
    private float fPositionZUpper_lineEngine;

    // ------------------------------------------------------------------------------------------------
    // - Health -

    private Health healthEnemy;
    private RectTransform rtCanvasHealth;
    private float fTimeDeltaWaitFlashDamaged = 0.1f;
    private bool bTriggeredDestroy = false;

    // ------------------------------------------------------------------------------------------------
    // - Damage -

    private RaycastHit rayHit;
    public GameObject goProjectile;
    private GameObject goLaser;
    private LineRenderer lineLaser;
    private float fPositionZBase_lineLaser;
    private GameObject goGunMiddleProjectileSpawnPoint;

    public enum AttackMode1 {projectile, laser};
    public AttackMode1 attackMode1;
    public enum AttackMode2 {constant, burst, pattern};
    public AttackMode2 attackMode2;

    public bool bAttackOnlyIfPlayerInRange = false;
    public bool bAttackOnlyIfPlayerInSight = false;
    public float fMetresDeltaToPlayerAttackProjectile = 200f;
    public float fMetresDeltaToPlayerAttackLaser = 100f;
    public float fDegreesDeltaRotationYToPlayerAttackProjectile = 5f;
    public float fDegreesDeltaRotationYToPlayerAttackLaser = 5f;

    public int iNumMinAttackProjectileBurst = 1;
    public int iNumMaxAttackProjectileBurst = 5;
    public int iNumMinAttackLaserBurst = 1;
    public int iNumMaxAttackLaserBurst = 5;
    private int iNumMaxThisAttackBurst;
    private int iNumThisAttackBurst = 0;
    private float fTimeThisAttackStop; // Only relevant for laser
    private float fTimeNextAttack;
    public float fTimeDeltaAttackProjectileConstant = 0.5f;
    public float fTimeDeltaMinAttackProjectileBurstInterBurst = 2f;
    public float fTimeDeltaMaxAttackProjectileBurstInterBurst = 5f;
    public float fTimeDeltaMinAttackProjectileBurstIntraBurst = 0.1f;
    public float fTimeDeltaMaxAttackProjectileBurstIntraBurst = 0.3f;
    public float fTimeDeltaMinAttackLaserBurstInterBurst = 2f;
    public float fTimeDeltaMaxAttackLaserBurstInterBurst = 5f;
    public float fTimeDeltaMinAttackLaserBurstIntraBurstOn = 0.1f;
    public float fTimeDeltaMaxAttackLaserBurstIntraBurstOn = 1f;
    public float fTimeDeltaMinAttackLaserBurstIntraBurstOff = 0.1f;
    public float fTimeDeltaMaxAttackLaserBurstIntraBurstOff = 0.3f;

    // These tuples are ([fTimeDeltaAttackLaserOn], [fTimeDeltaAttackLaserOff])
    public List<(float[], float[])> ListAttackPattern;
    private int iIdx1_ListAttackPattern = -1; // This is initialised at -1 as the first reset will add 1 and bring it to 0
    private int iIdx2_ListAttackPattern = 0;
    private bool bStartedAttack = false;
    private bool bAttackPatternCompleted = false;

    private int iDamageLaser = 1;
    private bool bRayHitLastFrame = false;
    private bool bRayHitThisFrame = false;
    private bool bRayHitPlayerLastFrame = false;
    private bool bRayHitPlayerThisFrame = false;
    private bool bLaserDamagePlayerLastFrame = false;
    private bool bLaserDamagePlayerThisFrame = false;

    // ------------------------------------------------------------------------------------------------
    // - Score -

    public int iScoreDelta = 10;

    // ------------------------------------------------------------------------------------------------

    void Start()
    {
        // ------------------------------------------------------------------------------------------------

        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
        spawnManager = GameObject.Find("Spawn Manager").GetComponent<SpawnManager>();
        audioManager = GameObject.Find("Audio Manager").GetComponent<AudioManager>();
        goPlayer = GameObject.FindWithTag("Player");

        // ------------------------------------------------------------------------------------------------

        // ListPositionPattern = new List<(float[], float)>(){
        //     (new float[]{-50f, 0f,  0f}, 2f),
        //     (new float[]{+50f, 0f,  0f}, 2f),
        //     (new float[]{+50f, 0f,+50f}, 2f),
        //     (new float[]{-50f, 0f,+50f, 10f}, 2f)
        // };
        //
        // ListRotationPattern = new List<(float[], float)>(){
        //     (new float[]{180f}, 2f),
        //     (new float[]{90f}, 2f),
        //     (new float[]{0f}, 2f),
        //     (new float[]{270f}, 2f)
        // };
        //
        // ListAttackPattern = new List<(float[], float[])>(){
        //     (new float[]{1f, 2f, 3f, 4f}, new float[]{2f, 2f, 2f, 2f})
        // };

        // ListPositionPattern = new List<(float[], float)>(){
        //     (new float[]{-80f, 0f,+80f}, 1f),
        //     (new float[]{}, 1f),
        //     (new float[]{-80f, 0f,  0f, 20f}, 1f),
        //     (new float[]{}, 1f),
        //     (new float[]{-80f, 0f,+80f, 20f}, 1f),
        //     (new float[]{}, 1f),
        // };
        //
        // ListRotationPattern = new List<(float[], float)>(){
        //     (new float[]{180f}, 1f),
        //     (new float[]{110f, 20f}, 1f),
        //     (new float[]{}, 1f),
        //     (new float[]{70f, 20f}, 1f),
        //     (new float[]{}, 1f),
        //     (new float[]{180f, 20f}, 1f),
        // };
        //
        // ListAttackPattern = new List<(float[], float[])>(){
        //     (new float[]{}, new float[]{}),
        //     (new float[]{1f}, new float[]{0f}),
        //     (new float[]{1f}, new float[]{0f}),
        //     (new float[]{1f}, new float[]{0f}),
        //     (new float[]{1f}, new float[]{0f}),
        //     (new float[]{6f}, new float[]{1f}),
        // };

        // ------------------------------------------------------------------------------------------------
        // - Movement -

        navEnemy = GetComponent<NavMeshAgent>();
        navEnemy.speed = fMetresPerSecMoveMaxDefault;
        navEnemy.updateRotation = false; // This is true by default, but that causes issues with the explicit rotation handling herein, so toggle off
        fRadiansPerSecMoveMaxDefault = fDegreesPerSecMoveMaxDefault * fDegreesToRadians;
        fRadiansPerSecMoveMax = fRadiansPerSecMoveMaxDefault;
        v3TransformForwardLastFrame = transform.forward;

        // ------------------------------------------------------------------------------------------------
        // - Appearance -

        matEnemy = transform.Find("Chasis").gameObject.GetComponent<Renderer>().material;
        foreach (Transform trn in transform.Find("Engine"))
        {
            lineListEngine.Add(trn.Find("Line : Engine").gameObject.GetComponent<LineRenderer>());
        }
        fPositionZLower_lineEngine = fPositionZBase_lineEngine - fPositionZDelta_lineEngine;
        fPositionZUpper_lineEngine = fPositionZBase_lineEngine + fPositionZDelta_lineEngine;

        // ------------------------------------------------------------------------------------------------
        // - Health -

        healthEnemy = gameObject.AddComponent<Health>();
        healthEnemy.sliHealth = transform.Find("Canvas : Health/Slider : Health").GetComponent<Slider>();
        healthEnemy.Change(healthEnemy.iHealthMax);
        rtCanvasHealth = transform.Find("Canvas : Health").GetComponent<RectTransform>();

        // ------------------------------------------------------------------------------------------------
        // - Damage -

        if (transform.Find("Weapons/Line : Laser"))
        {
            goLaser = transform.Find("Weapons/Line : Laser").gameObject;
            lineLaser = goLaser.GetComponent<LineRenderer>();
            fPositionZBase_lineLaser = lineLaser.GetPosition(1).z;
        }
        goGunMiddleProjectileSpawnPoint = transform.Find("Weapons/GunMiddleProjectileSpawnPoint").gameObject;
        if (attackMode2 == AttackMode2.burst)
        {
            ResetAttackBurst();
        }
        else if (attackMode2 == AttackMode2.pattern)
        {
            ResetAttackPattern();
        }

        // ------------------------------------------------------------------------------------------------

        SetPositionTarget();
        SetRotationTarget();

        // ------------------------------------------------------------------------------------------------
    }

    // ------------------------------------------------------------------------------------------------

    void Update()
    {
        // ------------------------------------------------------------------------------------------------

        SetInfoThisFrame();

        // ------------------------------------------------------------------------------------------------
        // - Movement -

        // Here we check both the remaining distance/angle to the target position/rotation, and the
        // remaining time to the target position/rotation at the current speed. This is important for
        // pattern motion that incorporates wait times. For example, if the enemy is moving very slowly,
        // then it could be within the stopping distance of the target and so trigger the next call to
        // the function that sets the target position/rotation without consideration of the stopping time.
        // This would allow for the possibility of the actual time to the target being longer than the
        // desired wait time at the target, and so no wait time would actually be had when the target is
        // arrived at. We don't simply use the stopping time to the target on its own either, as if the
        // enemy is moving very quickly, then it could be within stopping time of the target and so trigger
        // the next call to the function that sets the target position/rotation without consideration of
        // the stopping distance. The next target could then be set before the current one is arrived at,
        // and the course would be adjusted too soon.

        SetMovement();

        // ------------------------------------------------------------------------------------------------
        // - Appearance -

        // Randomise the engine exhaust for a flickering effect:
        foreach (LineRenderer line in lineListEngine)
        {
            line.SetPosition(1, new Vector3(0f, 0f, Random.Range(fPositionZLower_lineEngine, fPositionZUpper_lineEngine)));
        }

        // ------------------------------------------------------------------------------------------------
        // - Health -

        rtCanvasHealth.position = transform.position + new Vector3(0f, 0f, 10f);
        rtCanvasHealth.LookAt(transform.position + new Vector3(0f, 0f, 20f));

        // ------------------------------------------------------------------------------------------------
        // - Damage -

        SetAttack();
        if (goLaser)
        {
            SetHealthPlayer();
        }

        // ------------------------------------------------------------------------------------------------

        SetInfoLastFrame();

        // ------------------------------------------------------------------------------------------------
    }

    // ------------------------------------------------------------------------------------------------

    private void SetInfoThisFrame()
    {
        if (goLaser)
        {
            bRayHitThisFrame = Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out rayHit);
            if (bRayHitThisFrame)
            {
                Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * rayHit.distance, Color.yellow);
                lineLaser.SetPosition(1, new Vector3(0f, 0f, rayHit.distance + 1f)); // Add 1 here to go further in than the collider edge and so get closer to the mesh
                bRayHitPlayerThisFrame = rayHit.collider.attachedRigidbody.gameObject.CompareTag("Player");
            }
            else
            {
                Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * 1000f, Color.white);
                if (bRayHitLastFrame)
                {
                    lineLaser.SetPosition(1, new Vector3(0f, 0f, fPositionZBase_lineLaser));
                }
                if (bRayHitPlayerLastFrame)
                {
                    bRayHitPlayerThisFrame = false;
                }
            }
        }

        v3PositionDeltaToPlayer = goPlayer.transform.position - transform.position;
        fMetresDeltaToPlayer = v3PositionDeltaToPlayer.magnitude;
        fDegreesDeltaRotationYToPlayer = Vector3.Angle(transform.forward, v3PositionDeltaToPlayer);
        if (lookMode == LookMode.player)
        {
            fDegreesDeltaRotationYToRotationTarget = fDegreesDeltaRotationYToPlayer;
        }
        else
        {
            fDegreesDeltaRotationYToRotationTarget = Vector3.Angle(transform.forward, v3PositionDeltaToRotationTarget);
        }
    }

    // ------------------------------------------------------------------------------------------------

    private void SetInfoLastFrame()
    {
        if (goLaser)
        {
            bRayHitLastFrame = bRayHitThisFrame;
            bRayHitPlayerLastFrame = bRayHitPlayerThisFrame;
            bLaserDamagePlayerLastFrame = bLaserDamagePlayerThisFrame;
        }
        v3TransformForwardLastFrame = transform.forward;
    }

    // ------------------------------------------------------------------------------------------------

    private void SetMovement()
    {
        if (    (moveMode != MoveMode.constant)
            &&  (!bPositionTargetArrived) )
        {
            CheckPositionTargetArrived();
            if (    (bPositionTargetArrived)
                &&  (bWaitAtPositionTarget) )
            {
                StartCoroutine(WaitAtPositionTarget());
            }
        }

        if (    (lookMode == LookMode.pattern)
            &&  (!bRotationTargetArrived) )
        {
            CheckRotationTargetArrived();
            if (    (bRotationTargetArrived)
                &&  (bWaitAtRotationTarget) )
            {
                StartCoroutine(WaitAtRotationTarget());
            }
        }

        // if (bTEST)
        // {
        //     if (    (bPositionTargetArrived)
        //         &&  (!bWaitAtPositionTarget)
        //         &&  (bRotationTargetArrived)
        //         &&  (!bWaitAtRotationTarget) )
        //     {
        //         SetTEST();
        //     }
        // }
        if (    (moveMode == MoveMode.pattern)
            &&  (lookMode == LookMode.pattern)
            &&  (attackMode2 == AttackMode2.pattern)
            &&  (bSyncMoveLookAttackPatterns) )
        {
            if (    (bPositionTargetArrived)
                &&  (!bWaitAtPositionTarget)
                &&  (bRotationTargetArrived)
                &&  (!bWaitAtRotationTarget)
                &&  (bAttackPatternCompleted)
                &&  (!bWaitForAttackPattern) )
            {
                SetPositionTarget();
                SetRotationTarget();
                ResetAttackPattern();
            }
        }
        else if (   (moveMode == MoveMode.pattern)
                &&  (lookMode == LookMode.pattern)
                &&  (bSyncMoveLookPatterns) )
        {
            if (    (bPositionTargetArrived)
                &&  (!bWaitAtPositionTarget)
                &&  (bRotationTargetArrived)
                &&  (!bWaitAtRotationTarget) )
            {
                SetPositionTarget();
                SetRotationTarget();
            }
            if (    (attackMode2 == AttackMode2.pattern)
                &&  (bAttackPatternCompleted)
                &&  (!bWaitForAttackPattern) )
            {
                ResetAttackPattern();
            }
        }
        else
        {
            if (    (moveMode != MoveMode.constant)
                &&  (bPositionTargetArrived)
                &&  (!bWaitAtPositionTarget) )
            {
                SetPositionTarget();
            }
            if (    (lookMode == LookMode.pattern)
                &&  (bRotationTargetArrived)
                &&  (!bWaitAtRotationTarget) )
            {
                SetRotationTarget();
            }
            else if (lookMode == LookMode.player)
            {
                SetRotationTarget();
            }
            if (    (attackMode2 == AttackMode2.pattern)
                &&  (bAttackPatternCompleted)
                &&  (!bWaitForAttackPattern) )
            {
                ResetAttackPattern();
            }
        }

        // Using v3PositionDeltaToRotationTarget in Quaternion.LookRotation() gives instant rotation towards the target
        // Using v3PositionDeltaToRotationTargetNow in Quaternion.LookRotation() gives delayed rotation towards the target
        v3PositionDeltaToRotationTargetNow = Vector3.RotateTowards(transform.forward, v3PositionDeltaToRotationTarget, fRadiansPerSecMoveMax * Time.deltaTime, 0f);
        transform.rotation = Quaternion.LookRotation(v3PositionDeltaToRotationTargetNow);
    }

    // ------------------------------------------------------------------------------------------------

    private void CheckPositionTargetArrived()
    {
        if (navEnemy.remainingDistance <= navEnemy.stoppingDistance)
        {
            if (navEnemy.velocity.magnitude == 0f)
            {
                fTimeDeltaToPositionTarget = 0f;
            }
            else
            {
                fTimeDeltaToPositionTarget = navEnemy.remainingDistance / navEnemy.velocity.magnitude;
            }
            if (fTimeDeltaToPositionTarget <= fTimeDeltaStopMax)
            {
                bPositionTargetArrived = true;
            }
        }
    }

    // ------------------------------------------------------------------------------------------------

    private void CheckRotationTargetArrived()
    {
        if (fDegreesDeltaRotationYToRotationTarget <= fDegreesDeltaRotationYStopMax)
        {
            fDegreesPerSecMove = Vector3.Angle(transform.forward, v3TransformForwardLastFrame) / Time.deltaTime;
            if (fDegreesPerSecMove == 0f)
            {
                fTimeDeltaToRotationTarget = 0f;
            }
            else
            {
                fTimeDeltaToRotationTarget = fDegreesDeltaRotationYToRotationTarget / fDegreesPerSecMove;
            }
            if (fTimeDeltaToRotationTarget <= fTimeDeltaStopMax)
            {
                bRotationTargetArrived = true;
            }
        }
    }

    // ------------------------------------------------------------------------------------------------

    private void SetPositionTarget()
    {
        bPositionTargetArrived = false;
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
            if (ListPositionPattern[iIdx_ListPositionPattern].Item1.Length >= 3)
            {
                navEnemy.SetDestination(new Vector3(
                    ListPositionPattern[iIdx_ListPositionPattern].Item1[0],
                    ListPositionPattern[iIdx_ListPositionPattern].Item1[1],
                    ListPositionPattern[iIdx_ListPositionPattern].Item1[2]
                ));
                if (ListPositionPattern[iIdx_ListPositionPattern].Item1.Length == 4)
                {
                    navEnemy.speed = ListPositionPattern[iIdx_ListPositionPattern].Item1[3];
                }
                else
                {
                    navEnemy.speed = fMetresPerSecMoveMaxDefault;
                }
            }
            fTimeDeltaWaitAtPositionTarget = ListPositionPattern[iIdx_ListPositionPattern].Item2;
            bWaitAtPositionTarget = fTimeDeltaWaitAtPositionTarget > 0f;
            if (iIdx_ListPositionPattern < ListPositionPattern.Count-1)
            {
                iIdx_ListPositionPattern++;
            }
            else
            {
                iIdx_ListPositionPattern = 0;
            }
            return;
        }
        if (moveMode == MoveMode.player)
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

    private void SetRotationTarget()
    {
        bRotationTargetArrived = false;
        if (lookMode == LookMode.constant)
        {
            fRadiansRotationYConstant = fDegreesRotationYConstant * fDegreesToRadians;
            v3PositionDeltaToRotationTarget = new Vector3(
                1000f * Mathf.Sin(fRadiansRotationYConstant),
                0f,
                1000f * Mathf.Cos(fRadiansRotationYConstant)
            );
            return;
        }
        if (lookMode == LookMode.pattern)
        {
            if (ListRotationPattern[iIdx_ListRotationPattern].Item1.Length >= 1)
            {
                fDegreesRotationYTarget = ListRotationPattern[iIdx_ListRotationPattern].Item1[0];
                fRadiansRotationYTarget = fDegreesRotationYTarget * fDegreesToRadians;
                v3PositionDeltaToRotationTarget = new Vector3(
                    1000f * Mathf.Sin(fRadiansRotationYTarget),
                    0f,
                    1000f * Mathf.Cos(fRadiansRotationYTarget)
                );
                if (ListRotationPattern[iIdx_ListRotationPattern].Item1.Length == 2)
                {
                    fDegreesPerSecMoveMax = ListRotationPattern[iIdx_ListRotationPattern].Item1[1];
                    fRadiansPerSecMoveMax = fDegreesPerSecMoveMax * fDegreesToRadians;
                }
                else
                {
                    fRadiansPerSecMoveMax = fRadiansPerSecMoveMaxDefault;
                }
            }
            fTimeDeltaWaitAtRotationTarget = ListRotationPattern[iIdx_ListRotationPattern].Item2;
            bWaitAtRotationTarget = fTimeDeltaWaitAtRotationTarget > 0f;
            if (iIdx_ListRotationPattern < ListRotationPattern.Count-1)
            {
                iIdx_ListRotationPattern++;
            }
            else
            {
                iIdx_ListRotationPattern = 0;
            }
            return;
        }
        if (lookMode == LookMode.player)
        {
            v3PositionDeltaToRotationTarget = v3PositionDeltaToPlayer;
            return;
        }
    }

    // ------------------------------------------------------------------------------------------------

    // private void SetTEST()
    // {
    //     bPositionTargetArrived = false;
    //     bRotationTargetArrived = false;
    //
    //     if (TEST[iTEST].Item1.Length >= 3)
    //     {
    //         navEnemy.SetDestination(new Vector3(
    //             TEST[iTEST].Item1[0],
    //             TEST[iTEST].Item1[1],
    //             TEST[iTEST].Item1[2]
    //         ));
    //         if (TEST[iTEST].Item1.Length == 4)
    //         {
    //             navEnemy.speed = TEST[iTEST].Item1[3];
    //         }
    //         else
    //         {
    //             navEnemy.speed = fMetresPerSecMoveMaxDefault;
    //         }
    //     }
    //
    //     if (TEST[iTEST].Item2.Length >= 1)
    //     {
    //         fDegreesRotationYTarget = TEST[iTEST].Item2[0];
    //         fRadiansRotationYTarget = fDegreesRotationYTarget * fDegreesToRadians;
    //         v3PositionDeltaToRotationTarget = new Vector3(
    //             1000f * Mathf.Sin(fRadiansRotationYTarget),
    //             0f,
    //             1000f * Mathf.Cos(fRadiansRotationYTarget)
    //         );
    //         if (TEST[iTEST].Item2.Length == 2)
    //         {
    //             fDegreesPerSecMoveMax = TEST[iTEST].Item2[1];
    //             fRadiansPerSecMoveMax = fDegreesPerSecMoveMax * fDegreesToRadians;
    //         }
    //         else
    //         {
    //             fRadiansPerSecMoveMax = fRadiansPerSecMoveMaxDefault;
    //         }
    //     }
    //
    //     fTimeDeltaWaitAtPositionTarget = TEST[iTEST].Item3;
    //     fTimeDeltaWaitAtRotationTarget = fTimeDeltaWaitAtPositionTarget;
    //     bWaitAtPositionTarget = fTimeDeltaWaitAtPositionTarget > 0f;
    //     bWaitAtRotationTarget = bWaitAtPositionTarget;
    //
    //     if (iTEST < TEST.Count-1)
    //     {
    //         iTEST++;
    //     }
    //     else
    //     {
    //         iTEST = 0;
    //     }
    // }

    // ------------------------------------------------------------------------------------------------

    private void ResetAttackBurst()
    {
        iNumThisAttackBurst = 0;
        if (attackMode1 == AttackMode1.projectile)
        {
            iNumMaxThisAttackBurst = Random.Range(iNumMinAttackProjectileBurst, iNumMaxAttackProjectileBurst+1);
            fTimeNextAttack = Time.time + Random.Range(fTimeDeltaMinAttackProjectileBurstInterBurst, fTimeDeltaMaxAttackProjectileBurstInterBurst);
        }
        else if (attackMode1 == AttackMode1.laser)
        {
            iNumMaxThisAttackBurst = Random.Range(iNumMinAttackLaserBurst, iNumMaxAttackLaserBurst+1);
            fTimeNextAttack = Time.time + Random.Range(fTimeDeltaMinAttackLaserBurstInterBurst, fTimeDeltaMaxAttackLaserBurstInterBurst);
        }
    }

    // ------------------------------------------------------------------------------------------------

    private void ResetAttackPattern()
    {
        fTimeNextAttack = 0f;
        if (iIdx1_ListAttackPattern < ListAttackPattern.Count-1)
        {
            iIdx1_ListAttackPattern++;
        }
        else
        {
            iIdx1_ListAttackPattern = 0;
        }
        bAttackPatternCompleted = (ListAttackPattern[iIdx1_ListAttackPattern].Item1.Length == 0);
    }

    // ------------------------------------------------------------------------------------------------

    private void SetAttack()
    {
        if (    (   (!bAttackOnlyIfPlayerInRange)
                ||  (fMetresDeltaToPlayer <= ((attackMode1 == AttackMode1.projectile) ? fMetresDeltaToPlayerAttackProjectile : fMetresDeltaToPlayerAttackLaser)) )
            &&  (   (!bAttackOnlyIfPlayerInSight)
                ||  (fDegreesDeltaRotationYToPlayer <= ((attackMode1 == AttackMode1.projectile) ? fDegreesDeltaRotationYToPlayerAttackProjectile : fDegreesDeltaRotationYToPlayerAttackLaser)) ) )
        {
            if (    (goProjectile)
                &&  (attackMode1 == AttackMode1.projectile) )
            {
                SetAttackProjectile();
            }
            else if (   (goLaser)
                    &&  (attackMode1 == AttackMode1.laser) )
            {
                SetAttackLaser();
            }
        }
        else
        {
            if (attackMode2 == AttackMode2.burst)
            {
                ResetAttackBurst();
            }
            if (    (goLaser)
                &&  (lineLaser.enabled) )
            {
                StopLaser();
            }
        }
    }

    // ------------------------------------------------------------------------------------------------

    private void SetAttackProjectile()
    {
        if (    (goLaser)
            &&  (lineLaser.enabled) )
        {
            StopLaser();
        }
        if (attackMode2 == AttackMode2.constant)
        {
            if (Time.time >= fTimeNextAttack)
            {
                GameObject goProjectileClone = Instantiate(
                    goProjectile,
                    goGunMiddleProjectileSpawnPoint.transform.position,
                    goGunMiddleProjectileSpawnPoint.transform.rotation
                );
                audioManager.sfxclpvolListProjectileEnemy[Random.Range(0, audioManager.sfxclpvolListProjectileEnemy.Count)].PlayOneShot();
                fTimeNextAttack = Time.time + fTimeDeltaAttackProjectileConstant;
            }
            return;
        }
        if (attackMode2 == AttackMode2.burst)
        {
            if (Time.time >= fTimeNextAttack)
            {
                GameObject goProjectileClone = Instantiate(
                    goProjectile,
                    goGunMiddleProjectileSpawnPoint.transform.position,
                    goGunMiddleProjectileSpawnPoint.transform.rotation
                );
                audioManager.sfxclpvolListProjectileEnemy[Random.Range(0, audioManager.sfxclpvolListProjectileEnemy.Count)].PlayOneShot();
                iNumThisAttackBurst++;
                if (iNumThisAttackBurst < iNumMaxThisAttackBurst)
                {
                    fTimeNextAttack = Time.time + Random.Range(fTimeDeltaMinAttackProjectileBurstIntraBurst, fTimeDeltaMaxAttackProjectileBurstIntraBurst);
                }
                else
                {
                    ResetAttackBurst();
                }
            }
            return;
        }
        if (attackMode2 == AttackMode2.pattern)
        {
            return;
        }
    }

    // ------------------------------------------------------------------------------------------------

    private void SetAttackLaser()
    {
        if (attackMode2 == AttackMode2.constant)
        {
            if (!lineLaser.enabled)
            {
                StartLaser();
            }
            return;
        }
        if (attackMode2 == AttackMode2.burst)
        {
            if (    (!lineLaser.enabled)
                &&  (Time.time >= fTimeNextAttack) )
            {
                StartLaser();
                fTimeThisAttackStop = Time.time + Random.Range(fTimeDeltaMinAttackLaserBurstIntraBurstOn, fTimeDeltaMaxAttackLaserBurstIntraBurstOn);
            }
            else if (   (lineLaser.enabled)
                    &&  (Time.time >= fTimeThisAttackStop) )
            {
                StopLaser();
                iNumThisAttackBurst++;
                if (iNumThisAttackBurst < iNumMaxThisAttackBurst)
                {
                    fTimeNextAttack = Time.time + Random.Range(fTimeDeltaMinAttackLaserBurstIntraBurstOff, fTimeDeltaMaxAttackLaserBurstIntraBurstOff);
                }
                else
                {
                    ResetAttackBurst();
                }
            }
            return;
        }
        if (attackMode2 == AttackMode2.pattern)
        {
            if (!bAttackPatternCompleted)
            {
                if (    (!bStartedAttack)
                    &&  (Time.time >= fTimeNextAttack) )
                {
                    if (!lineLaser.enabled)
                    {
                        StartLaser();
                    }
                    bStartedAttack = true;
                    fTimeThisAttackStop = Time.time + ListAttackPattern[iIdx1_ListAttackPattern].Item1[iIdx2_ListAttackPattern];
                }
                else if (   (bStartedAttack)
                        &&  (Time.time >= fTimeThisAttackStop) )
                {
                    bStartedAttack = false;
                    fTimeDeltaWaitForAttackPattern = ListAttackPattern[iIdx1_ListAttackPattern].Item2[iIdx2_ListAttackPattern];
                    fTimeNextAttack = Time.time + fTimeDeltaWaitForAttackPattern;
                    if (fTimeDeltaWaitForAttackPattern > 0)
                    {
                        StopLaser();
                    }
                    if (iIdx2_ListAttackPattern < ListAttackPattern[iIdx1_ListAttackPattern].Item2.Length-1)
                    {
                        iIdx2_ListAttackPattern++;
                    }
                    else
                    {
                        iIdx2_ListAttackPattern = 0;
                        bAttackPatternCompleted = true;
                        bWaitForAttackPattern = fTimeDeltaWaitForAttackPattern > 0f;
                        if (bWaitForAttackPattern)
                        {
                            StartCoroutine(WaitForAttackPattern());
                        }
                    }
                }
            }
            return;
        }
    }

    // ------------------------------------------------------------------------------------------------

    private void StartLaser()
    {
        lineLaser.enabled = true;
        InvokeRepeating("PlaySfxLaser", 0f, 0.1f);
    }

    // ------------------------------------------------------------------------------------------------

    private void StopLaser()
    {
        lineLaser.enabled = false;
        CancelInvoke("PlaySfxLaser");
    }

    // ------------------------------------------------------------------------------------------------

    private void PlaySfxLaser()
    {
        audioManager.sfxclpvolLaserEnemy.PlayOneShot();
    }

    // ------------------------------------------------------------------------------------------------

    private void SetHealthPlayer()
    {
        if (    (bRayHitPlayerThisFrame)
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
    }

    // ------------------------------------------------------------------------------------------------

    IEnumerator WaitAtPositionTarget()
    {
        yield return new WaitForSeconds(fTimeDeltaWaitAtPositionTarget + fTimeDeltaToPositionTarget);
        bWaitAtPositionTarget = false;
    }

    // ------------------------------------------------------------------------------------------------

    IEnumerator WaitAtRotationTarget()
    {
        yield return new WaitForSeconds(fTimeDeltaWaitAtRotationTarget + fTimeDeltaToRotationTarget);
        bWaitAtRotationTarget = false;
    }

    // ------------------------------------------------------------------------------------------------

    IEnumerator WaitForAttackPattern()
    {
        yield return new WaitForSeconds(fTimeDeltaWaitForAttackPattern);
        bWaitForAttackPattern = false;
    }

    // ------------------------------------------------------------------------------------------------

    IEnumerator FlashDamaged()
    {
        matEnemy.EnableKeyword("_EMISSION");
        yield return new WaitForSeconds(fTimeDeltaWaitFlashDamaged);
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
