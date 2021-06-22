using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Camera:
    private Camera camMainCamera;
    public Vector3 v3CamLowerLeft;
    public Vector3 v3CamUpperRight;

    // ------------------------------------------------------------------------------------------------

    void Start()
    {
        camMainCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
        v3CamLowerLeft = camMainCamera.ViewportToWorldPoint(new Vector3(0, 0, camMainCamera.transform.position.y));
        v3CamUpperRight = camMainCamera.ViewportToWorldPoint(new Vector3(1, 1, camMainCamera.transform.position.y));
    }

    // ------------------------------------------------------------------------------------------------

    void Update()
    {

    }

    // ------------------------------------------------------------------------------------------------

}
