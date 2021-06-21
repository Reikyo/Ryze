using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    // Camera:
    private Camera camMainCamera;
    private float fPosYDeltaCam;
    private Vector3 v3CamLowerLeft;
    private Vector3 v3CamUpperRight;

    // Movement:
    public float fMetresPerSecMove = 100f;
    public float fMetresPerSecMoveDeltaBoost = 20f;
    private float fInputHorz;
    private float fInputVert;
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
        camMainCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
        fPosYDeltaCam = camMainCamera.transform.position.y - transform.position.y;
        v3CamLowerLeft = camMainCamera.ViewportToWorldPoint(new Vector3(0, 0, fPosYDeltaCam));
        v3CamUpperRight = camMainCamera.ViewportToWorldPoint(new Vector3(1, 1, fPosYDeltaCam));

        v3MoveLimitLowerLeft = v3CamLowerLeft + v3MoveLimitCamOffset;
        v3MoveLimitUpperRight = v3CamUpperRight - v3MoveLimitCamOffset;

        iHealth = iHealthMax;
        bAlive = iHealth > 0;
        sliHealth.value = iHealth;
    }

    // ------------------------------------------------------------------------------------------------

    void Update()
    {
        fInputHorz = Input.GetAxis("Horizontal");
        fInputVert = Input.GetAxis("Vertical");

        if (    ((fInputHorz < 0) && (transform.position.x <= v3MoveLimitLowerLeft.x))
            ||  ((fInputHorz > 0) && (transform.position.x >= v3MoveLimitUpperRight.x)) )
        {
            fInputHorz = 0f;
        }
        if (    ((fInputVert < 0) && (transform.position.z <= v3MoveLimitLowerLeft.z))
            ||  ((fInputVert > 0) && (transform.position.z >= v3MoveLimitUpperRight.z)) )
        {
            fInputVert = 0f;
        }
        if (Math.Abs(fInputHorz) + Math.Abs(fInputVert) > 0f)
        {
            v3DirectionMove = ((fInputHorz * transform.right) + (fInputVert * transform.forward)).normalized;
            // transform.position = Vector3.MoveTowards(transform.position, transform.position + v3DirectionMove, fMetresPerSecMove * Time.deltaTime);
            transform.Translate(fMetresPerSecMove * Time.deltaTime * v3DirectionMove);
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
