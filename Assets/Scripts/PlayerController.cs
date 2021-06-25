using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    private GameManager gameManager;

    // Movement:
    private float fInputHorz;
    private float fInputVert;
    private Rigidbody rbPlayer;
    public float fForceMove = 1e7f;
    public float fForceMoveDeltaBoost = 1e4f;
    // public float fMetresPerSecMove = 100f;
    // public float fMetresPerSecMoveDeltaBoost = 20f;
    private Vector3 v3DirectionMove;
    private Vector3 v3MoveLimitLowerLeft;
    private Vector3 v3MoveLimitUpperRight;
    private Vector3 v3MoveLimitCamOffset = new Vector3(5f, 0f, 5f);

    // Appearance:
    private List<Material> matListChildren = new List<Material>();

    // Health:
    private Health healthPlayer;
    private GameObject goShield;
    public float fTimeFlashDamaged = 0.1f;
    private float fRelativeMomentum;
    public float fRelativeMomentumBenchmark = 250000f;

    // Damage:
    public GameObject goProjectile;
    private GameObject goGunLeftProjectileSpawnPoint;
    private GameObject goGunRightProjectileSpawnPoint;
    private float fTimeNextFire;
    public float fTimeNextFireDelta = 0.1f;

    // ------------------------------------------------------------------------------------------------

    void Start()
    {
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();

        rbPlayer = GetComponent<Rigidbody>();
        v3MoveLimitLowerLeft = gameManager.v3CamLowerLeft + v3MoveLimitCamOffset;
        v3MoveLimitUpperRight = gameManager.v3CamUpperRight - v3MoveLimitCamOffset;

        foreach (Renderer rendChild in GetComponentsInChildren<Renderer>())
        {
            matListChildren.Add(rendChild.material);
        }

        healthPlayer = GetComponent<Health>();
        goShield = transform.Find("Shield").gameObject;

        goGunLeftProjectileSpawnPoint = transform.Find("08_Gun_L/GunLeftProjectileSpawnPoint").gameObject;
        goGunRightProjectileSpawnPoint = transform.Find("08_Gun_R/GunRightProjectileSpawnPoint").gameObject;
        fTimeNextFire = Time.time;
    }

    // ------------------------------------------------------------------------------------------------

    void Update()
    {
        fInputHorz = Input.GetAxis("Horizontal");
        fInputVert = Input.GetAxis("Vertical");

        // Debug.LogFormat("{0} {1} {2}", fInputHorz, fInputVert, Math.Pow(Math.Pow(fInputHorz,2f) + Math.Pow(fInputVert,2f), 0.5f));

        // if (    ((fInputHorz < 0) && (transform.position.x <= v3MoveLimitLowerLeft.x))
        //     ||  ((fInputHorz > 0) && (transform.position.x >= v3MoveLimitUpperRight.x)) )
        // {
        //     fInputHorz = 0f;
        // }
        // if (    ((fInputVert < 0) && (transform.position.z <= v3MoveLimitLowerLeft.z))
        //     ||  ((fInputVert > 0) && (transform.position.z >= v3MoveLimitUpperRight.z)) )
        // {
        //     fInputVert = 0f;
        // }
        // if (Math.Abs(fInputHorz) + Math.Abs(fInputVert) > 0f)
        // {
        //     v3DirectionMove = (fInputHorz * transform.right) + (fInputVert * transform.forward);
        //     if (v3DirectionMove.magnitude > 1f)
        //     {
        //         v3DirectionMove = v3DirectionMove.normalized;
        //     }
        //     // transform.position = Vector3.MoveTowards(transform.position, transform.position + v3DirectionMove, fMetresPerSecMove * Time.deltaTime);
        //     transform.Translate(fMetresPerSecMove * Time.deltaTime * v3DirectionMove);
        // }

        if (    ((fInputHorz < 0) && (transform.position.x <= v3MoveLimitLowerLeft.x))
            ||  ((fInputHorz > 0) && (transform.position.x >= v3MoveLimitUpperRight.x)) )
        {
            // fInputHorz *= -1f; // This doesn't work so well, gives bouncy behaviour at move limits
            fInputHorz = 0f;
            rbPlayer.AddForce(30f * rbPlayer.mass * new Vector3(-rbPlayer.velocity.x, 0f, 0f));
        }
        if (    ((fInputVert < 0) && (transform.position.z <= v3MoveLimitLowerLeft.z))
            ||  ((fInputVert > 0) && (transform.position.z >= v3MoveLimitUpperRight.z)) )
        {
            // fInputVert *= -1f; // This doesn't work so well, gives bouncy behaviour at move limits
            fInputVert = 0f;
            rbPlayer.AddForce(30f * rbPlayer.mass * new Vector3(0f, 0f, -rbPlayer.velocity.z));
        }

        if (Math.Abs(fInputHorz) + Math.Abs(fInputVert) > 0f)
        {
            v3DirectionMove = (fInputHorz * transform.right) + (fInputVert * transform.forward);
            if (v3DirectionMove.magnitude > 1f)
            {
                v3DirectionMove = v3DirectionMove.normalized;
            }
            rbPlayer.AddForce(fForceMove * v3DirectionMove);
        }

        // ------------------------------------------------------------------------------------------------

        if (Input.GetButton("Fire1"))
        {
            if (Time.time >= fTimeNextFire)
            {
                Instantiate(goProjectile, goGunLeftProjectileSpawnPoint.transform.position, goGunLeftProjectileSpawnPoint.transform.rotation);
                Instantiate(goProjectile, goGunRightProjectileSpawnPoint.transform.position, goGunRightProjectileSpawnPoint.transform.rotation);
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
        }
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
            // if (healthPlayer.iHealth == 0)
            // {
            //     Destroy(gameObject);
            // }
            StartCoroutine(FlashDamaged());
            return;
        }
        if (collision.gameObject.CompareTag("EnemyProjectile"))
        {
            healthPlayer.Change(-collision.gameObject.GetComponent<ProjectileController>().iDamage);
            // if (healthPlayer.iHealth == 0)
            // {
            //     Destroy(gameObject);
            // }
            StartCoroutine(FlashDamaged());
            return;
        }
    }

    // ------------------------------------------------------------------------------------------------

}
