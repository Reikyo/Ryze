using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarController : MonoBehaviour
{
    // Movement:
    private Rigidbody rbStar;

    // ------------------------------------------------------------------------------------------------

    void Start()
    {
        rbStar = GetComponent<Rigidbody>();
        rbStar.AddForce(
            0f,
            0f,
            UnityEngine.Random.Range(-2e6f, -1e7f)
        );
    }

    // ------------------------------------------------------------------------------------------------

}
