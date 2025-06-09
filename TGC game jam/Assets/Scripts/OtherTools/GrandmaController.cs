using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrandmaController : MonoBehaviour
{
    public GameObject grandma;

    public Transform aimGrandmaPos;
    
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
        if(GameVariables.Day == 4)
        {
            grandma.SetActive(false);
        }
        
        if(GameVariables.Day == 5)
        {
            grandma.SetActive(true);
            grandma.transform.position =  aimGrandmaPos.position;
        }
    }
}
