using System.Collections;
using System.Collections.Generic;
using UnityEditor; // Needed for AssetDatabase
using UnityEngine;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    private GameManager gameManager;

    public List<Button> butListButtons = new List<Button>();
    private List<GameObject> goListButtonMarkers = new List<GameObject>();
    private int iButtonSelected;
    private bool bInputTriggered;

    // Audio:
    private AudioSource audioSource;
    private AudioClip sfxclpUIScroll;
    private AudioClip sfxclpUISubmit;

    // ------------------------------------------------------------------------------------------------

    void OnEnable()
    {
        Reset();
    }

    // ------------------------------------------------------------------------------------------------

    void Start()
    {
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();

        foreach(Button button in butListButtons)
        {
            goListButtonMarkers.Add(button.GetComponent<RectTransform>().Find("Markers").gameObject);
        }

        goListButtonMarkers[iButtonSelected].SetActive(true);

        audioSource = GameObject.Find("Audio Source").GetComponent<AudioSource>();
        sfxclpUIScroll = (AudioClip)AssetDatabase.LoadAssetAtPath("Assets/Asset Store/Audio/Little Robot Sound Factory/UI Sfx/Mp3/Click_Electronic/Click_Electronic_14.mp3", typeof(AudioClip));
        sfxclpUISubmit = (AudioClip)AssetDatabase.LoadAssetAtPath("Assets/Asset Store/Audio/Little Robot Sound Factory/UI Sfx/Mp3/Click_Electronic/Click_Electronic_12.mp3", typeof(AudioClip));
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
            audioSource.PlayOneShot(sfxclpUIScroll, 0.5f);
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
            gameManager.HandleUIButton(butListButtons[iButtonSelected].gameObject.name);
            audioSource.PlayOneShot(sfxclpUISubmit, 0.5f);
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
