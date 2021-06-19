using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float fMetresPerSecMove = 100f;
    public float fMetresPerSecMoveDeltaBoost = 20f;
    private float fInputHorz;
    private float fInputVert;
    private Vector3 v3DirectionMove;

    // ------------------------------------------------------------------------------------------------

    void Start()
    {

    }

    // ------------------------------------------------------------------------------------------------

    void FixedUpdate()
    {
        fInputHorz = Input.GetAxis("Horizontal");
        fInputVert = Input.GetAxis("Vertical");

        if (Math.Abs(fInputHorz) + Math.Abs(fInputVert) > 0f)
        {
            v3DirectionMove = ((fInputHorz * transform.right) + (fInputVert * transform.forward)).normalized;
            // transform.position = Vector3.MoveTowards(transform.position, transform.position + v3DirectionMove, fMetresPerSecMove * Time.deltaTime);
            transform.Translate(fMetresPerSecMove * Time.deltaTime * v3DirectionMove);
        }
    }

    // ------------------------------------------------------------------------------------------------

}
