using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrandmaController : MonoBehaviour
{
    public GameObject grandma;

    
    private void Start()
    {

        GameVariables.OnDayChanged += OndayChange;
    }

    private void OnDestroy()
    {
        GameVariables.OnDayChanged -= OndayChange;
    }

    private void OndayChange()
    {
        if(GameVariables.Day == 5)
        {
            grandma.SetActive(false);
        }
    }
}
