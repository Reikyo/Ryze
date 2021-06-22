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

    // Health:
    public bool bAlive;
    public int iHealth;
    public int iHealthMax = 100;
    public Slider sliHealth;

    // Attack:
    public GameObject goProjectile;
    public GameObject goGunLeftProjectileSpawnPoint;
    public GameObject goGunRightProjectileSpawnPoint;

    // ------------------------------------------------------------------------------------------------

    void Start()
    {
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();

        rbPlayer = GetComponent<Rigidbody>();
        v3MoveLimitLowerLeft = gameManager.v3CamLowerLeft + v3MoveLimitCamOffset;
        v3MoveLimitUpperRight = gameManager.v3CamUpperRight - v3MoveLimitCamOffset;

        iHealth = iHealthMax;
        bAlive = iHealth > 0;
        sliHealth.value = iHealth;
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
            rbPlayer.AddForce(20f * rbPlayer.mass * new Vector3(-rbPlayer.velocity.x, 0f, 0f));
        }
        if (    ((fInputVert < 0) && (transform.position.z <= v3MoveLimitLowerLeft.z))
            ||  ((fInputVert > 0) && (transform.position.z >= v3MoveLimitUpperRight.z)) )
        {
            // fInputVert *= -1f; // This doesn't work so well, gives bouncy behaviour at move limits
            fInputVert = 0f;
            rbPlayer.AddForce(20f * rbPlayer.mass * new Vector3(0f, 0f, -rbPlayer.velocity.z));
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

        if (Input.GetButtonDown("Fire1"))
        {
            Instantiate(goProjectile, goGunLeftProjectileSpawnPoint.transform.position, transform.rotation);
            Instantiate(goProjectile, goGunRightProjectileSpawnPoint.transform.position, transform.rotation);
        }
    }

    // ------------------------------------------------------------------------------------------------

    public void Attacked(int iDamage)
    {
        if (    (iHealth == 0)
            ||  (iDamage <= 0) )
        {
            return;
        }
        if (iHealth > iDamage)
        {
            iHealth -= iDamage;
            sliHealth.value = iHealth;
        }
        else
        {
            iHealth = 0;
            bAlive = false;
            sliHealth.transform.Find("Fill Area").gameObject.SetActive(false);
            gameObject.GetComponent<PlayerController>().enabled = false; // This line disables this script!
        }
    }

    // ------------------------------------------------------------------------------------------------

}
