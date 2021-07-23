using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    private GameManager gameManager;

    // Movement:
    private float fInputHorzMove;
    private float fInputVertMove;
    private float fInputHorzLook;
    private float fInputVertLook;
    private Rigidbody rbPlayer;
    public float fForceMove = 1e7f;
    public float fForceMoveDeltaBoost = 1e4f;
    public float fAngSpeedMove = 5f;
    // public float fMetresPerSecMove = 100f;
    // public float fMetresPerSecMoveDeltaBoost = 20f;
    private Vector3 v3DirectionMove;

    // Appearance:
    private List<Material> matListChildren = new List<Material>();

    // Health:
    private Health healthPlayer;
    private GameObject goShield;
    public float fTimeFlashDamaged = 0.1f;
    private float fRelativeMomentum;
    public float fRelativeMomentumBenchmark = 250000f;

    // Damage:
    private Charge chargePlayer;
    public GameObject goProjectile;
    private GameObject goGunLeftProjectileSpawnPoint;
    private GameObject goGunRightProjectileSpawnPoint;
    private float fTimeNextFire;
    public float fTimeNextFireDelta = 0.1f;

    // Audio:
    private AudioSource audioSource;
    private AudioClip sfxclpPowerUpHealth;
    private AudioClip sfxclpPowerUpCharge;
    private List<AudioClip> sfxclpListProjectile = new List<AudioClip>();

    // ------------------------------------------------------------------------------------------------

    void Start()
    {
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();

        rbPlayer = GetComponent<Rigidbody>();

        foreach (Renderer rendChild in GetComponentsInChildren<Renderer>())
        {
            matListChildren.Add(rendChild.material);
        }

        healthPlayer = GetComponent<Health>();
        healthPlayer.sliHealth = GameObject.Find("Slider : Health").GetComponent<Slider>();
        goShield = transform.Find("Shield").gameObject;

        chargePlayer = GetComponent<Charge>();
        chargePlayer.sliCharge = GameObject.Find("Slider : Charge").GetComponent<Slider>();
        goGunLeftProjectileSpawnPoint = transform.Find("08_Gun_L/GunLeftProjectileSpawnPoint").gameObject;
        goGunRightProjectileSpawnPoint = transform.Find("08_Gun_R/GunRightProjectileSpawnPoint").gameObject;
        fTimeNextFire = Time.time;

        audioSource = GameObject.Find("Audio Source").GetComponent<AudioSource>();
        sfxclpPowerUpHealth = (AudioClip)AssetDatabase.LoadAssetAtPath("Assets/Asset Store/Audio/Little Robot Sound Factory/Electric Sfx/Mp3/Jingle_Win_Synth/Jingle_Win_Synth_03.mp3", typeof(AudioClip));
        sfxclpPowerUpCharge = (AudioClip)AssetDatabase.LoadAssetAtPath("Assets/Asset Store/Audio/Little Robot Sound Factory/Electric Sfx/Mp3/Jingle_Win_Synth/Jingle_Win_Synth_04.mp3", typeof(AudioClip));
        sfxclpListProjectile.Add((AudioClip)AssetDatabase.LoadAssetAtPath("Assets/Asset Store/Audio/Shapeforms/Shapeforms Audio Free Sound Effects/Type Preview/AUDIO/Tablet_Swipe_01.wav", typeof(AudioClip)));
        // sfxclpListProjectile.Add((AudioClip)AssetDatabase.LoadAssetAtPath("Assets/Asset Store/Audio/Shapeforms/Shapeforms Audio Free Sound Effects/Hit and Punch Preview/AUDIO/WHOOSH_AIRY_FLUTTER_01.wav", typeof(AudioClip))); // A bit too harsh for constant firing
        // sfxclpListProjectile.Add((AudioClip)AssetDatabase.LoadAssetAtPath("Assets/Asset Store/Audio/MGWSoundDesign/FuturisticGunSoundFX/Laser/Laser10.wav", typeof(AudioClip))); // A bit too harsh for constant firing
        // sfxclpListProjectile.Add((AudioClip)AssetDatabase.LoadAssetAtPath("Assets/Asset Store/Audio/MGWSoundDesign/FuturisticGunSoundFX/Laser/Laser20.wav", typeof(AudioClip))); // A bit too harsh for constant firing
        // sfxclpListProjectile.Add((AudioClip)AssetDatabase.LoadAssetAtPath("Assets/Asset Store/Audio/Little Robot Sound Factory/UI Sfx/Mp3/Click_Electronic/Click_Electronic_05.mp3", typeof(AudioClip))); // Okay, but a bit too chirpy like a little bird
        // sfxclpListProjectile.Add((AudioClip)AssetDatabase.LoadAssetAtPath("Assets/Asset Store/Audio/Little Robot Sound Factory/UI Sfx/Mp3/Click_Standard_00.mp3", typeof(AudioClip))); // Okay, but a bit too clicky like a typewriter
    }

    // ------------------------------------------------------------------------------------------------

    void Update()
    {
        // Movement:

        fInputHorzMove = Input.GetAxis("Horizontal Move");
        fInputVertMove = Input.GetAxis("Vertical Move");

        // Debug.LogFormat("{0} {1} {2}", fInputHorzMove, fInputVertMove, Math.Pow(Math.Pow(fInputHorzMove,2f) + Math.Pow(fInputVertMove,2f), 0.5f));

        // if (    ((fInputHorzMove < 0) && (transform.position.x <= gameManager.v3MoveLimitLowerLeft.x))
        //     ||  ((fInputHorzMove > 0) && (transform.position.x >= gameManager.v3MoveLimitUpperRight.x)) )
        // {
        //     fInputHorzMove = 0f;
        // }
        // if (    ((fInputVertMove < 0) && (transform.position.z <= gameManager.v3MoveLimitLowerLeft.z))
        //     ||  ((fInputVertMove > 0) && (transform.position.z >= gameManager.v3MoveLimitUpperRight.z)) )
        // {
        //     fInputVertMove = 0f;
        // }
        // if (Math.Abs(fInputHorzMove) + Math.Abs(fInputVertMove) > 0f)
        // {
        //     v3DirectionMove = (fInputHorzMove * transform.right) + (fInputVertMove * transform.forward);
        //     if (v3DirectionMove.magnitude > 1f)
        //     {
        //         v3DirectionMove = v3DirectionMove.normalized;
        //     }
        //     // transform.position = Vector3.MoveTowards(transform.position, transform.position + v3DirectionMove, fMetresPerSecMove * Time.deltaTime);
        //     transform.Translate(fMetresPerSecMove * Time.deltaTime * v3DirectionMove);
        // }

        if (    ((fInputHorzMove < 0) && (transform.position.x <= gameManager.v3MoveLimitLowerLeft.x))
            ||  ((fInputHorzMove > 0) && (transform.position.x >= gameManager.v3MoveLimitUpperRight.x)) )
        {
            // fInputHorzMove *= -1f; // This doesn't work so well, gives bouncy behaviour at move limits
            fInputHorzMove = 0f;
            rbPlayer.AddForce(30f * rbPlayer.mass * new Vector3(-rbPlayer.velocity.x, 0f, 0f));
        }
        if (    ((fInputVertMove < 0) && (transform.position.z <= gameManager.v3MoveLimitLowerLeft.z))
            ||  ((fInputVertMove > 0) && (transform.position.z >= gameManager.v3MoveLimitUpperRight.z)) )
        {
            // fInputVertMove *= -1f; // This doesn't work so well, gives bouncy behaviour at move limits
            fInputVertMove = 0f;
            rbPlayer.AddForce(30f * rbPlayer.mass * new Vector3(0f, 0f, -rbPlayer.velocity.z));
        }

        if (Math.Abs(fInputHorzMove) + Math.Abs(fInputVertMove) > 0f)
        {
            v3DirectionMove = (fInputHorzMove * Vector3.right) + (fInputVertMove * Vector3.forward);
            if (v3DirectionMove.magnitude > 1f)
            {
                v3DirectionMove = v3DirectionMove.normalized;
            }
            rbPlayer.AddForce(fForceMove * v3DirectionMove);
        }

        // ------------------------------------------------------------------------------------------------

        // Look:

        fInputHorzLook = Input.GetAxis("Horizontal Look");
        fInputVertLook = Input.GetAxis("Vertical Look");

        if (Math.Abs(fInputHorzLook) + Math.Abs(fInputVertLook) > 0f)
        {
            Vector3 v3PositionRelativeLook = (new Vector3(fInputHorzLook, 0f, fInputVertLook).normalized);
            Vector3 v3PositionRelativeLookNow = Vector3.RotateTowards(transform.forward, v3PositionRelativeLook, fAngSpeedMove * Time.deltaTime, 0f);
            transform.rotation = Quaternion.LookRotation(v3PositionRelativeLookNow);
        }

        // ------------------------------------------------------------------------------------------------

        // Damage:

        if (Input.GetButton("Fire1"))
        {
            if (Time.time >= fTimeNextFire)
            {
                GameObject goProjectileClone1 = Instantiate(
                    goProjectile,
                    goGunLeftProjectileSpawnPoint.transform.position,
                    goGunLeftProjectileSpawnPoint.transform.rotation
                );
                GameObject goProjectileClone2 = Instantiate(
                    goProjectile,
                    goGunRightProjectileSpawnPoint.transform.position,
                    goGunRightProjectileSpawnPoint.transform.rotation
                );
                // audioSource.PlayOneShot(sfxclpListProjectile[UnityEngine.Random.Range(0, sfxclpListProjectile.Count)], 0.75f);
                audioSource.PlayOneShot(sfxclpListProjectile[0], 0.75f);
                fTimeNextFire = Time.time + fTimeNextFireDelta;
            }
        }

        // ------------------------------------------------------------------------------------------------

        // Just to test the health component:

        if (Input.GetKeyDown(KeyCode.G))
        {
            healthPlayer.Change(10);
        }
        if (Input.GetKeyDown(KeyCode.H))
        {
            healthPlayer.Change(-10);
            if (healthPlayer.iHealth == 0)
            {
                gameManager.GameOver();
            }
        }

        // ------------------------------------------------------------------------------------------------

        // Just to test the charge component:

        if (Input.GetKeyDown(KeyCode.J))
        {
            chargePlayer.Change(10);
        }
        if (Input.GetKeyDown(KeyCode.K))
        {
            chargePlayer.Change(-10);
        }

        // ------------------------------------------------------------------------------------------------

    }

    // ------------------------------------------------------------------------------------------------

    IEnumerator FlashDamaged()
    {
        // goShield.SetActive(true);
        // yield return new WaitForSeconds(fTimeFlashDamaged);
        // goShield.SetActive(false);
        // yield return new WaitForSeconds(fTimeFlashDamaged);
        // goShield.SetActive(true);
        // yield return new WaitForSeconds(fTimeFlashDamaged);
        // goShield.SetActive(false);

        foreach (Material matChild in matListChildren)
        {
            matChild.EnableKeyword("_EMISSION");
        }

        yield return new WaitForSeconds(fTimeFlashDamaged);

        foreach (Material matChild in matListChildren)
        {
            matChild.DisableKeyword("_EMISSION");
        }
    }

    // ------------------------------------------------------------------------------------------------

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Asteroid"))
        {
            // The approximate extremes we are looking at here for a stationary player are:
            //   1,000 kg asteroid with relative speed of 1 ms^-1 => relative momentum = 1 kg ms^-1
            //   5,000 kg asteroid with relative speed of 50 ms^-1 => relative momentum = 250,000 kg ms^-1
            fRelativeMomentum = collision.rigidbody.mass * collision.relativeVelocity.magnitude;
            healthPlayer.Change(-(int)Math.Ceiling(collision.gameObject.GetComponent<AsteroidController>().iDamage * fRelativeMomentum / fRelativeMomentumBenchmark));
            StartCoroutine(FlashDamaged());
            if (healthPlayer.iHealth == 0)
            {
                gameManager.GameOver();
            }
            return;
        }
        if (collision.gameObject.CompareTag("ProjectileEnemy"))
        {
            healthPlayer.Change(-collision.gameObject.GetComponent<ProjectileController>().iDamage);
            StartCoroutine(FlashDamaged());
            if (healthPlayer.iHealth == 0)
            {
                gameManager.GameOver();
            }
            return;
        }
        if (collision.gameObject.CompareTag("PowerUpHealth"))
        {
            if (healthPlayer.iHealth < healthPlayer.iHealthMax)
            {
                healthPlayer.Change(collision.gameObject.GetComponent<PowerUpController>().iValue);
                audioSource.PlayOneShot(sfxclpPowerUpHealth, 0.5f);
                Destroy(collision.gameObject);
            }
            return;
        }
        if (collision.gameObject.CompareTag("PowerUpCharge"))
        {
            if (chargePlayer.iCharge < chargePlayer.iChargeMax)
            {
                chargePlayer.Change(collision.gameObject.GetComponent<PowerUpController>().iValue);
                audioSource.PlayOneShot(sfxclpPowerUpCharge, 0.5f);
                Destroy(collision.gameObject);
            }
            return;
        }
    }

    // ------------------------------------------------------------------------------------------------

}
