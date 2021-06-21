using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyOutOfBounds : MonoBehaviour
{
    // Camera:
    private Camera camMainCamera;
    private float fPosYDeltaCam;
    private Vector3 v3CamLowerLeft;
    private Vector3 v3CamUpperRight;

    // Movement:
    private Vector3 v3ExistLimitLowerLeft;
    private Vector3 v3ExistLimitUpperRight;
    private Vector3 v3ExistLimitCamOffset = new Vector3(5f, 0f, 5f);

    // ------------------------------------------------------------------------------------------------

    void Start()
    {
        camMainCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
        fPosYDeltaCam = camMainCamera.transform.position.y - transform.position.y;
        v3CamLowerLeft = camMainCamera.ViewportToWorldPoint(new Vector3(0, 0, fPosYDeltaCam));
        v3CamUpperRight = camMainCamera.ViewportToWorldPoint(new Vector3(1, 1, fPosYDeltaCam));
        v3ExistLimitLowerLeft = v3CamLowerLeft - v3ExistLimitCamOffset;
        v3ExistLimitUpperRight = v3CamUpperRight + v3ExistLimitCamOffset;
    }

    // ------------------------------------------------------------------------------------------------

    void Update()
    {
        if (    (transform.position.x <= v3ExistLimitLowerLeft.x)
            ||  (transform.position.x >= v3ExistLimitUpperRight.x)
            ||  (transform.position.z <= v3ExistLimitLowerLeft.z)
            ||  (transform.position.z >= v3ExistLimitUpperRight.z) )
        {
            Destroy(gameObject);
        }
    }

    // ------------------------------------------------------------------------------------------------

}
