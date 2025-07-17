using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

public class PaperNotice : MonoBehaviour
{
    public GameObject paper;
    
    public GameObject ball;
    
    
    private void Start()
    {
        paper.SetActive(false);
        
        ball.SetActive(false);

        GameVariables.OnDayChanged += OndayChange;
    }

    private void OnDestroy()
    {
        GameVariables.OnDayChanged -= OndayChange;
    }

    private void OndayChange()
    {
        if(GameVariables.Day == 2)
        {
            paper.SetActive(true);
        }
        
        if(GameVariables.Day == 3)
        {
            ball.SetActive(true);
        }
    }
    
    
}
