using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsteroidFieldController : MonoBehaviour
{
    private GameManager gameManager;

    public GameObject[] goArrAsteroids;
    private GameObject goAsteroid;
    private int iNumAsteroids;

    private float fTimeNextSpawn;
    private float fTimeNextSpawnDeltaMin = 0f;
    private float fTimeNextSpawnDeltaMax = 2f;

    // ------------------------------------------------------------------------------------------------

    void Start()
    {
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();

        iNumAsteroids = goArrAsteroids.Length;

        fTimeNextSpawn = Time.time + UnityEngine.Random.Range(fTimeNextSpawnDeltaMin, fTimeNextSpawnDeltaMax);
    }

    // ------------------------------------------------------------------------------------------------

    void Update()
    {
        if (Time.time >= fTimeNextSpawn)
        {
            goAsteroid = goArrAsteroids[UnityEngine.Random.Range(0, iNumAsteroids)];
            Instantiate(
                goAsteroid,
                new Vector3(
                    UnityEngine.Random.Range(gameManager.v3CamLowerLeft.x + 10f, gameManager.v3CamUpperRight.x - 10f),
                    0f,
                    gameManager.v3CamUpperRight.z + 10f
                ),
                goAsteroid.transform.rotation
            );
            fTimeNextSpawn = Time.time + UnityEngine.Random.Range(fTimeNextSpawnDeltaMin, fTimeNextSpawnDeltaMax);
        }
    }

    // ------------------------------------------------------------------------------------------------

}
