using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PowerUpController : MonoBehaviour
{
    public int iValue = 20;
    public TextMeshProUGUI guiLabel1;
    public TextMeshProUGUI guiLabel2;
    public TextMeshProUGUI guiLabel3;

    // Movement:
    private float fDegreesPerSecond = 90f;
    private float fDegreesPerFrame;

    // ------------------------------------------------------------------------------------------------

    void Start()
    {
        guiLabel1.text = iValue.ToString() + "\n*";
        guiLabel2.text = iValue.ToString() + "\n*";
        guiLabel3.text = iValue.ToString() + "\n*";
    }

    // ------------------------------------------------------------------------------------------------

    void Update()
    {
        fDegreesPerFrame = fDegreesPerSecond * Time.deltaTime;
        transform.Rotate(0f, fDegreesPerFrame, 0f, Space.World);
    }

    // ------------------------------------------------------------------------------------------------

}
