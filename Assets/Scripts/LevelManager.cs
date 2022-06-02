using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    private GameManager gameManager;
    private SpawnManager spawnManager;

    public int iLevel = 1;
    public bool bLevelReady = false;
    private bool bInitiatedLevel1 = false;
    private bool bInitiatedLevel2 = false;
    private float fTimeDeltaWaitStartLevel1 = 10f;
    private float fTimeDeltaWaitStartLevel2 = 10f;
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
            if (!bInitiatedLevel1)
            {
                spawnManager.bSpawnAsteroids = true;
                bInitiatedLevel1 = true;
                return;
            }
            if (Time.time >= gameManager.fTimeStartLevel + fTimeDeltaWaitStartLevel1 + fTimeDeltaLevel1)
            {
                gameManager.fTimeStartLevel = Time.time;
                spawnManager.bSpawnAsteroids = false;
                bLevelReady = false;
                iLevel++;
                return;
            }
        }
        if (iLevel == 2)
        {
            if (Time.time < gameManager.fTimeStartLevel + fTimeDeltaWaitStartLevel2)
            {
                return;
            }
            if (!bInitiatedLevel2)
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
                bInitiatedLevel2 = true;
                return;
            }
            if (    (bLevelReady)
                &&  (spawnManager.iNumEnemy == 0) )
            {
                gameManager.fTimeStartLevel = Time.time;
                spawnManager.bSpawnEnemies = false;
                bLevelReady = false;
                iLevel++;
                return;
            }
        }
    }

    // ------------------------------------------------------------------------------------------------

    public void Reset()
    {
        iLevel = 1;
        bInitiatedLevel1 = false;
        bInitiatedLevel2 = false;
    }

    // ------------------------------------------------------------------------------------------------

}
