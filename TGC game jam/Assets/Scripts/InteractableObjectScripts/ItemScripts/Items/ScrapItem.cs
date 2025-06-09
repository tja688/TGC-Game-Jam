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
        paper.SetActive(true);

        var ani = paper.GetComponent<AnimationTriggerAndDestroy>();

        ani.TriggerAnimation();

        QuestTipManager.Instance.CompleteTask("Origami");
        
        GameVariables.Day2EventCount++;
        
        Debug.Log("Day 2 Event:" + GameVariables.Day2EventCount);
        
        base.Interact(player);
        
        
    }
}
