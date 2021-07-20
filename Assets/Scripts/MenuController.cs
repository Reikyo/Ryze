using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    private GameManager gameManager;

    public List<Button> butListButtons = new List<Button>();
    private List<GameObject> goListButtonMarkers = new List<GameObject>();
    private int iButtonSelected = 0;
    private bool bInputTriggered = false;

    // ------------------------------------------------------------------------------------------------

    void Start()
    {
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();

        foreach(Button button in butListButtons)
        {
            goListButtonMarkers.Add(button.GetComponent<RectTransform>().Find("Markers").gameObject);
        }
        // butListButtons[iButtonSelected].interactable = true;
        goListButtonMarkers[iButtonSelected].SetActive(true);
    }

    // ------------------------------------------------------------------------------------------------

    void Update()
    {
        if (Input.GetButtonDown("Submit"))
        {
            gameManager.HandleUIButton(butListButtons[iButtonSelected].gameObject.name);
        }
        if (    !bInputTriggered
            &&  ((int)Input.GetAxis("Vertical Menu") != 0) )
        {
            bInputTriggered = true;
            // butListButtons[iButtonSelected].interactable = false;
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
            // butListButtons[iButtonSelected].interactable = true;
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

}
