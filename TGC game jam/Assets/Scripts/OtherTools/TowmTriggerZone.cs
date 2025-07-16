using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowmTriggerZone : MonoBehaviour
{
    [SerializeField]
    private string targetTag = "Player";

    private bool isTalk;

    private void Awake()
    {
        var myCollider = GetComponent<Collider2D>();
        if (!myCollider.isTrigger)
        {
            myCollider.isTrigger = true;
        }

        isTalk = false;
    }


    private void Start()
    {
        GameVariables.OnDayChanged += DayChange;
        
    }

    private void OnDisable()
    {
        GameVariables.OnDayChanged -= DayChange;
    }

    /// <summary>
    /// 当另一个带有 Collider2D 的对象进入此对象的触发器时，此方法会被调用。
    /// </summary>
    /// <param name="other">进入触发器的另一个对象的 Collider2D 组件。</param>
    private void OnTriggerEnter2D(Collider2D other)
    {
        if(isTalk) return;
        
        if (other.CompareTag(targetTag))
        {
            switch( GameVariables.Day)
            {
                case 1:
                    PlayerDialogue.Instance.Day1OutSideTalk();
                    // QuestTipManager.Instance.AddTask("SendLetterDay1", "Objective: Deliver Mail Across the Town. February 8th.");
                    isTalk = true;
                    break;
                case 2:
                    PlayerDialogue.Instance.Day2OutSideTalk();
                    // QuestTipManager.Instance.AddTask("SendLetterDay2", "Objective: Deliver Mail Across the Town. February 9th.");
                    isTalk = true;
                    break;
                case 3:
                    PlayerDialogue.Instance.Day3OutSideTalk();
                    // QuestTipManager.Instance.AddTask("SendLetterDay3", "Objective: Deliver Mail Across the Town. February 10th.");
                    isTalk = true;
                    break;
                case 4:
                    PlayerDialogue.Instance.Day4OutSideTalk();
                    // QuestTipManager.Instance.AddTask("SendLetterDay4", "Objective: Deliver Mail Across the Town. February 11th.");
                    isTalk = true;
                    break;
                case 5:
                    PlayerDialogue.Instance.Day5OutSideTalk();
                    // QuestTipManager.Instance.AddTask("SendLetterDay5", "Objective: Deliver Mail Across the Town. February 12th.");
                    isTalk = true;
                    break;
            }
        }
    }


    private void DayChange()
    {
        isTalk = false;
    }
}
