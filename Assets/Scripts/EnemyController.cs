using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    private GameManager gameManager;
    private SpawnManager spawnManager;

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

    // Health:
    private Health healthEnemy;
    public float fTimeFlashDamaged = 0.1f;
    private bool bDestroyTriggered = false;

    // Damage:
    public GameObject goProjectile;
    private GameObject goGunMiddleProjectileSpawnPoint;
    private int iNumFire = 0;
    private int iNumFireBurst;
    public int iNumFireBurstMax = 5;
    private float fTimeNextFire;
    public float fTimeNextFireDelta = 0.5f;
    public float fTimeNextFireDeltaBurstMin = 2f;
    public float fTimeNextFireDeltaBurstMax = 5f;

    // Score:
    public int iScoreDelta = 10;

    // Audio:
    private AudioSource audioSource;
    private List<AudioClip> sfxclpListProjectile = new List<AudioClip>();

    // From other objects:
    private GameObject goPlayer;

    // ------------------------------------------------------------------------------------------------

    void Start()
    {
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
        spawnManager = GameObject.Find("Spawn Manager").GetComponent<SpawnManager>();

        navEnemy = GetComponent<NavMeshAgent>();

        matEnemy = transform.Find("Chasis").gameObject.GetComponent<Renderer>().material;

        healthEnemy = GetComponent<Health>();

        goGunMiddleProjectileSpawnPoint = transform.Find("Chasis/GunMiddleProjectileSpawnPoint").gameObject;
        iNumFireBurst = Random.Range(1, iNumFireBurstMax+1);
        fTimeNextFire = Time.time + Random.Range(fTimeNextFireDeltaBurstMin, fTimeNextFireDeltaBurstMax);

        audioSource = GameObject.Find("Audio Source").GetComponent<AudioSource>();
        sfxclpListProjectile.Add((AudioClip)AssetDatabase.LoadAssetAtPath("Assets/Asset Store/Audio/MGWSoundDesign/FuturisticGunSoundFX/Laser/Laser6.wav", typeof(AudioClip)));
        sfxclpListProjectile.Add((AudioClip)AssetDatabase.LoadAssetAtPath("Assets/Asset Store/Audio/MGWSoundDesign/FuturisticGunSoundFX/Laser/Laser7.wav", typeof(AudioClip)));
        sfxclpListProjectile.Add((AudioClip)AssetDatabase.LoadAssetAtPath("Assets/Asset Store/Audio/MGWSoundDesign/FuturisticGunSoundFX/Laser/Laser8.wav", typeof(AudioClip)));
        sfxclpListProjectile.Add((AudioClip)AssetDatabase.LoadAssetAtPath("Assets/Asset Store/Audio/MGWSoundDesign/FuturisticGunSoundFX/Laser/Laser9.wav", typeof(AudioClip)));
        sfxclpListProjectile.Add((AudioClip)AssetDatabase.LoadAssetAtPath("Assets/Asset Store/Audio/MGWSoundDesign/FuturisticGunSoundFX/Laser/Laser10.wav", typeof(AudioClip)));
        sfxclpListProjectile.Add((AudioClip)AssetDatabase.LoadAssetAtPath("Assets/Asset Store/Audio/MGWSoundDesign/FuturisticGunSoundFX/Laser/Laser16.wav", typeof(AudioClip)));
        sfxclpListProjectile.Add((AudioClip)AssetDatabase.LoadAssetAtPath("Assets/Asset Store/Audio/MGWSoundDesign/FuturisticGunSoundFX/Laser/Laser17.wav", typeof(AudioClip)));
        sfxclpListProjectile.Add((AudioClip)AssetDatabase.LoadAssetAtPath("Assets/Asset Store/Audio/MGWSoundDesign/FuturisticGunSoundFX/Laser/Laser18.wav", typeof(AudioClip)));
        sfxclpListProjectile.Add((AudioClip)AssetDatabase.LoadAssetAtPath("Assets/Asset Store/Audio/MGWSoundDesign/FuturisticGunSoundFX/Laser/Laser19.wav", typeof(AudioClip)));
        sfxclpListProjectile.Add((AudioClip)AssetDatabase.LoadAssetAtPath("Assets/Asset Store/Audio/MGWSoundDesign/FuturisticGunSoundFX/Laser/Laser20.wav", typeof(AudioClip)));

        goPlayer = GameObject.FindWithTag("Player");

        SetDestination();
    }

    // ------------------------------------------------------------------------------------------------

    void Update()
    {
        // Movement:

        if (    (moveMode != MoveMode.constant)
            &&  (navEnemy.remainingDistance <= navEnemy.stoppingDistance) )
        {
            SetDestination();
        }

        // ------------------------------------------------------------------------------------------------

        // Look:

        Vector3 v3PositionRelativeLook = goPlayer.transform.position - transform.position;
        Vector3 v3PositionRelativeLookNow = Vector3.RotateTowards(transform.forward, v3PositionRelativeLook, fAngSpeedMove * Time.deltaTime, 0f);
        transform.rotation = Quaternion.LookRotation(v3PositionRelativeLookNow);

        // ------------------------------------------------------------------------------------------------

        // Damage:

        if (v3PositionRelativeLook.magnitude <= 200f)
        {
            if (Time.time >= fTimeNextFire)
            {
                GameObject goProjectileClone = Instantiate(
                    goProjectile,
                    goGunMiddleProjectileSpawnPoint.transform.position,
                    goGunMiddleProjectileSpawnPoint.transform.rotation
                );
                goProjectileClone.GetComponent<ProjectileController>().fForceMove = 100f;
                audioSource.PlayOneShot(sfxclpListProjectile[UnityEngine.Random.Range(0, sfxclpListProjectile.Count)], 0.1f);
                iNumFire += 1;
                if (iNumFire < iNumFireBurst)
                {
                    fTimeNextFire = Time.time + fTimeNextFireDelta;
                }
                else
                {
                    iNumFire = 0;
                    iNumFireBurst = Random.Range(1, iNumFireBurstMax+1);
                    fTimeNextFire = Time.time + Random.Range(fTimeNextFireDeltaBurstMin, fTimeNextFireDeltaBurstMax);
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

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("ProjectilePlayer"))
        {
            healthEnemy.Change(-collision.gameObject.GetComponent<ProjectileController>().iDamage);
            if (    (healthEnemy.iHealth == 0)
                &&  (!bDestroyTriggered) )
            {
                bDestroyTriggered = true;
                spawnManager.iNumEnemy -= 1;
                gameManager.ChangeScore(iScoreDelta);
                if (Random.Range(0, 10) >= 5)
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
                spawnManager.SpawnExplosionEnemy(transform.position);
                Destroy(gameObject);
            }
            StartCoroutine(FlashDamaged());
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
