using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrapItem : ItemBase
{
    public GameObject paper;

    protected override void Start()
    {
        base.Start();
        
        paper.SetActive(false);
    }

    public override void Interact(GameObject player)
    {
        if(!GameVariables.CanPickPaper) return;
        
        paper.SetActive(true);

        var ani = paper.GetComponent<AnimationTriggerAndDestroy>();

        ani.TriggerAnimation();
        
        GameVariables.Day2EventCount++;
        
        base.Interact(player);
        
        
    }
}
