using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    private GameManager gameManager;
    private LevelManager levelManager;
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
    public GameObject goEnemy1;
    public GameObject goEnemy2;
    public int iNumEnemy = 0;
    // public int iNumEnemyPerWave = 1;
    private float fTimeNextSpawnEnemy;
    private float fTimeDeltaSpawnEnemy = 3f;
    public List<int> ListiEnemy = new List<int>();
    public List<Vector3> Listv3PositionSpawnEnemy = new List<Vector3>();
    public List<Vector3> Listv3PositionConstantEnemy = new List<Vector3>();
    public List<EnemyController.MoveMode> ListmoveModeEnemy = new List<EnemyController.MoveMode>();
    public List<EnemyController.LookMode> ListlookModeEnemy = new List<EnemyController.LookMode>();
    public List<EnemyController.AttackMode1> ListattackMode1Enemy = new List<EnemyController.AttackMode1>();
    public List<EnemyController.AttackMode2> ListattackMode2Enemy = new List<EnemyController.AttackMode2>();
    public List<List<(float[], float)>> ListPositionPatternEnemy = new List<List<(float[], float)>>();
    public List<List<(float[], float)>> ListRotationPatternEnemy = new List<List<(float[], float)>>();
    public List<List<(float[], float[])>> ListAttackPatternEnemy = new List<List<(float[], float[])>>();

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
        levelManager = GameObject.Find("Level Manager").GetComponent<LevelManager>();
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
            &&  (!levelManager.bPreparedLevel)
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
        for (int i=0; i<ListiEnemy.Count; i++)
        {
            if (ListiEnemy[i] == 1)
            {
                goEnemy = goEnemy1;
            }
            else if (ListiEnemy[i] == 2)
            {
                goEnemy = goEnemy2;
            }

            GameObject goEnemyClone = Instantiate(
                goEnemy,
                Listv3PositionSpawnEnemy[i],
                Quaternion.LookRotation(new Vector3(0f, 0f, -1f))
            );
            iNumEnemy += 1;

            EnemyController enemyController = goEnemyClone.GetComponent<EnemyController>();

            if (Listv3PositionConstantEnemy.Count > 0)
            {
                enemyController.v3PositionConstant = Listv3PositionConstantEnemy[i];
            }
            if (ListmoveModeEnemy.Count > 0)
            {
                enemyController.moveMode = ListmoveModeEnemy[i];
            }
            if (ListlookModeEnemy.Count > 0)
            {
                enemyController.lookMode = ListlookModeEnemy[i];
            }
            if (ListattackMode1Enemy.Count > 0)
            {
                enemyController.attackMode1 = ListattackMode1Enemy[i];
            }
            if (ListattackMode2Enemy.Count > 0)
            {
                enemyController.attackMode2 = ListattackMode2Enemy[i];
            }
            if (ListPositionPatternEnemy.Count > 0)
            {
                enemyController.ListPositionPattern = ListPositionPatternEnemy[i];
            }
            if (ListRotationPatternEnemy.Count > 0)
            {
                enemyController.ListRotationPattern = ListRotationPatternEnemy[i];
            }
            if (ListAttackPatternEnemy.Count > 0)
            {
                enemyController.ListAttackPattern = ListAttackPatternEnemy[i];
            }

            if (i == ListiEnemy.Count-1)
            {
                levelManager.bPreparedLevel = true;
            }
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

    public void ClearEnemyLists()
    {
        ListiEnemy.Clear();
        Listv3PositionSpawnEnemy.Clear();
        Listv3PositionConstantEnemy.Clear();
        ListmoveModeEnemy.Clear();
        ListlookModeEnemy.Clear();
        ListattackMode1Enemy.Clear();
        ListattackMode2Enemy.Clear();
    }

    // ------------------------------------------------------------------------------------------------
}
