using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Camera:
    private Camera camMainCamera;
    public Vector3 v3CamLowerLeft;
    public Vector3 v3CamUpperRight;
    public Vector3 v3CamLowerLeftStars;
    public Vector3 v3CamUpperRightStars;

    // Movement:
    public Vector3 v3MoveLimitLowerLeft;
    public Vector3 v3MoveLimitUpperRight;
    private Vector3 v3MoveLimitCamOffset = new Vector3(5f, 0f, 5f);

    // Existence:
    public Vector3 v3ExistLimitLowerLeft;
    public Vector3 v3ExistLimitUpperRight;
    public Vector3 v3ExistLimitLowerLeftStars;
    public Vector3 v3ExistLimitUpperRightStars;
    private Vector3 v3ExistLimitCamOffset = new Vector3(20f, 0f, 20f);

    public float fPositionYSpawnStars = -500f;

    // ------------------------------------------------------------------------------------------------

    void Start()
    {
        camMainCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
        v3CamLowerLeft = camMainCamera.ViewportToWorldPoint(new Vector3(0, 0, camMainCamera.transform.position.y));
        v3CamUpperRight = camMainCamera.ViewportToWorldPoint(new Vector3(1, 1, camMainCamera.transform.position.y));
        v3CamLowerLeftStars = camMainCamera.ViewportToWorldPoint(new Vector3(0, 0, camMainCamera.transform.position.y - fPositionYSpawnStars));
        v3CamUpperRightStars = camMainCamera.ViewportToWorldPoint(new Vector3(1, 1, camMainCamera.transform.position.y - fPositionYSpawnStars));

        v3MoveLimitLowerLeft = v3CamLowerLeft + v3MoveLimitCamOffset;
        v3MoveLimitUpperRight = v3CamUpperRight - v3MoveLimitCamOffset;

        v3ExistLimitLowerLeft = v3CamLowerLeft - v3ExistLimitCamOffset;
        v3ExistLimitUpperRight = v3CamUpperRight + v3ExistLimitCamOffset;
        v3ExistLimitLowerLeftStars = v3CamLowerLeftStars - v3ExistLimitCamOffset;
        v3ExistLimitUpperRightStars = v3CamUpperRightStars + v3ExistLimitCamOffset;
    }

    // ------------------------------------------------------------------------------------------------

}
