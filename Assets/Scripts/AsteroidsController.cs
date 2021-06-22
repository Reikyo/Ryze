using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsteroidsController : MonoBehaviour
{
    private GameManager gameManager;

    public GameObject[] goArrAsteroids;
    private GameObject goAsteroidPrefab;
    private GameObject goAsteroid;
    private Rigidbody rbAsteroid;

    private float fTimeNextSpawn;

    // ------------------------------------------------------------------------------------------------

    void Start()
    {
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();

        fTimeNextSpawn = Time.time + UnityEngine.Random.Range(0f, 2f);
    }

    // ------------------------------------------------------------------------------------------------

    void Update()
    {
        if (Time.time >= fTimeNextSpawn)
        {
            goAsteroidPrefab = goArrAsteroids[UnityEngine.Random.Range(0, 5)];
            goAsteroid = Instantiate(
                goAsteroidPrefab,
                new Vector3(
                    UnityEngine.Random.Range(gameManager.v3CamLowerLeft.x + 10f, gameManager.v3CamUpperRight.x - 10f),
                    0f,
                    gameManager.v3CamUpperRight.z + 10f
                ),
                goAsteroidPrefab.transform.rotation
            );
            rbAsteroid = goAsteroid.GetComponent<Rigidbody>();
            rbAsteroid.AddForce(
                UnityEngine.Random.Range(-1e5f, +1e5f),
                0f,
                UnityEngine.Random.Range(-1e6f, -1e7f)
            );
            rbAsteroid.AddTorque(
                UnityEngine.Random.Range(-1e6f, +1e6f),
                UnityEngine.Random.Range(-1e6f, +1e6f),
                UnityEngine.Random.Range(-1e6f, +1e6f)
            );
            fTimeNextSpawn = Time.time + UnityEngine.Random.Range(0f, 2f);
        }
    }

    // ------------------------------------------------------------------------------------------------

}
