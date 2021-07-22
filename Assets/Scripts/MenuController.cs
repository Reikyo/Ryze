using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    private GameManager gameManager;

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

        foreach(Button button in butListButtons)
        {
            goListButtonMarkers.Add(button.GetComponent<RectTransform>().Find("Markers").gameObject);
        }

        goListButtonMarkers[iButtonSelected].SetActive(true);
    }

    // ------------------------------------------------------------------------------------------------

    void Update()
    {
        if (Input.GetButtonDown("Submit"))
        {
            gameManager.HandleUIButton(butListButtons[iButtonSelected].gameObject.name);
            return;
        }

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
            return;
        }

        if (    bInputTriggered
            &&  ((int)Input.GetAxis("Vertical Menu") == 0) )
        {
            bInputTriggered = false;
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
