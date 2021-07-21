using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Charge : MonoBehaviour
{
    // Charge:
    public int iCharge;
    public int iChargeMax = 100;
    public Slider sliCharge;

    // ------------------------------------------------------------------------------------------------

    void Start()
    {
        iCharge = 0;
        if (sliCharge)
        {
            sliCharge.value = iCharge;
            if ((iCharge > 0) && !sliCharge.transform.Find("Fill Area").gameObject.activeSelf)
            {
                sliCharge.transform.Find("Fill Area").gameObject.SetActive(true);
            }
        }
    }

    // ------------------------------------------------------------------------------------------------

    public void Change(int iChargeDelta)
    {
        if (    ((iChargeDelta > 0) && (iCharge == iChargeMax))
            ||  ((iChargeDelta < 0) && (iCharge == 0)) )
        {
            return;
        }

        iCharge += iChargeDelta;

        if (iCharge > iChargeMax)
        {
            iCharge = iChargeMax;
        }
        else if (iCharge < 0)
        {
            iCharge = 0;
        }

        if (sliCharge)
        {
            sliCharge.value = iCharge;
            if ((iCharge > 0) && !sliCharge.transform.Find("Fill Area").gameObject.activeSelf)
            {
                sliCharge.transform.Find("Fill Area").gameObject.SetActive(true);
            }
            else if (iCharge == 0)
            {
                sliCharge.transform.Find("Fill Area").gameObject.SetActive(false);
            }
        }
    }

    // ------------------------------------------------------------------------------------------------

}
