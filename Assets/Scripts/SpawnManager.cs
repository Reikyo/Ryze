using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    private GameManager gameManager;
    private AudioManager audioManager;

    public bool bSpawnStars = true;
    public bool bSpawnAsteroids = false;
    public bool bSpawnEnemies = false;

    // Stars:
    public GameObject[] goArrStars;
    private GameObject goStar;
    private float fTimeNextSpawnStar;
    public float fTimeDeltaMinSpawnStar = 0f;
    public float fTimeDeltaMaxSpawnStar = 5f;
    public float fPositionYSpawnStars = -500f;

    // Asteroids:
    public GameObject[] goArrAsteroids;
    private GameObject goAsteroid;
    private float fTimeNextSpawnAsteroid;
    public float fTimeDeltaMinSpawnAsteroid = 0f;
    public float fTimeDeltaMaxSpawnAsteroid = 2f;

    // Enemies:
    public GameObject goEnemy;
    public int iNumEnemy = 0;
    private int iNumEnemyPerWave = 2;
    private float fTimeNextSpawnEnemy;
    private float fTimeDeltaSpawnEnemy = 3f;

    // PowerUps:
    public GameObject goPowerUpHealth;
    public GameObject goPowerUpCharge;

    // VFX:
    public GameObject vfxclpExplosionAsteroid;
    public GameObject vfxclpExplosionEnemy;
    public GameObject vfxclpSparksAsteroid;
    public GameObject vfxclpSparksVehicle;
    private float fTimeDelta_vfxclpExplosionAsteroid;
    private float fTimeDelta_vfxclpExplosionEnemy;
    private float fTimeDelta_vfxclpSparksAsteroid;
    private float fTimeDelta_vfxclpSparksVehicle;

    // ------------------------------------------------------------------------------------------------

    void Start()
    {
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
        audioManager = GameObject.Find("Audio Manager").GetComponent<AudioManager>();

        fTimeNextSpawnStar = Time.time + Random.Range(fTimeDeltaMinSpawnStar, fTimeDeltaMaxSpawnStar);
        fTimeNextSpawnAsteroid = Time.time + Random.Range(fTimeDeltaMinSpawnAsteroid, fTimeDeltaMaxSpawnAsteroid);
        fTimeNextSpawnEnemy = Time.time + fTimeDeltaSpawnEnemy;

        fTimeDelta_vfxclpExplosionAsteroid = vfxclpExplosionAsteroid.GetComponent<ParticleSystem>().main.duration;
        fTimeDelta_vfxclpExplosionEnemy = vfxclpExplosionAsteroid.GetComponent<ParticleSystem>().main.duration;
        fTimeDelta_vfxclpSparksAsteroid = vfxclpSparksAsteroid.GetComponent<ParticleSystem>().main.duration;
        fTimeDelta_vfxclpSparksVehicle = vfxclpSparksVehicle.GetComponent<ParticleSystem>().main.duration;
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
        goStar = goArrStars[Random.Range(0, goArrStars.Length)];
        Instantiate(
            goStar,
            new Vector3(
                Random.Range(gameManager.v3CamLowerLeftStars.x + 10f, gameManager.v3CamUpperRightStars.x - 10f),
                fPositionYSpawnStars,
                gameManager.v3CamUpperRightStars.z + 10f
            ),
            goStar.transform.rotation
        );
        fTimeNextSpawnStar = Time.time + Random.Range(fTimeDeltaMinSpawnStar, fTimeDeltaMaxSpawnStar);
        bSpawnStars = true;
    }

    // ------------------------------------------------------------------------------------------------

    private void SpawnAsteroids()
    {
        goAsteroid = goArrAsteroids[Random.Range(0, goArrAsteroids.Length)];
        Instantiate(
            goAsteroid,
            new Vector3(
                Random.Range(gameManager.v3CamLowerLeft.x + 10f, gameManager.v3CamUpperRight.x - 10f),
                0f,
                gameManager.v3CamUpperRight.z + 10f
            ),
            goAsteroid.transform.rotation
        );
        fTimeNextSpawnAsteroid = Time.time + Random.Range(fTimeDeltaMinSpawnAsteroid, fTimeDeltaMaxSpawnAsteroid);
        bSpawnAsteroids = true;
    }

    // ------------------------------------------------------------------------------------------------

    private void SpawnEnemies()
    {
        for (int i=0; i<iNumEnemyPerWave; i++)
        {
            Vector3 v3PositionSpawn = new Vector3(0f, 0f, 0f);

            switch(i)
            {
                case 0: v3PositionSpawn = new Vector3(-80f, 0f, gameManager.v3CamUpperRight.z + 10f);
                        break;
                case 1: v3PositionSpawn = new Vector3(80f, 0f, gameManager.v3CamUpperRight.z + 10f);
                        break;
                // case 2: v3PositionSpawn = new Vector3(0f, 0f, gameManager.v3CamUpperRight.z + 10f);
                //         break;
                // case 3: v3PositionSpawn = new Vector3(-40f, 0f, gameManager.v3CamUpperRight.z + 10f);
                //         break;
                // case 4: v3PositionSpawn = new Vector3(40f, 0f, gameManager.v3CamUpperRight.z + 10f);
                //         break;
            }

            GameObject goEnemyClone = Instantiate(
                goEnemy,
                v3PositionSpawn,
                Quaternion.LookRotation(new Vector3(0f, 0f, -1f))
            );
            EnemyController enemyController = goEnemyClone.GetComponent<EnemyController>();

            if (i == 0)
            {
                enemyController.v3PositionConstant = new Vector3(-80f, 0f, 60f);
                enemyController.ListPositionPattern = new List<(float[], float)>(){
                    (new float[]{-80f, 0f,+80f}, 1f),
                    (new float[]{}, 1f),
                    (new float[]{}, 1f),
                    (new float[]{-80f, 0f,  0f, 20f}, 1f),
                    (new float[]{}, 1f),
                    (new float[]{-80f, 0f,+80f, 20f}, 1f),
                    (new float[]{}, 1f),
                };
                enemyController.ListRotationPattern = new List<(float[], float)>(){
                    (new float[]{180f}, 1f),
                    (new float[]{}, 1f),
                    (new float[]{110f, 20f}, 1f),
                    (new float[]{}, 1f),
                    (new float[]{70f, 20f}, 1f),
                    (new float[]{}, 1f),
                    (new float[]{180f, 20f}, 1f),
                };
                enemyController.ListAttackPattern = new List<(float[], float[])>(){
                    (new float[]{}, new float[]{}),
                    (new float[]{5f}, new float[]{0f}),
                    (new float[]{1f}, new float[]{0f}),
                    (new float[]{1f}, new float[]{0f}),
                    (new float[]{1f}, new float[]{0f}),
                    (new float[]{1f}, new float[]{0f}),
                    (new float[]{6f}, new float[]{1f}),
                };
            }
            else if (i == 1)
            {
                enemyController.v3PositionConstant = new Vector3(80f, 0f, 60f);
                enemyController.ListPositionPattern = new List<(float[], float)>(){
                    (new float[]{+80f, 0f,+80f}, 1f),
                    (new float[]{}, 1f),
                    (new float[]{}, 1f),
                    (new float[]{+80f, 0f,  0f, 20f}, 1f),
                    (new float[]{}, 1f),
                    (new float[]{+80f, 0f,+80f, 20f}, 1f),
                    (new float[]{}, 1f),
                };
                enemyController.ListRotationPattern = new List<(float[], float)>(){
                    (new float[]{180f}, 1f),
                    (new float[]{}, 1f),
                    (new float[]{250f, 20f}, 1f),
                    (new float[]{}, 1f),
                    (new float[]{290f, 20f}, 1f),
                    (new float[]{}, 1f),
                    (new float[]{180f, 20f}, 1f),
                };
                enemyController.ListAttackPattern = new List<(float[], float[])>(){
                    (new float[]{}, new float[]{}),
                    (new float[]{5f}, new float[]{0f}),
                    (new float[]{1f}, new float[]{0f}),
                    (new float[]{1f}, new float[]{0f}),
                    (new float[]{1f}, new float[]{0f}),
                    (new float[]{1f}, new float[]{0f}),
                    (new float[]{6f}, new float[]{1f}),
                };
            }
            // else if (i == 2)
            // {
            //     enemyController.v3PositionConstant = new Vector3(0f, 0f, 80f);
            // }
            // else if (i == 3)
            // {
            //     enemyController.v3PositionConstant = new Vector3(-40f, 0f, 70f);
            // }
            // else if (i == 4)
            // {
            //     enemyController.v3PositionConstant = new Vector3(40f, 0f, 70f);
            // }

            enemyController.moveMode = EnemyController.MoveMode.pattern;
            enemyController.lookMode = EnemyController.LookMode.pattern;
            enemyController.attackMode1 = EnemyController.AttackMode1.laser;
            enemyController.attackMode2 = EnemyController.AttackMode2.pattern;

            iNumEnemy += 1;
        }
        fTimeNextSpawnEnemy = Time.time + fTimeDeltaSpawnEnemy;
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

    public void SpawnExplosionAsteroid(Vector3 v3PositionSpawn)
    {
        GameObject vfxclpExplosionAsteroidClone = Instantiate(
            vfxclpExplosionAsteroid,
            v3PositionSpawn,
            vfxclpExplosionAsteroid.transform.rotation
        );
        Destroy(vfxclpExplosionAsteroidClone, fTimeDelta_vfxclpExplosionAsteroid);
        audioManager.sfxclpvolExplosionAsteroid.PlayOneShot();
    }

    // ------------------------------------------------------------------------------------------------

    public void SpawnExplosionEnemy(Vector3 v3PositionSpawn)
    {
        GameObject vfxclpExplosionEnemyClone = Instantiate(
            vfxclpExplosionEnemy,
            v3PositionSpawn,
            vfxclpExplosionEnemy.transform.rotation
        );
        Destroy(vfxclpExplosionEnemyClone, fTimeDelta_vfxclpExplosionEnemy);
        audioManager.sfxclpvolExplosionEnemy.PlayOneShot();
    }

    // ------------------------------------------------------------------------------------------------

    public void SpawnSparksAsteroid(Vector3 v3PositionSpawn, Quaternion quatRotationSpawn, Transform trnParent)
    {
        GameObject vfxclpSparksAsteroidClone = Instantiate(
            vfxclpSparksAsteroid,
            v3PositionSpawn,
            quatRotationSpawn,
            trnParent
        );
        Destroy(vfxclpSparksAsteroidClone, fTimeDelta_vfxclpSparksAsteroid);
    }

    // ------------------------------------------------------------------------------------------------

    public void SpawnSparksVehicle(Vector3 v3PositionSpawn, Quaternion quatRotationSpawn, Transform trnParent)
    {
        GameObject vfxclpSparksVehicleClone = Instantiate(
            vfxclpSparksVehicle,
            v3PositionSpawn,
            quatRotationSpawn,
            trnParent
        );
        Destroy(vfxclpSparksVehicleClone, fTimeDelta_vfxclpSparksVehicle);
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

    public void DestroyPowerUps()
    {
        foreach (GameObject goPowerUpHealthClone in GameObject.FindGameObjectsWithTag("PowerUpHealth"))
        {
            Destroy(goPowerUpHealthClone);
        }
        foreach (GameObject goPowerUpChargeClone in GameObject.FindGameObjectsWithTag("PowerUpCharge"))
        {
            Destroy(goPowerUpChargeClone);
        }
    }

    // ------------------------------------------------------------------------------------------------

    public void DestroyProjectiles()
    {
        foreach (GameObject goProjectilePlayerClone in GameObject.FindGameObjectsWithTag("ProjectilePlayer"))
        {
            Destroy(goProjectilePlayerClone);
        }
        foreach (GameObject goProjectileEnemyClone in GameObject.FindGameObjectsWithTag("ProjectileEnemy"))
        {
            Destroy(goProjectileEnemyClone);
        }
    }

    // ------------------------------------------------------------------------------------------------

}
