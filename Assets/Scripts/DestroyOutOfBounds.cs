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
        if (!transform.CompareTag("Star"))
        {
            if (    (transform.position.x <= gameManager.v3ExistLimitLowerLeft.x)
                ||  (transform.position.x >= gameManager.v3ExistLimitUpperRight.x)
                ||  (transform.position.z <= gameManager.v3ExistLimitLowerLeft.z)
                ||  (transform.position.z >= gameManager.v3ExistLimitUpperRight.z) )
            {
                Destroy(gameObject);
            }
        }
        else
        {
            if (    (transform.position.x <= gameManager.v3ExistLimitLowerLeftStars.x)
                ||  (transform.position.x >= gameManager.v3ExistLimitUpperRightStars.x)
                ||  (transform.position.z <= gameManager.v3ExistLimitLowerLeftStars.z)
                ||  (transform.position.z >= gameManager.v3ExistLimitUpperRightStars.z) )
            {
                Destroy(gameObject);
            }
        }
    }

    // ------------------------------------------------------------------------------------------------

}
