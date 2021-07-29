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
            Random.Range(-3.6e3f, -5e3f) // Lower value just matches 30ms^-1 background movement, upper value chosen by eye
        );
    }

    // ------------------------------------------------------------------------------------------------

}
