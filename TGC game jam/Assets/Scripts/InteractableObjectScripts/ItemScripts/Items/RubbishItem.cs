using System;
using Unity.VisualScripting;
using UnityEngine;

public class RubbishItem : ItemBase
{
    
    [Header("Letters")]
    [SerializeField] private GameObject letter1;
    [SerializeField] private GameObject letter2;
    [SerializeField] private GameObject letter3;
    
    private int interactionCount = 0;
    private const int MaxInteractions = 4;

    public static event Action OnFindAllLetters; 
    
    public override void Interact(GameObject player)
    {
        interactionCount++;
        
        switch (interactionCount)
        {
            case 1:
                var newItem = Instantiate(letter1);
                BackpackManager.Instance.StoreItem(newItem);
                MessageTipManager.ShowMessage("Received a letter.");
                break;
            case 2:
                var newItem1 = Instantiate(letter2);
                BackpackManager.Instance.StoreItem(newItem1);
                MessageTipManager.ShowMessage("Received a letter.");
                break;
            case 3:
                var newItem2 = Instantiate(letter3);
                BackpackManager.Instance.StoreItem(newItem2);
                MessageTipManager.ShowMessage("Received a letter.");
                break;
            case 4:
                MessageTipManager.ShowMessage("That's all the mail for now.");
                QuestTipManager.Instance.CompleteTask("FindMail");
                OnFindAllLetters?.Invoke();
                break;
            default:
                Debug.LogWarning("Unexpected interaction count: " + interactionCount);
                break;
        }

        if (grabSound && interactionCount < MaxInteractions)
        {
            AudioManager.Instance.Play(grabSound);
        }
    }
}