using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

}
