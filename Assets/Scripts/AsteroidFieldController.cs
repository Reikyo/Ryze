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
    public float fTimeNextSpawnDeltaMin = 0f;
    public float fTimeNextSpawnDeltaMax = 2f;

    // ------------------------------------------------------------------------------------------------

    void Start()
    {
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();

        iNumAsteroids = goArrAsteroids.Length;

        fTimeNextSpawn = Time.time + UnityEngine.Random.Range(fTimeNextSpawnDeltaMin, fTimeNextSpawnDeltaMax);

        // Instantiate(goArrAsteroids[4], new Vector3(0f, 0f, 50f), goArrAsteroids[4].transform.rotation);
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