using System.Collections;
using System.Collections.Generic;
using UnityEditor; // Needed for AssetDatabase
using UnityEngine;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    private GameManager gameManager;
    private AudioManager audioManager;

    public List<Button> butListButtons = new List<Button>();
    private List<GameObject> goListButtonMarkers = new List<GameObject>();
    private int iButtonSelected;
    private bool bInputTriggered;

    // ------------------------------------------------------------------------------------------------

    void OnEnable()
    {
        Reset();
    }

    // ------------------------------------------------------------------------------------------------

    void Start()
    {
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
        audioManager = GameObject.Find("Audio Manager").GetComponent<AudioManager>();

        foreach(Button button in butListButtons)
        {
            goListButtonMarkers.Add(button.GetComponent<RectTransform>().Find("Markers").gameObject);
        }

        goListButtonMarkers[iButtonSelected].SetActive(true);
    }

    // ------------------------------------------------------------------------------------------------

    void Update()
    {
        if (    !bInputTriggered
            &&  ((int)Input.GetAxis("Vertical Menu") != 0) )
        {
            bInputTriggered = true;
            goListButtonMarkers[iButtonSelected].SetActive(false);
            iButtonSelected -= (int)Input.GetAxis("Vertical Menu");
            if (iButtonSelected == goListButtonMarkers.Count)
            {
                iButtonSelected = 0;
            }
            if (iButtonSelected == -1)
            {
                iButtonSelected = goListButtonMarkers.Count-1;
            }
            goListButtonMarkers[iButtonSelected].SetActive(true);
            audioManager.sfxclpvolUIScroll.PlayOneShot();
            return;
        }

        if (    bInputTriggered
            &&  ((int)Input.GetAxis("Vertical Menu") == 0) )
        {
            bInputTriggered = false;
            return;
        }

        if (Input.GetButtonDown("Submit"))
        {
            switch(butListButtons[iButtonSelected].gameObject.name)
            {
                case "Button : Start": gameManager.StartGame(); break;
                case "Button : Controls": gameManager.ToggleUICanvas("Controls"); break;
                case "Button : Credits": gameManager.ToggleUICanvas("Credits"); break;
                case "Button : Retry": gameManager.RestartLevel(); break;
                case "Button : Title": gameManager.EndGame(); break;
            }
            audioManager.sfxclpvolUISubmit.PlayOneShot();
            return;
        }
    }

    // ------------------------------------------------------------------------------------------------

    private void Reset()
    {
        // This first check is only true if we have already passed through Start() and so have already been
        // active, therefore possibly having any button selected:

        if (goListButtonMarkers.Count > 0)
        {
            goListButtonMarkers[iButtonSelected].SetActive(false);
        }

        iButtonSelected = 0;
        bInputTriggered = false;

        if (goListButtonMarkers.Count > 0)
        {
            goListButtonMarkers[iButtonSelected].SetActive(true);
        }
    }

    // ------------------------------------------------------------------------------------------------

}
