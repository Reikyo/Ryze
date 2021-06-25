using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyOutOfBounds : MonoBehaviour
{
    private GameManager gameManager;

    // ------------------------------------------------------------------------------------------------

    void Start()
    {
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
    }

    // ------------------------------------------------------------------------------------------------

    void Update()
    {
        if (    (transform.position.x <= gameManager.v3ExistLimitLowerLeft.x)
            ||  (transform.position.x >= gameManager.v3ExistLimitUpperRight.x)
            ||  (transform.position.z <= gameManager.v3ExistLimitLowerLeft.z)
            ||  (transform.position.z >= gameManager.v3ExistLimitUpperRight.z) )
        {
            Destroy(gameObject);
        }
    }

    // ------------------------------------------------------------------------------------------------

}
