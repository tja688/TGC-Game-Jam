using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImageStoreTransmissionControl : MonoBehaviour
{
    
    private void Start()
    {
        GameVariables.OnDayChanged += DayChange;

        DayChange();
    }

    private void OnDisable()
    {
        GameVariables.OnDayChanged -= DayChange;
    }
    
    private void DayChange()
    {
        switch( GameVariables.Day)
        {
            case 1:
                break;
            case 2:
            case 3:
            case 4:
            case 5:
                break;
        }
    }
}
