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

    private bool bPositionTargetArrived = false;
    private bool bRotationTargetArrived = false;
    private bool bWaitAtPosition = false;
    private bool bWaitAtRotation = false;
    private float fTimeWaitAtPosition;
    private float fTimeWaitAtRotation;
    public bool bSyncPattern = true;

    private Vector3 v3TransformForwardLastFrame = new Vector3();
    public float fMetresPerSecMoveMaxDefault = 100f;
    public float fDegreesPerSecMoveMaxDefault = 360f;
    private float fRadiansPerSecMoveMaxDefault;
    private float fDegreesPerSecMoveMax;
    private float fRadiansPerSecMoveMax;
    private float fDegreesPerSecMove;

    private Vector3 v3PositionRelativePlayer = new Vector3();
    private float fDistToPlayer;
    private float fDegreesRotationYToPlayer;
    private float fDegreesRotationYToTarget;

    private float fTimeStop = 0.5f;
    private float fDegreesRotationYStop = 1f;

    public enum MoveMode {constant, constanthover, oscillatehorz, oscillatevert, pattern, player};
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
    }; // These tuples are (v3Position, fTimeWait)
    private int iIdx_v3fListPositionPattern = 0;

    public enum LookMode {constant, pattern, player};
    public LookMode lookMode;

    // Looking down from the positive y-axis, angles are clockwise from the positive z-axis
    public float fDegreesRotationYConstant = 0f;
    private float fRadiansRotationYConstant = 0f;
    private float fDegreesRotationYTarget = 0f;
    private float fRadiansRotationYTarget = 0f;
    private Vector3 v3PositionRelativeTarget;
    private Vector3 v3PositionRelativeTargetNow;
    private List<(float, float)> ffListRotationPattern = new List<(float, float)>(){
        (180f, 2f),
        (90f, 2f),
        (0f, 2f),
        (270f, 2f)
    }; // These tuples are (fDegreesRotationY, fTimeWait)
    private int iIdx_ffListRotationPattern = 0;

    private bool bTEST = true;
    private int iTEST = 0;
    // private List<(float[], float[], float)> TEST = new List<(float[], float[], float)>(){
    //     (new float[]{-50f, 0f,  0f}, new float[]{180f}, 2f),
    //     (new float[]{+50f, 0f,  0f}, new float[]{ 90f}, 2f),
    //     (new float[]{+50f, 0f,+50f}, new float[]{  0f}, 2f),
    //     (new float[]{-50f, 0f,+50f, 10f}, new float[]{270f}, 2f)
    // };
    private List<(float[], float[], float)> TEST = new List<(float[], float[], float)>(){
        (new float[]{-80f, 0f,+80f}, new float[]{180f}, 2f),
        (new float[]{}, new float[]{90f, 18f}, 2f),
        (new float[]{-40f, 0f,  0f}, new float[]{}, 2f),
        (new float[]{}, new float[]{-45f}, 2f),
        (new float[]{-40f, 0f,+80f}, new float[]{}, 2f),
    };

    // Appearance:
    private Material matEnemy;
    private List<LineRenderer> lineListEngine = new List<LineRenderer>();
    public float fPositionZBase_lineEngine = -2f;
    public float fPositionZDelta_lineEngine = 0.25f;
    private float fPositionZLower_lineEngine;
    private float fPositionZUpper_lineEngine;

    // Health:
    private Health healthEnemy;
    private RectTransform rtCanvasHealth;
    public float fTimeWaitFlashDamaged = 0.1f;
    private bool bTriggeredDestroy = false;

    // Damage:
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

    public float fDistToPlayerAttack = 100f; // 200f for projectile, 100f for laser
    public float fDegreesRotationYToPlayerAttack = 5f;
    public bool bAttackOnlyIfPlayerInRange = false;
    public bool bAttackOnlyIfPlayerInSight = false;

    private int iNumAttack = 0;
    private int iNumAttackBurst;
    public int iNumMaxAttackBurst = 5;
    private float fTimeNextAttack;
    public float fTimeDeltaAttack = 0.5f;
    public float fTimeDeltaMinAttackBurst = 2f;
    public float fTimeDeltaMaxAttackBurst = 5f;

    private int iDamageLaser = 1;
    private bool bRayHitLastFrame = false;
    private bool bRayHitThisFrame = false;
    private bool bRayHitPlayerLastFrame = false;
    private bool bRayHitPlayerThisFrame = false;
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
        navEnemy.speed = fMetresPerSecMoveMaxDefault;
        navEnemy.updateRotation = false; // This is true by default, but that causes issues with the explicit rotation handling herein, so toggle off

        fRadiansPerSecMoveMaxDefault = fDegreesPerSecMoveMaxDefault * Mathf.PI/180f;
        fRadiansPerSecMoveMax = fRadiansPerSecMoveMaxDefault;

        v3TransformForwardLastFrame = transform.forward;

        matEnemy = transform.Find("Chasis").gameObject.GetComponent<Renderer>().material;
        foreach (Transform trn in transform.Find("Engine"))
        {
            lineListEngine.Add(trn.Find("Line : Engine").gameObject.GetComponent<LineRenderer>());
        }
        fPositionZLower_lineEngine = fPositionZBase_lineEngine - fPositionZDelta_lineEngine;
        fPositionZUpper_lineEngine = fPositionZBase_lineEngine + fPositionZDelta_lineEngine;

        healthEnemy = gameObject.AddComponent<Health>();
        healthEnemy.sliHealth = transform.Find("Canvas : Health/Slider : Health").GetComponent<Slider>();
        healthEnemy.Change(healthEnemy.iHealthMax);
        rtCanvasHealth = transform.Find("Canvas : Health").GetComponent<RectTransform>();

        if (transform.Find("Weapons/Line : Laser"))
        {
            goLaser = transform.Find("Weapons/Line : Laser").gameObject;
            lineLaser = goLaser.GetComponent<LineRenderer>();
            fPositionZBase_lineLaser = lineLaser.GetPosition(1).z;
        }
        goGunMiddleProjectileSpawnPoint = transform.Find("Weapons/GunMiddleProjectileSpawnPoint").gameObject;
        iNumAttackBurst = Random.Range(1, iNumMaxAttackBurst+1);
        fTimeNextAttack = Time.time + Random.Range(fTimeDeltaMinAttackBurst, fTimeDeltaMaxAttackBurst);

        goPlayer = GameObject.FindWithTag("Player");

        SetPositionTarget();
        SetRotationTarget();
    }

    // ------------------------------------------------------------------------------------------------

    void Update()
    {
        // ------------------------------------------------------------------------------------------------

        SetInfoThisFrame();

        // ------------------------------------------------------------------------------------------------

        // Movement:

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

        // Appearance:

        // Randomise the engine exhaust for a flickering effect:
        foreach (LineRenderer line in lineListEngine)
        {
            line.SetPosition(1, new Vector3(0f, 0f, Random.Range(fPositionZLower_lineEngine, fPositionZUpper_lineEngine)));
        }

        // ------------------------------------------------------------------------------------------------

        // Health:

        rtCanvasHealth.position = transform.position + new Vector3(0f, 0f, 10f);
        rtCanvasHealth.LookAt(transform.position + new Vector3(0f, 0f, 20f));

        // ------------------------------------------------------------------------------------------------

        // Damage:

        SetAttack();
        SetHealthPlayer();

        // ------------------------------------------------------------------------------------------------

        SetInfoLastFrame();

        // ------------------------------------------------------------------------------------------------
    }

    // ------------------------------------------------------------------------------------------------

    private void SetInfoThisFrame()
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

        v3PositionRelativePlayer = goPlayer.transform.position - transform.position;
        fDistToPlayer = v3PositionRelativePlayer.magnitude;

        fDegreesRotationYToPlayer = Vector3.Angle(transform.forward, v3PositionRelativePlayer);
        if (lookMode == LookMode.player)
        {
            fDegreesRotationYToTarget = fDegreesRotationYToPlayer;
        }
        else
        {
            fDegreesRotationYToTarget = Vector3.Angle(transform.forward, v3PositionRelativeTarget);
        }
    }

    // ------------------------------------------------------------------------------------------------

    private void SetInfoLastFrame()
    {
        bRayHitLastFrame = bRayHitThisFrame;
        bRayHitPlayerLastFrame = bRayHitPlayerThisFrame;
        bLaserDamagePlayerLastFrame = bLaserDamagePlayerThisFrame;
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
                &&  (bWaitAtPosition) )
            {
                StartCoroutine(WaitAtPosition());
            }
        }

        if (    (lookMode != LookMode.constant)
            &&  (!bRotationTargetArrived) )
        {
            CheckRotationTargetArrived();
            if (    (bRotationTargetArrived)
                &&  (bWaitAtRotation) )
            {
                StartCoroutine(WaitAtRotation());
            }
        }

        if (bTEST)
        {
            if (    (bPositionTargetArrived)
                &&  (!bWaitAtPosition)
                &&  (bRotationTargetArrived)
                &&  (!bWaitAtRotation) )
            {
                SetTEST();
            }
        }
        else if (   (moveMode == MoveMode.pattern)
                &&  (lookMode == LookMode.pattern)
                &&  (bSyncPattern) )
        {
            if (    (bPositionTargetArrived)
                &&  (!bWaitAtPosition)
                &&  (bRotationTargetArrived)
                &&  (!bWaitAtRotation) )
            {
                SetPositionTarget();
                SetRotationTarget();
            }
        }
        else
        {
            if (    (moveMode != MoveMode.constant)
                &&  (bPositionTargetArrived)
                &&  (!bWaitAtPosition) )
            {
                SetPositionTarget();
            }
            if (    (lookMode != LookMode.constant)
                &&  (bRotationTargetArrived)
                &&  (!bWaitAtRotation) )
            {
                SetRotationTarget();
            }
        }

        // Using v3PositionRelativeTarget in Quaternion.LookRotation() gives instant rotation towards the target
        // Using v3PositionRelativeTargetNow in Quaternion.LookRotation() gives delayed rotation towards the target
        v3PositionRelativeTargetNow = Vector3.RotateTowards(transform.forward, v3PositionRelativeTarget, fRadiansPerSecMoveMax * Time.deltaTime, 0f);
        transform.rotation = Quaternion.LookRotation(v3PositionRelativeTargetNow);
    }

    // ------------------------------------------------------------------------------------------------

    private void CheckPositionTargetArrived()
    {
        if (navEnemy.remainingDistance <= navEnemy.stoppingDistance)
        {
            if (    (navEnemy.velocity.magnitude == 0f)
                ||  (navEnemy.remainingDistance / navEnemy.velocity.magnitude <= fTimeStop) )
            {
                bPositionTargetArrived = true;
            }
        }
    }

    // ------------------------------------------------------------------------------------------------

    private void CheckRotationTargetArrived()
    {
        if (fDegreesRotationYToTarget <= fDegreesRotationYStop)
        {
            fDegreesPerSecMove = Vector3.Angle(transform.forward, v3TransformForwardLastFrame) / Time.deltaTime;
            if (    (fDegreesPerSecMove == 0f)
                ||  (fDegreesRotationYToTarget / fDegreesPerSecMove <= fTimeStop) )
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
            navEnemy.SetDestination(v3fListPositionPattern[iIdx_v3fListPositionPattern].Item1);
            fTimeWaitAtPosition = v3fListPositionPattern[iIdx_v3fListPositionPattern].Item2;
            bWaitAtPosition = fTimeWaitAtPosition > 0f;
            if (iIdx_v3fListPositionPattern < v3fListPositionPattern.Count-1)
            {
                iIdx_v3fListPositionPattern++;
            }
            else
            {
                iIdx_v3fListPositionPattern = 0;
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
            fRadiansRotationYConstant = fDegreesRotationYConstant * Mathf.PI/180f;
            v3PositionRelativeTarget = new Vector3(
                1000f * Mathf.Sin(fRadiansRotationYConstant),
                0f,
                1000f * Mathf.Cos(fRadiansRotationYConstant)
            );
            return;
        }
        if (lookMode == LookMode.pattern)
        {
            fDegreesRotationYTarget = ffListRotationPattern[iIdx_ffListRotationPattern].Item1;
            fRadiansRotationYTarget = fDegreesRotationYTarget * Mathf.PI/180f;
            v3PositionRelativeTarget = new Vector3(
                1000f * Mathf.Sin(fRadiansRotationYTarget),
                0f,
                1000f * Mathf.Cos(fRadiansRotationYTarget)
            );
            fTimeWaitAtRotation = ffListRotationPattern[iIdx_ffListRotationPattern].Item2;
            bWaitAtRotation = fTimeWaitAtRotation > 0f;
            if (iIdx_ffListRotationPattern < ffListRotationPattern.Count-1)
            {
                iIdx_ffListRotationPattern++;
            }
            else
            {
                iIdx_ffListRotationPattern = 0;
            }
            return;
        }
        if (lookMode == LookMode.player)
        {
            v3PositionRelativeTarget = v3PositionRelativePlayer;
            return;
        }
    }

    // ------------------------------------------------------------------------------------------------

    private void SetTEST()
    {
        bPositionTargetArrived = false;
        bRotationTargetArrived = false;

        if (TEST[iTEST].Item1.Length >= 3)
        {
            navEnemy.SetDestination(new Vector3(
                TEST[iTEST].Item1[0],
                TEST[iTEST].Item1[1],
                TEST[iTEST].Item1[2]
            ));
            if (TEST[iTEST].Item1.Length == 4)
            {
                navEnemy.speed = TEST[iTEST].Item1[3];
            }
            else
            {
                navEnemy.speed = fMetresPerSecMoveMaxDefault;
            }
        }

        if (TEST[iTEST].Item2.Length >= 1)
        {
            fDegreesRotationYTarget = TEST[iTEST].Item2[0];
            fRadiansRotationYTarget = fDegreesRotationYTarget * Mathf.PI/180f;
            v3PositionRelativeTarget = new Vector3(
                1000f * Mathf.Sin(fRadiansRotationYTarget),
                0f,
                1000f * Mathf.Cos(fRadiansRotationYTarget)
            );
            if (TEST[iTEST].Item2.Length == 2)
            {
                fDegreesPerSecMoveMax = TEST[iTEST].Item2[1];
                fRadiansPerSecMoveMax = fDegreesPerSecMoveMax * Mathf.PI/180f;
            }
            else
            {
                fRadiansPerSecMoveMax = fRadiansPerSecMoveMaxDefault;
            }
        }

        fTimeWaitAtPosition = TEST[iTEST].Item3;
        fTimeWaitAtRotation = fTimeWaitAtPosition;
        bWaitAtPosition = fTimeWaitAtPosition > 0f;
        bWaitAtRotation = bWaitAtPosition;

        if (iTEST < TEST.Count-1)
        {
            iTEST++;
        }
        else
        {
            iTEST = 0;
        }
    }

    // ------------------------------------------------------------------------------------------------

    private void SetAttack()
    {
        if (    (   (!bAttackOnlyIfPlayerInRange)
                ||  (fDistToPlayer <= fDistToPlayerAttack) )
            &&  (   (!bAttackOnlyIfPlayerInSight)
                ||  (fDegreesRotationYToPlayer <= fDegreesRotationYToPlayerAttack) ) )
        {
            if (    (attackMode1 == AttackMode1.projectile)
                &&  (goProjectile) )
            {
                SetAttackProjectile();
            }
            else if (   (attackMode1 == AttackMode1.laser)
                    &&  (goLaser) )
            {
                SetAttackLaser();
            }
        }
        else if (lineLaser.enabled)
        {
            lineLaser.enabled = false;
            CancelInvoke("PlaySfxLaser");
        }
    }

    // ------------------------------------------------------------------------------------------------

    private void SetAttackProjectile()
    {
        if (lineLaser.enabled)
        {
            lineLaser.enabled = false;
            CancelInvoke("PlaySfxLaser");
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
                fTimeNextAttack = Time.time + fTimeDeltaAttack;
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
                lineLaser.enabled = true;
                InvokeRepeating("PlaySfxLaser", 0f, 0.1f);
            }
            return;
        }
        if (attackMode2 == AttackMode2.burst)
        {
            return;
        }
        if (attackMode2 == AttackMode2.pattern)
        {
            return;
        }
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

    IEnumerator WaitAtPosition()
    {
        yield return new WaitForSeconds(fTimeWaitAtPosition);
        bWaitAtPosition = false;
    }

    // ------------------------------------------------------------------------------------------------

    IEnumerator WaitAtRotation()
    {
        yield return new WaitForSeconds(fTimeWaitAtRotation);
        bWaitAtRotation = false;
    }

    // ------------------------------------------------------------------------------------------------

    IEnumerator FlashDamaged()
    {
        matEnemy.EnableKeyword("_EMISSION");
        yield return new WaitForSeconds(fTimeWaitFlashDamaged);
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
