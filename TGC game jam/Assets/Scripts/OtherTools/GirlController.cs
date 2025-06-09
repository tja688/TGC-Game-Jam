using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GirlController : MonoBehaviour
{
    public GameObject girl;

    private void Start()
    {
        girl.SetActive(false);

        GameVariables.OnDayChanged += OndayChange;
    }

    private void OnDestroy()
    {
        GameVariables.OnDayChanged -= OndayChange;
    }

    private void OndayChange()
    {
        if(GameVariables.Day == 4)
        {
            girl.SetActive(true);
        }
        
        if(GameVariables.Day == 5)
        {
            girl.SetActive(false);
        }
    }

}
