using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreditsController : MonoBehaviour
{
    private GameManager gameManager;
    private Camera camMainCamera;

    private float fMetresPositionYStart = -400f;
    private float fMetresPositionYFinish = 1100f;
    private float fMetresPerSecReference = 50f;
    private float fMetresPerSec;
    private int iDirection;

    private int iPixelWidthMainCameraReference = 1000;
    private int iPixelWidthMainCameraLastFrame;

    // ------------------------------------------------------------------------------------------------

    void OnEnable()
    {
        transform.localPosition = new Vector3(0f, fMetresPositionYStart, 0f);
    }

    // ------------------------------------------------------------------------------------------------

    void Start()
    {
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
        camMainCamera = GameObject.Find("Main Camera").GetComponent<Camera>();

        if (fMetresPositionYFinish > fMetresPositionYStart)
        {
            iDirection = 1;
        }
        else if (fMetresPositionYFinish < fMetresPositionYStart)
        {
            iDirection = -1;
        }
        else
        {
            iDirection = 0;
        }

        SetSpeed();
    }

    // ------------------------------------------------------------------------------------------------

    void Update()
    {
        if (iPixelWidthMainCameraLastFrame != camMainCamera.pixelWidth)
        {
            SetSpeed();
        }

        if (    ((iDirection == 1) && (transform.localPosition.y < fMetresPositionYFinish))
            ||  ((iDirection == -1) && (transform.localPosition.y > fMetresPositionYFinish)) )
        {
            transform.Translate(iDirection * fMetresPerSec * Time.deltaTime * Vector3.up);
            return;
        }

        gameManager.StopCredits();
    }

    // ------------------------------------------------------------------------------------------------

    private void SetSpeed()
    {
        fMetresPerSec = fMetresPerSecReference * ((float)camMainCamera.pixelWidth / (float)iPixelWidthMainCameraReference);
        iPixelWidthMainCameraLastFrame = camMainCamera.pixelWidth;
    }

    // ------------------------------------------------------------------------------------------------

}
