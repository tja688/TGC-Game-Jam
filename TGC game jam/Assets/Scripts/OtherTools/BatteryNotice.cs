using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatteryNotice : MonoBehaviour
{
    public GameObject battery;

    private void Start()
    {
        battery.SetActive(false);

        GameVariables.OnDayChanged += OndayChange;
    }

    private void OnDestroy()
    {
        GameVariables.OnDayChanged -= OndayChange;
    }

    private void OndayChange()
    {
        if(GameVariables.Day == 3)
        {
            battery.SetActive(true);
        }
    }
    
}
