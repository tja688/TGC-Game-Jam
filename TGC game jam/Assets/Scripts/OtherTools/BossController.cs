using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossController : MonoBehaviour
{
    public GameObject boss1;
    public GameObject boss2;

    private void Start()
    {
        boss2.SetActive(false);

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
            boss2.SetActive(true);
            boss1.SetActive(false);
        }
    }

}
