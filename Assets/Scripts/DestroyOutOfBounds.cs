using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyOutOfBounds : MonoBehaviour
{
    private GameManager gameManager;

    // Movement:
    private Vector3 v3ExistLimitLowerLeft;
    private Vector3 v3ExistLimitUpperRight;
    private Vector3 v3ExistLimitCamOffset = new Vector3(20f, 0f, 20f);

    // ------------------------------------------------------------------------------------------------

    void Start()
    {
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();

        v3ExistLimitLowerLeft = gameManager.v3CamLowerLeft - v3ExistLimitCamOffset;
        v3ExistLimitUpperRight = gameManager.v3CamUpperRight + v3ExistLimitCamOffset;
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
