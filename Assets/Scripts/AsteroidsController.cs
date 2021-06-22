using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsteroidsController : MonoBehaviour
{
     public GameObject[] goArrAsteroids;

    // ------------------------------------------------------------------------------------------------

    void Start()
    {
        GameObject goAsteroid = goArrAsteroids[UnityEngine.Random.Range(0,4)];
        Instantiate(goAsteroid, new Vector3(0f, 0f, 50f), goAsteroid.transform.rotation);
    }

    // ------------------------------------------------------------------------------------------------

    void Update()
    {

    }

    // ------------------------------------------------------------------------------------------------

}
