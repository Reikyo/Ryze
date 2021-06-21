using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileController : MonoBehaviour
{
    public float fMetresPerSecMove = 200f;

    // ------------------------------------------------------------------------------------------------

    void Update()
    {
        transform.Translate(fMetresPerSecMove * Time.deltaTime * Vector3.forward);
    }

    // ------------------------------------------------------------------------------------------------

}
