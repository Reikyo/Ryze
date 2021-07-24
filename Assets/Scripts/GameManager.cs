using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor; // Needed for AssetDatabase
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private SpawnManager spawnManager;
    private AudioManager audioManager;

    // Status:
    public bool bInPlay = false;
    public bool bPaused = false;

    // UI:
    private GameObject goUICanvasTitle;
    private GameObject goUICanvasControls;
    private GameObject goUICanvasCredits;
    private GameObject goUICanvasPaused;
    private GameObject goUICanvasGameOver;
    private GameObject goUICanvasHUD;
    private GameObject goUICanvasActive = null;

    // Camera:
    private Camera camMainCamera;
    public Vector3 v3CamLowerLeft;
    public Vector3 v3CamUpperRight;
    public Vector3 v3CamLowerLeftStars;
    public Vector3 v3CamUpperRightStars;

    // Movement:
    public Vector3 v3MoveLimitLowerLeft;
    public Vector3 v3MoveLimitUpperRight;
    private Vector3 v3MoveLimitCamOffset = new Vector3(5f, 0f, 5f);

    // Existence:
    public Vector3 v3ExistLimitLowerLeft;
    public Vector3 v3ExistLimitUpperRight;
    public Vector3 v3ExistLimitLowerLeftStars;
    public Vector3 v3ExistLimitUpperRightStars;
    private Vector3 v3ExistLimitCamOffset = new Vector3(20f, 0f, 20f);

    // Score:
    public int iScore = 0;
    public int iScoreMax = 99999;
    public TextMeshProUGUI guiScore;

    public GameObject goPlayer;
    private GameObject goPlayerClone;

    // ------------------------------------------------------------------------------------------------

    void Start()
    {
        spawnManager = GameObject.Find("Spawn Manager").GetComponent<SpawnManager>();
        audioManager = GameObject.Find("Audio Manager").GetComponent<AudioManager>();

        goUICanvasTitle = GameObject.Find("Canvas : Title");
        goUICanvasControls = GameObject.Find("Canvas : Controls");
        goUICanvasCredits = GameObject.Find("Canvas : Credits");
        goUICanvasPaused = GameObject.Find("Canvas : Paused");
        goUICanvasGameOver = GameObject.Find("Canvas : Game Over");
        goUICanvasHUD = GameObject.Find("Canvas : HUD");

        goUICanvasTitle.SetActive(true);
        goUICanvasControls.SetActive(false);
        goUICanvasCredits.SetActive(false);
        goUICanvasPaused.SetActive(false);
        goUICanvasGameOver.SetActive(false);
        goUICanvasHUD.SetActive(false);

        camMainCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
        v3CamLowerLeft = camMainCamera.ViewportToWorldPoint(new Vector3(0, 0, camMainCamera.transform.position.y));
        v3CamUpperRight = camMainCamera.ViewportToWorldPoint(new Vector3(1, 1, camMainCamera.transform.position.y));
        v3CamLowerLeftStars = camMainCamera.ViewportToWorldPoint(new Vector3(0, 0, camMainCamera.transform.position.y - spawnManager.fPositionYSpawnStars));
        v3CamUpperRightStars = camMainCamera.ViewportToWorldPoint(new Vector3(1, 1, camMainCamera.transform.position.y - spawnManager.fPositionYSpawnStars));

        v3MoveLimitLowerLeft = v3CamLowerLeft + v3MoveLimitCamOffset;
        v3MoveLimitUpperRight = v3CamUpperRight - v3MoveLimitCamOffset;

        v3ExistLimitLowerLeft = v3CamLowerLeft - v3ExistLimitCamOffset;
        v3ExistLimitUpperRight = v3CamUpperRight + v3ExistLimitCamOffset;
        v3ExistLimitLowerLeftStars = v3CamLowerLeftStars - v3ExistLimitCamOffset;
        v3ExistLimitUpperRightStars = v3CamUpperRightStars + v3ExistLimitCamOffset;
    }

    // ------------------------------------------------------------------------------------------------

    void Update()
    {
        if (    goUICanvasActive
            &&  Input.GetButtonDown("Cancel") )
        {
            ToggleUICanvas();
            audioManager.sfxclpvolUICancel.PlayOneShot();
            return;
        }

        if (    bInPlay
            &&  !bPaused
            &&  Input.GetButtonDown("Pause") )
        {
            goPlayerClone.GetComponent<PlayerController>().enabled = false;
            Time.timeScale = 0f;
            bPaused = true;
            goUICanvasPaused.SetActive(true);
            return;
        }

        if (    bInPlay
            &&  bPaused
            &&  (   Input.GetButtonDown("Pause")
                ||  Input.GetButtonDown("Cancel") ) )
        {
            goPlayerClone.GetComponent<PlayerController>().enabled = true;
            Time.timeScale = 1f;
            bPaused = false;
            goUICanvasPaused.SetActive(false);
            return;
        }
    }

    // ------------------------------------------------------------------------------------------------

    public void HandleUIButton(string sNameUIButton)
    {
        switch(sNameUIButton)
        {
            case "Button : Start": StartGame(); break;
            case "Button : Controls": ToggleUICanvas(goUICanvasControls); break;
            case "Button : Credits": ToggleUICanvas(goUICanvasCredits); break;
            case "Button : Retry": RestartLevel(); break;
            case "Button : Title": EndGame(); break;
        }
    }

    // ------------------------------------------------------------------------------------------------

    public void StartGame()
    {
        goPlayerClone = Instantiate(goPlayer);
        // spawnManager.bSpawnAsteroids = true;
        // spawnManager.bSpawnEnemies = true;
        goUICanvasTitle.SetActive(false);
        goUICanvasHUD.SetActive(true);
        bInPlay = true;
    }

    // ------------------------------------------------------------------------------------------------

    public void GameOver()
    {
        spawnManager.bSpawnEnemies = false;
        goPlayerClone.GetComponent<PlayerController>().enabled = false;
        goUICanvasGameOver.SetActive(true);
        bInPlay = false;
    }

    // ------------------------------------------------------------------------------------------------

    public void RestartLevel()
    {
        spawnManager.DestroyAsteroids();
        spawnManager.DestroyEnemies();
        spawnManager.DestroyPowerUps();
        spawnManager.DestroyProjectiles();
        Destroy(goPlayerClone);
        ChangeScore(-iScore);
        goPlayerClone = Instantiate(goPlayer);
        spawnManager.bSpawnEnemies = true;
        goUICanvasGameOver.SetActive(false);
        bInPlay = true;
    }

    // ------------------------------------------------------------------------------------------------

    public void EndGame()
    {
        spawnManager.bSpawnAsteroids = false;
        spawnManager.bSpawnEnemies = false;
        spawnManager.DestroyAsteroids();
        spawnManager.DestroyEnemies();
        spawnManager.DestroyPowerUps();
        spawnManager.DestroyProjectiles();
        Destroy(goPlayerClone);
        ChangeScore(-iScore);
        goUICanvasHUD.SetActive(false);
        goUICanvasGameOver.SetActive(false);
        goUICanvasTitle.SetActive(true);
    }

    // ------------------------------------------------------------------------------------------------

    public void ToggleUICanvas(GameObject goUICanvas = null)
    {
        if (    !goUICanvas
            &&  goUICanvasActive )
        {
            goUICanvas = goUICanvasActive;
        }

        goUICanvasTitle.SetActive(!goUICanvasTitle.activeSelf);
        goUICanvas.SetActive(!goUICanvas.activeSelf);

        if (goUICanvas.activeSelf)
        {
            goUICanvasActive = goUICanvas;
        }
        else
        {
            goUICanvasActive = null;
        }
    }

    // ------------------------------------------------------------------------------------------------

    public void ChangeScore(int iScoreDelta)
    {
        if (    ((iScoreDelta > 0) && (iScore == iScoreMax))
            ||  ((iScoreDelta < 0) && (iScore == 0)) )
        {
            return;
        }

        iScore += iScoreDelta;

        if (iScore > iScoreMax)
        {
            iScore = iScoreMax;
        }
        else if (iScore < 0)
        {
            iScore = 0;
        }

        guiScore.text = iScore.ToString("D5");
    }

    // ------------------------------------------------------------------------------------------------

}
