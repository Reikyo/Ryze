using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    private GameManager gameManager;
    private SpawnManager spawnManager;

    public int iLevel = 1;
    private bool bInitiatedLevel = false;
    public bool bPreparedLevel = false;
    private float fTimeDeltaWaitStartLevel1 = 5f;
    private float fTimeDeltaWaitStartLevel2 = 2f;
    private float fTimeDeltaLevel1 = 20f;

    // ------------------------------------------------------------------------------------------------

    void Start()
    {
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
        spawnManager = GameObject.Find("Spawn Manager").GetComponent<SpawnManager>();
    }

    // ------------------------------------------------------------------------------------------------

    void Update()
    {
        if (!gameManager.bInPlay)
        {
            return;
        }
        if (iLevel == 1)
        {
            if (Time.time < gameManager.fTimeStartLevel + fTimeDeltaWaitStartLevel1)
            {
                return;
            }
            if (!bInitiatedLevel)
            {
                spawnManager.bSpawnAsteroids = true;
                bInitiatedLevel = true;
                return;
            }
            if (Time.time >= gameManager.fTimeStartLevel + fTimeDeltaWaitStartLevel1 + fTimeDeltaLevel1)
            {
                FinishLevel();
                return;
            }
        }
        if (iLevel == 2)
        {
            if (Time.time < gameManager.fTimeStartLevel + fTimeDeltaWaitStartLevel2)
            {
                return;
            }
            if (!bInitiatedLevel)
            {
                spawnManager.ListiEnemy = new List<int>{
                    1
                };
                spawnManager.Listv3PositionSpawnEnemy = new List<Vector3>{
                    new Vector3(0f, 0f, gameManager.v3CamUpperRight.z + 10f)
                };
                spawnManager.ListmoveModeEnemy = new List<EnemyController.MoveMode>{
                    EnemyController.MoveMode.player
                };
                spawnManager.ListlookModeEnemy = new List<EnemyController.LookMode>{
                    EnemyController.LookMode.player
                };
                spawnManager.ListattackMode1Enemy = new List<EnemyController.AttackMode1>{
                    EnemyController.AttackMode1.projectile
                };
                spawnManager.ListattackMode2Enemy = new List<EnemyController.AttackMode2>{
                    EnemyController.AttackMode2.burst
                };
                spawnManager.bSpawnEnemies = true;
                bInitiatedLevel = true;
                return;
            }
            CheckFinishEnemyLevel();
        }
        if (iLevel == 3)
        {
            if (Time.time < gameManager.fTimeStartLevel + fTimeDeltaWaitStartLevel2)
            {
                return;
            }
            if (!bInitiatedLevel)
            {
                spawnManager.ListiEnemy = new List<int>{
                    1, 1, 1
                };
                spawnManager.Listv3PositionSpawnEnemy = new List<Vector3>{
                    new Vector3(-80f, 0f, gameManager.v3CamUpperRight.z + 10f),
                    new Vector3(  0f, 0f, gameManager.v3CamUpperRight.z + 10f),
                    new Vector3( 80f, 0f, gameManager.v3CamUpperRight.z + 10f)
                };
                spawnManager.ListmoveModeEnemy = new List<EnemyController.MoveMode>{
                    EnemyController.MoveMode.player,
                    EnemyController.MoveMode.player,
                    EnemyController.MoveMode.player
                };
                spawnManager.ListlookModeEnemy = new List<EnemyController.LookMode>{
                    EnemyController.LookMode.player,
                    EnemyController.LookMode.player,
                    EnemyController.LookMode.player
                };
                spawnManager.ListattackMode1Enemy = new List<EnemyController.AttackMode1>{
                    EnemyController.AttackMode1.projectile,
                    EnemyController.AttackMode1.projectile,
                    EnemyController.AttackMode1.projectile
                };
                spawnManager.ListattackMode2Enemy = new List<EnemyController.AttackMode2>{
                    EnemyController.AttackMode2.burst,
                    EnemyController.AttackMode2.burst,
                    EnemyController.AttackMode2.burst
                };
                spawnManager.bSpawnEnemies = true;
                bInitiatedLevel = true;
                return;
            }
            CheckFinishEnemyLevel();
        }
        if (iLevel == 4)
        {
            if (Time.time < gameManager.fTimeStartLevel + fTimeDeltaWaitStartLevel2)
            {
                return;
            }
            if (!bInitiatedLevel)
            {
                spawnManager.ListiEnemy = new List<int>{
                    1, 1, 1, 1, 1
                };
                spawnManager.Listv3PositionSpawnEnemy = new List<Vector3>{
                    new Vector3(-80f, 0f, gameManager.v3CamUpperRight.z + 10f),
                    new Vector3(-40f, 0f, gameManager.v3CamUpperRight.z + 10f),
                    new Vector3(  0f, 0f, gameManager.v3CamUpperRight.z + 10f),
                    new Vector3( 40f, 0f, gameManager.v3CamUpperRight.z + 10f),
                    new Vector3( 80f, 0f, gameManager.v3CamUpperRight.z + 10f)
                };
                spawnManager.Listv3PositionConstantEnemy = new List<Vector3>{
                    new Vector3(-80f, 0f, +60f),
                    new Vector3(-40f, 0f, +70f),
                    new Vector3(  0f, 0f, +80f),
                    new Vector3( 40f, 0f, +70f),
                    new Vector3( 80f, 0f, +60f)
                };
                spawnManager.ListmoveModeEnemy = new List<EnemyController.MoveMode>{
                    EnemyController.MoveMode.constanthover,
                    EnemyController.MoveMode.constanthover,
                    EnemyController.MoveMode.constanthover,
                    EnemyController.MoveMode.constanthover,
                    EnemyController.MoveMode.constanthover
                };
                spawnManager.ListlookModeEnemy = new List<EnemyController.LookMode>{
                    EnemyController.LookMode.player,
                    EnemyController.LookMode.player,
                    EnemyController.LookMode.player,
                    EnemyController.LookMode.player,
                    EnemyController.LookMode.player
                };
                spawnManager.ListattackMode1Enemy = new List<EnemyController.AttackMode1>{
                    EnemyController.AttackMode1.projectile,
                    EnemyController.AttackMode1.projectile,
                    EnemyController.AttackMode1.projectile,
                    EnemyController.AttackMode1.projectile,
                    EnemyController.AttackMode1.projectile
                };
                spawnManager.ListattackMode2Enemy = new List<EnemyController.AttackMode2>{
                    EnemyController.AttackMode2.constant,
                    EnemyController.AttackMode2.constant,
                    EnemyController.AttackMode2.constant,
                    EnemyController.AttackMode2.constant,
                    EnemyController.AttackMode2.constant
                };
                spawnManager.bSpawnEnemies = true;
                bInitiatedLevel = true;
                return;
            }
            CheckFinishEnemyLevel();
        }
        if (iLevel == 5)
        {
            if (Time.time < gameManager.fTimeStartLevel + fTimeDeltaWaitStartLevel2)
            {
                return;
            }
            if (!bInitiatedLevel)
            {
                spawnManager.ListiEnemy = new List<int>{
                    2
                };
                spawnManager.Listv3PositionSpawnEnemy = new List<Vector3>{
                    new Vector3(0f, 0f, gameManager.v3CamUpperRight.z + 10f)
                };
                spawnManager.ListmoveModeEnemy = new List<EnemyController.MoveMode>{
                    EnemyController.MoveMode.player
                };
                spawnManager.ListlookModeEnemy = new List<EnemyController.LookMode>{
                    EnemyController.LookMode.player
                };
                spawnManager.ListattackMode1Enemy = new List<EnemyController.AttackMode1>{
                    EnemyController.AttackMode1.laser
                };
                spawnManager.ListattackMode2Enemy = new List<EnemyController.AttackMode2>{
                    EnemyController.AttackMode2.burst
                };
                spawnManager.bSpawnEnemies = true;
                bInitiatedLevel = true;
                return;
            }
            CheckFinishEnemyLevel();
        }
        if (iLevel == 6)
        {
            if (Time.time < gameManager.fTimeStartLevel + fTimeDeltaWaitStartLevel2)
            {
                return;
            }
            if (!bInitiatedLevel)
            {
                spawnManager.ListiEnemy = new List<int>{
                    2, 2
                };
                spawnManager.Listv3PositionSpawnEnemy = new List<Vector3>{
                    new Vector3(-80f, 0f, gameManager.v3CamUpperRight.z + 10f),
                    new Vector3( 80f, 0f, gameManager.v3CamUpperRight.z + 10f)
                };
                spawnManager.ListmoveModeEnemy = new List<EnemyController.MoveMode>{
                    EnemyController.MoveMode.pattern,
                    EnemyController.MoveMode.pattern
                };
                spawnManager.ListlookModeEnemy = new List<EnemyController.LookMode>{
                    EnemyController.LookMode.pattern,
                    EnemyController.LookMode.pattern
                };
                spawnManager.ListattackMode1Enemy = new List<EnemyController.AttackMode1>{
                    EnemyController.AttackMode1.laser,
                    EnemyController.AttackMode1.laser
                };
                spawnManager.ListattackMode2Enemy = new List<EnemyController.AttackMode2>{
                    EnemyController.AttackMode2.pattern,
                    EnemyController.AttackMode2.pattern
                };
                spawnManager.ListPositionPatternEnemy = new List<List<(float[], float)>>{
                    new List<(float[], float)>(){
                        (new float[]{-80f, 0f,+80f}, 1f),
                        (new float[]{}, 1f),
                        (new float[]{}, 1f),
                        (new float[]{-80f, 0f,  0f, 20f}, 1f),
                        (new float[]{}, 1f),
                        (new float[]{-80f, 0f,+80f, 20f}, 1f),
                        (new float[]{}, 1f),
                    },
                    new List<(float[], float)>(){
                        (new float[]{+80f, 0f,+80f}, 1f),
                        (new float[]{}, 1f),
                        (new float[]{}, 1f),
                        (new float[]{+80f, 0f,  0f, 20f}, 1f),
                        (new float[]{}, 1f),
                        (new float[]{+80f, 0f,+80f, 20f}, 1f),
                        (new float[]{}, 1f),
                    }
                };
                spawnManager.ListRotationPatternEnemy = new List<List<(float[], float)>>{
                    new List<(float[], float)>(){
                        (new float[]{180f}, 1f),
                        (new float[]{}, 1f),
                        (new float[]{110f, 20f}, 1f),
                        (new float[]{}, 1f),
                        (new float[]{70f, 20f}, 1f),
                        (new float[]{}, 1f),
                        (new float[]{180f, 20f}, 1f),
                    },
                    new List<(float[], float)>(){
                        (new float[]{180f}, 1f),
                        (new float[]{}, 1f),
                        (new float[]{250f, 20f}, 1f),
                        (new float[]{}, 1f),
                        (new float[]{290f, 20f}, 1f),
                        (new float[]{}, 1f),
                        (new float[]{180f, 20f}, 1f),
                    }
                };
                spawnManager.ListAttackPatternEnemy = new List<List<(float[], float[])>>{
                    new List<(float[], float[])>(){
                        (new float[]{}, new float[]{}),
                        (new float[]{5f}, new float[]{0f}),
                        (new float[]{1f}, new float[]{0f}),
                        (new float[]{1f}, new float[]{0f}),
                        (new float[]{1f}, new float[]{0f}),
                        (new float[]{1f}, new float[]{0f}),
                        (new float[]{6f}, new float[]{1f}),
                    },
                    new List<(float[], float[])>(){
                        (new float[]{}, new float[]{}),
                        (new float[]{5f}, new float[]{0f}),
                        (new float[]{1f}, new float[]{0f}),
                        (new float[]{1f}, new float[]{0f}),
                        (new float[]{1f}, new float[]{0f}),
                        (new float[]{1f}, new float[]{0f}),
                        (new float[]{6f}, new float[]{1f}),
                    }
                };
                spawnManager.bSpawnEnemies = true;
                bInitiatedLevel = true;
                return;
            }
            CheckFinishEnemyLevel();
        }
    }

    // ------------------------------------------------------------------------------------------------

    private void CheckFinishEnemyLevel()
    {
        if (    (bPreparedLevel)
            &&  (spawnManager.iNumEnemy == 0) )
        {
            FinishLevel();
            return;
        }
    }

    // ------------------------------------------------------------------------------------------------

    private void FinishLevel()
    {
        gameManager.fTimeStartLevel = Time.time;
        spawnManager.bSpawnAsteroids = false;
        spawnManager.bSpawnEnemies = false;
        spawnManager.ClearEnemyLists();
        bInitiatedLevel = false;
        bPreparedLevel = false;
        iLevel++;
    }

    // ------------------------------------------------------------------------------------------------

    public void ResetLevel()
    {
        bInitiatedLevel = false;
        bPreparedLevel = false;
    }

    // ------------------------------------------------------------------------------------------------

    public void ResetGame()
    {
        iLevel = 1;
        bInitiatedLevel = false;
        bPreparedLevel = false;
    }

    // ------------------------------------------------------------------------------------------------
}
