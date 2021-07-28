using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    private GameManager gameManager;
    private AudioManager audioManager;

    // Movement:
    private float fInputHorzMove;
    private float fInputVertMove;
    private float fInputHorzLook;
    private float fInputVertLook;
    private Rigidbody rbPlayer;
    public float fForceMove = 4e7f;
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
    private Charge chargeAttack1;
    private Charge chargeAttack2;

    public GameObject goProjectile;
    private GameObject goGunLeftProjectileSpawnPoint;
    private GameObject goGunRightProjectileSpawnPoint;
    private GameObject goGunMiddleProjectileSpawnPoint;

    private bool bTriggeredAttack1Auto = false;
    private bool bTriggeredAttack1Wait = false;

    private float fTimeNextAttack1 = 0f;
    private float fTimeNextAttack1Decharge = 0f;
    private float fTimeNextAttack1Recharge = 0f;

    public float fTimeDeltaAttack1 = 0.1f;
    public float fTimeDeltaAttack1Decharge = 0.1f;
    public float fTimeDeltaAttack1Recharge = 0.02f;
    public float fTimeDeltaAttack1RechargeWait = 0.1f;

    public enum Attack2Mode {straight, spiral};
    public Attack2Mode attack2Mode;
    private int iIdxLastAttack2Mode = Enum.GetNames(typeof(Attack2Mode)).Length - 1;

    private bool bTriggeredAttack2 = false;

    private float fTimeNextAttack2 = 0f;
    private float fTimeNextAttack2Decharge = 0f;
    private float fAngleNextAttack2 = 0f;

    public float fForceMoveAttack2ModeStraight = 400f;
    public float fTimeDeltaAttack2ModeStraight = 0.01f;
    public float fTimeDeltaAttack2DechargeModeStraight = 0.05f;

    public float fForceMoveAttack2ModeSpiral = 50f;
    public float fTimeDeltaAttack2ModeSpiral = 0.01f;
    public float fTimeDeltaAttack2DechargeModeSpiral = 0.02f;
    public float fAngleDeltaAttack2ModeSpiral = 10f;

    private class Icon
    {
        private RawImage imgUnselected;
        private RawImage imgSelected;

        public Icon(GameObject goUnselectedGiven, GameObject goSelectedGiven)
        {
            imgUnselected = goUnselectedGiven.GetComponent<RawImage>();
            imgSelected = goSelectedGiven.GetComponent<RawImage>();
            imgUnselected.enabled = true;
            imgSelected.enabled = false;
        }

        public void Toggle()
        {
            imgUnselected.enabled = !imgUnselected.enabled;
            imgSelected.enabled = !imgSelected.enabled;
        }
    }
    private List<Icon> iconListAttack2Mode = new List<Icon>();

    // ------------------------------------------------------------------------------------------------

    void Start()
    {
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
        audioManager = GameObject.Find("Audio Manager").GetComponent<AudioManager>();

        rbPlayer = GetComponent<Rigidbody>();

        foreach (Renderer rendChild in GetComponentsInChildren<Renderer>())
        {
            matListChildren.Add(rendChild.material);
        }

        healthPlayer = gameObject.AddComponent<Health>();
        healthPlayer.sliHealth = GameObject.Find("Slider : Health").GetComponent<Slider>();
        goShield = transform.Find("Shield").gameObject;

        chargeAttack1 = gameObject.AddComponent<Charge>();
        chargeAttack1.sliCharge = GameObject.Find("Slider : Charge : Attack1").GetComponent<Slider>();
        chargeAttack1.Change(chargeAttack1.iChargeMax);

        chargeAttack2 = gameObject.AddComponent<Charge>();
        chargeAttack2.sliCharge = GameObject.Find("Slider : Charge : Attack2").GetComponent<Slider>();

        goGunLeftProjectileSpawnPoint = transform.Find("08_Gun_L/GunLeftProjectileSpawnPoint").gameObject;
        goGunRightProjectileSpawnPoint = transform.Find("08_Gun_R/GunRightProjectileSpawnPoint").gameObject;
        goGunMiddleProjectileSpawnPoint = transform.Find("02_CockpitExtension/GunMiddleProjectileSpawnPoint").gameObject;

        iconListAttack2Mode.Add(new Icon(
            GameObject.Find("RawImage : IconAttack2ModeStraightUnselected"),
            GameObject.Find("RawImage : IconAttack2ModeStraightSelected")
        ));
        iconListAttack2Mode.Add(new Icon(
            GameObject.Find("RawImage : IconAttack2ModeSpiralUnselected"),
            GameObject.Find("RawImage : IconAttack2ModeSpiralSelected")
        ));
        iconListAttack2Mode[0].Toggle();
    }

    // ------------------------------------------------------------------------------------------------

    void Update()
    {

        // ------------------------------------------------------------------------------------------------

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
            rbPlayer.AddForce(150f * rbPlayer.mass * new Vector3(-rbPlayer.velocity.x, 0f, 0f));
        }
        if (    ((fInputVertMove < 0) && (transform.position.z <= gameManager.v3MoveLimitLowerLeft.z))
            ||  ((fInputVertMove > 0) && (transform.position.z >= gameManager.v3MoveLimitUpperRight.z)) )
        {
            // fInputVertMove *= -1f; // This doesn't work so well, gives bouncy behaviour at move limits
            fInputVertMove = 0f;
            rbPlayer.AddForce(150f * rbPlayer.mass * new Vector3(0f, 0f, -rbPlayer.velocity.z));
        }

        // ------------------------------------------------------------------------------------------------

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

        fInputHorzLook = Input.GetAxis("Horizontal Look");
        fInputVertLook = Input.GetAxis("Vertical Look");

        if (Math.Abs(fInputHorzLook) + Math.Abs(fInputVertLook) > 0f)
        {
            Vector3 v3PositionRelativeLook = (new Vector3(fInputHorzLook, 0f, fInputVertLook).normalized);
            Vector3 v3PositionRelativeLookNow = Vector3.RotateTowards(transform.forward, v3PositionRelativeLook, fAngSpeedMove * Time.deltaTime, 0f);
            transform.rotation = Quaternion.LookRotation(v3PositionRelativeLookNow);
        }

        // ------------------------------------------------------------------------------------------------

        // Damage: Attack1:

        if (Input.GetButtonDown("Attack1 Auto"))
        {
            bTriggeredAttack1Auto = !bTriggeredAttack1Auto;
        }

        // ------------------------------------------------------------------------------------------------

        // Damage: Attack2:

        if (Input.GetButtonDown("Attack2 Mode"))
        {
            iconListAttack2Mode[(int)attack2Mode].Toggle();

            if ((int)attack2Mode == iIdxLastAttack2Mode)
            {
                attack2Mode = (Attack2Mode)(0);
            }
            else
            {
                attack2Mode = (Attack2Mode)((int)attack2Mode + 1);
            }

            iconListAttack2Mode[(int)attack2Mode].Toggle();
        }

        if (    (Input.GetButtonDown("Attack2"))
            &&  (!bTriggeredAttack2)
            &&  (chargeAttack2.iCharge == chargeAttack2.iChargeMax) )
        {
            bTriggeredAttack2 = true;
            fTimeNextAttack2 = 0f;
            fAngleNextAttack2 = 0f;
            switch (attack2Mode)
            {
                case Attack2Mode.straight: fTimeNextAttack2Decharge = Time.time + fTimeDeltaAttack2DechargeModeStraight; break;
                case Attack2Mode.spiral: fTimeNextAttack2Decharge = Time.time + fTimeDeltaAttack2DechargeModeSpiral; break;
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
            chargeAttack2.Change(10);
        }
        if (Input.GetKeyDown(KeyCode.K))
        {
            chargeAttack2.Change(-10);
        }

        // ------------------------------------------------------------------------------------------------

    }

    void FixedUpdate()
    {

        // ------------------------------------------------------------------------------------------------

        // Damage: Attack1:

        if (    (   (Input.GetButton("Attack1"))
                ||  (bTriggeredAttack1Auto) )
            &&  (!bTriggeredAttack1Wait) )
        {

            // ------------------------------------------------------------------------------------------------

            if (Time.time >= fTimeNextAttack1)
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
                // audioManager.sfxclpvolListProjectilePlayer[UnityEngine.Random.Range(0, audioManager.sfxclpvolListProjectilePlayer.Count)].PlayOneShot();
                audioManager.sfxclpvolListProjectilePlayer[0].PlayOneShot();
                fTimeNextAttack1 = Time.time + fTimeDeltaAttack1;
            }

            // ------------------------------------------------------------------------------------------------

            if (Time.time >= fTimeNextAttack1Decharge)
            {
                chargeAttack1.Change(-1);
                if (chargeAttack1.iCharge == 0)
                {
                    bTriggeredAttack1Wait = true;
                }
                fTimeNextAttack1Decharge = Time.time + fTimeDeltaAttack1Decharge;
            }

            // ------------------------------------------------------------------------------------------------

        }
        else if (chargeAttack1.iCharge < chargeAttack1.iChargeMax)
        {
            if (Time.time >= fTimeNextAttack1Recharge)
            {
                chargeAttack1.Change(+1);
                if (chargeAttack1.iCharge == chargeAttack1.iChargeMax)
                {
                    bTriggeredAttack1Wait = false;
                }
                switch (bTriggeredAttack1Wait)
                {
                    case false: fTimeNextAttack1Recharge = Time.time + fTimeDeltaAttack1Recharge; break;
                    case true: fTimeNextAttack1Recharge = Time.time + fTimeDeltaAttack1RechargeWait; break;
                }
            }
        }

        // ------------------------------------------------------------------------------------------------

        // Damage: Attack2:

        if (bTriggeredAttack2)
        {

            // ------------------------------------------------------------------------------------------------

            if (Time.time >= fTimeNextAttack2)
            {
                if (attack2Mode == Attack2Mode.straight)
                {
                    GameObject goProjectileClone = Instantiate(
                        goProjectile,
                        goGunMiddleProjectileSpawnPoint.transform.position,
                        transform.rotation
                    );
                    goProjectileClone.GetComponent<ProjectileController>().fForceMove = fForceMoveAttack2ModeStraight;

                    fTimeNextAttack2 = Time.time + fTimeDeltaAttack2ModeStraight;
                }
                else if (attack2Mode == Attack2Mode.spiral)
                {
                    for (int i=0; i<4; i++)
                    {
                        GameObject goProjectileClone = Instantiate(
                            goProjectile,
                            goGunMiddleProjectileSpawnPoint.transform.position,
                            Quaternion.Euler(0f, i*90f + fAngleNextAttack2 + transform.rotation.eulerAngles.y, 0f)
                        );
                        goProjectileClone.GetComponent<ProjectileController>().fForceMove = fForceMoveAttack2ModeSpiral;
                    }

                    fTimeNextAttack2 = Time.time + fTimeDeltaAttack2ModeSpiral;
                    fAngleNextAttack2 += fAngleDeltaAttack2ModeSpiral;
                }
            }

            // ------------------------------------------------------------------------------------------------

            if (Time.time >= fTimeNextAttack2Decharge)
            {
                chargeAttack2.Change(-1);
                if (chargeAttack2.iCharge == 0)
                {
                    bTriggeredAttack2 = false;
                }
                else
                {
                    switch (attack2Mode)
                    {
                        case Attack2Mode.straight: fTimeNextAttack2Decharge = Time.time + fTimeDeltaAttack2DechargeModeStraight; break;
                        case Attack2Mode.spiral: fTimeNextAttack2Decharge = Time.time + fTimeDeltaAttack2DechargeModeSpiral; break;
                    }
                }
            }

            // ------------------------------------------------------------------------------------------------

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
            fRelativeMomentum *= 0.01f; // Adjustment to reduce damage done to player, still tweaking to get a good balance ...
            healthPlayer.Change(-(int)Math.Ceiling(collision.gameObject.GetComponent<AsteroidController>().iDamage * fRelativeMomentum / fRelativeMomentumBenchmark));
            StartCoroutine(FlashDamaged());
            if (healthPlayer.iHealth == 0)
            {
                gameManager.GameOver();
            }
            return;
        }
        if (collision.gameObject.CompareTag("PowerUpHealth"))
        {
            PowerUpController powerUpController = collision.gameObject.GetComponent<PowerUpController>();
            if (    (healthPlayer.iHealth < healthPlayer.iHealthMax)
                &&  (!powerUpController.bTriggeredDestroy) )
            {
                powerUpController.bTriggeredDestroy = true;
                healthPlayer.Change(powerUpController.iValue);
                Destroy(collision.gameObject);
                audioManager.sfxclpvolPowerUpHealth.PlayOneShot();
            }
            return;
        }
        if (collision.gameObject.CompareTag("PowerUpCharge"))
        {
            PowerUpController powerUpController = collision.gameObject.GetComponent<PowerUpController>();
            if (    (chargeAttack2.iCharge < chargeAttack2.iChargeMax)
                &&  (!powerUpController.bTriggeredDestroy) )
            {
                powerUpController.bTriggeredDestroy = true;
                chargeAttack2.Change(powerUpController.iValue);
                Destroy(collision.gameObject);
                audioManager.sfxclpvolPowerUpCharge.PlayOneShot();
            }
            return;
        }
    }

    // ------------------------------------------------------------------------------------------------

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.CompareTag("ProjectileEnemy"))
        {
            ProjectileController projectileController = collider.gameObject.GetComponent<ProjectileController>();
            if (!projectileController.bTriggeredDestroy)
            {
                projectileController.bTriggeredDestroy = true;
                healthPlayer.Change(-projectileController.iDamage);
                Destroy(collider.gameObject);
                if (    (healthPlayer.iHealth == 0)
                    &&  (gameManager.bInPlay) )
                {
                    gameManager.GameOver();
                }
                StartCoroutine(FlashDamaged());
            }
            return;
        }
    }

    // ------------------------------------------------------------------------------------------------

}
