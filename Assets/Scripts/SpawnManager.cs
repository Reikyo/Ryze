using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    private GameManager gameManager;

    public bool bSpawnStars = true;
    public bool bSpawnAsteroids = false;
    public bool bSpawnEnemies = false;

    public GameObject[] goArrStars;
    private GameObject goStar;
    private float fTimeNextSpawnStar;
    public float fTimeNextSpawnStarDeltaMin = 0f;
    public float fTimeNextSpawnStarDeltaMax = 5f;

    public GameObject[] goArrAsteroids;
    private GameObject goAsteroid;
    private float fTimeNextSpawnAsteroid;
    public float fTimeNextSpawnAsteroidDeltaMin = 0f;
    public float fTimeNextSpawnAsteroidDeltaMax = 2f;

    public GameObject goEnemy;
    public int iNumEnemy = 0;
    private float fTimeNextSpawnEnemy;

    public GameObject goPowerUpHealth;
    public GameObject goPowerUpCharge;

    // ------------------------------------------------------------------------------------------------

    void Start()
    {
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();

        // bSpawnStars = false;
        // bSpawnAsteroids = false;
        // bSpawnEnemies = false;

        fTimeNextSpawnStar = Time.time + UnityEngine.Random.Range(fTimeNextSpawnStarDeltaMin, fTimeNextSpawnStarDeltaMax);
        fTimeNextSpawnAsteroid = Time.time + UnityEngine.Random.Range(fTimeNextSpawnAsteroidDeltaMin, fTimeNextSpawnAsteroidDeltaMax);
        fTimeNextSpawnEnemy = Time.time + 3f;

        // Instantiate(goArrAsteroids[4], new Vector3(0f, 0f, 50f), goArrAsteroids[4].transform.rotation);
    }

    // ------------------------------------------------------------------------------------------------

    void Update()
    {
        if (    (bSpawnStars)
            &&  (Time.time >= fTimeNextSpawnStar) )
        {
            bSpawnStars = false;
            SpawnStars();
        }
        if (    (bSpawnAsteroids)
            &&  (Time.time >= fTimeNextSpawnAsteroid) )
        {
            bSpawnAsteroids = false;
            SpawnAsteroids();
        }
        if (    (bSpawnEnemies)
            &&  (iNumEnemy == 0)
            &&  (Time.time >= fTimeNextSpawnEnemy) )
        {
            bSpawnEnemies = false;
            SpawnEnemies();
        }
    }

    // ------------------------------------------------------------------------------------------------

    private void SpawnStars()
    {
        goStar = goArrStars[UnityEngine.Random.Range(0, goArrStars.Length)];
        Instantiate(
            goStar,
            new Vector3(
                UnityEngine.Random.Range(gameManager.v3CamLowerLeftStars.x + 10f, gameManager.v3CamUpperRightStars.x - 10f),
                gameManager.fPositionYSpawnStars,
                gameManager.v3CamUpperRightStars.z + 10f
            ),
            goStar.transform.rotation
        );
        fTimeNextSpawnStar = Time.time + UnityEngine.Random.Range(fTimeNextSpawnStarDeltaMin, fTimeNextSpawnStarDeltaMax);
        bSpawnStars = true;
    }

    // ------------------------------------------------------------------------------------------------

    private void SpawnAsteroids()
    {
        goAsteroid = goArrAsteroids[UnityEngine.Random.Range(0, goArrAsteroids.Length)];
        Instantiate(
            goAsteroid,
            new Vector3(
                UnityEngine.Random.Range(gameManager.v3CamLowerLeft.x + 10f, gameManager.v3CamUpperRight.x - 10f),
                0f,
                gameManager.v3CamUpperRight.z + 10f
            ),
            goAsteroid.transform.rotation
        );
        fTimeNextSpawnAsteroid = Time.time + UnityEngine.Random.Range(fTimeNextSpawnAsteroidDeltaMin, fTimeNextSpawnAsteroidDeltaMax);
        bSpawnAsteroids = true;
    }

    // ------------------------------------------------------------------------------------------------

    private void SpawnEnemies()
    {
        for (int i=0; i<3; i++)
        {
            Vector3 v3PositionSpawn = new Vector3(0f, 0f, 0f);
            Vector3 v3PositionConstant = new Vector3(0f, 0f, 0f);
            EnemyController.MoveMode moveMode = EnemyController.MoveMode.random;

            switch(i)
            {
                case 0: v3PositionSpawn = new Vector3(-80f, 0f, gameManager.v3CamUpperRight.z + 10f);
                        v3PositionConstant = new Vector3(-80f, 0f, 60f);
                        break;
                case 1: v3PositionSpawn = new Vector3(-40f, 0f, gameManager.v3CamUpperRight.z + 10f);
                        v3PositionConstant = new Vector3(-40f, 0f, 70f);
                        break;
                case 2: v3PositionSpawn = new Vector3(0f, 0f, gameManager.v3CamUpperRight.z + 10f);
                        v3PositionConstant = new Vector3(0f, 0f, 80f);
                        break;
                case 3: v3PositionSpawn = new Vector3(40f, 0f, gameManager.v3CamUpperRight.z + 10f);
                        v3PositionConstant = new Vector3(40f, 0f, 70f);
                        break;
                case 4: v3PositionSpawn = new Vector3(80f, 0f, gameManager.v3CamUpperRight.z + 10f);
                        v3PositionConstant = new Vector3(80f, 0f, 60f);
                        break;
            }

            GameObject goEnemyClone = Instantiate(
                goEnemy,
                v3PositionSpawn,
                goEnemy.transform.rotation
            );

            EnemyController enemyController = goEnemyClone.GetComponent<EnemyController>();
            enemyController.moveMode = moveMode;
            enemyController.v3PositionConstant = v3PositionConstant;

            iNumEnemy += 1;
        }
        fTimeNextSpawnEnemy = Time.time + 3f;
        bSpawnEnemies = true;
    }

    // ------------------------------------------------------------------------------------------------

    public void SpawnPowerUpHealth(Vector3 v3PositionSpawn)
    {
        Instantiate(
            goPowerUpHealth,
            v3PositionSpawn,
            goPowerUpHealth.transform.rotation
        );
    }

    // ------------------------------------------------------------------------------------------------

    public void SpawnPowerUpCharge(Vector3 v3PositionSpawn)
    {
        Instantiate(
            goPowerUpCharge,
            v3PositionSpawn,
            goPowerUpCharge.transform.rotation
        );
    }

    // ------------------------------------------------------------------------------------------------

    public void DestroyStars()
    {
        foreach (GameObject goStarClone in GameObject.FindGameObjectsWithTag("Star"))
        {
            Destroy(goStarClone);
        }
    }

    // ------------------------------------------------------------------------------------------------

    public void DestroyAsteroids()
    {
        foreach (GameObject goAsteroidClone in GameObject.FindGameObjectsWithTag("Asteroid"))
        {
            Destroy(goAsteroidClone);
        }
    }

    // ------------------------------------------------------------------------------------------------

    public void DestroyEnemies()
    {
        foreach (GameObject goEnemyClone in GameObject.FindGameObjectsWithTag("Enemy"))
        {
            Destroy(goEnemyClone);
        }
        iNumEnemy = 0;
    }

    // ------------------------------------------------------------------------------------------------

}
