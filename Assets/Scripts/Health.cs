using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
    // Health:
    public int iHealth = 0;
    public int iHealthMax = 100;
    public Slider sliHealth;

    public int iHealthDeltaPerSec = 0;
    private float fTimeNextHealthChange = 0f;

    // ------------------------------------------------------------------------------------------------

    void Start()
    {
        if (    (sliHealth)
            &&  (sliHealth.value != iHealth) )
        {
            SetSlider();
        }
    }

    // ------------------------------------------------------------------------------------------------

    void Update()
    {
        if (    (iHealthDeltaPerSec != 0)
            &&  (Time.time >= fTimeNextHealthChange) )
        {
            if (iHealthDeltaPerSec > 0)
            {
                Change(+1);
            }
            else
            {
                Change(-1);
            }
            fTimeNextHealthChange = Time.time + (1f / Math.Abs(iHealthDeltaPerSec));
        }
    }

    // ------------------------------------------------------------------------------------------------

    public void Change(int iHealthDelta)
    {
        if (    ((iHealthDelta > 0) && (iHealth == iHealthMax))
            ||  ((iHealthDelta < 0) && (iHealth == 0)) )
        {
            return;
        }

        iHealth += iHealthDelta;

        if (iHealth > iHealthMax)
        {
            iHealth = iHealthMax;
        }
        else if (iHealth < 0)
        {
            iHealth = 0;
        }

        if (sliHealth)
        {
            SetSlider();
        }
    }

    // ------------------------------------------------------------------------------------------------

    private void SetSlider()
    {
        sliHealth.value = iHealth;
        if (    (iHealth > 0)
            &&  (!sliHealth.transform.Find("Fill Area").gameObject.activeSelf) )
        {
            sliHealth.transform.Find("Fill Area").gameObject.SetActive(true);
        }
        else if (   (iHealth == 0)
                &&  (sliHealth.transform.Find("Fill Area").gameObject.activeSelf) )
        {
            sliHealth.transform.Find("Fill Area").gameObject.SetActive(false);
        }
    }

    // ------------------------------------------------------------------------------------------------

}
