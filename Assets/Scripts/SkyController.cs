using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkyController : MonoBehaviour
{
    // Movement:
    public float fMetresPerSecMove = 30f;

    // ------------------------------------------------------------------------------------------------

    void Update()
    {
        transform.Translate(fMetresPerSecMove * Time.deltaTime * -Vector3.forward);

        if (transform.position.z <= -4000f)
        {
            transform.Translate(8000f * Vector3.forward);
        }
    }

    // ------------------------------------------------------------------------------------------------

}
