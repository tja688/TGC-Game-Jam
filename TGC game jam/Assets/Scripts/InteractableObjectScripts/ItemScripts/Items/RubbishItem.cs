using System;
using Unity.VisualScripting;
using UnityEngine;

public class RubbishItem : ItemBase
{
    
    [Header("Day1 Letters")]
    [SerializeField] private GameObject letter1;
    [SerializeField] private GameObject letter2;
    [SerializeField] private GameObject letter3;
    
    [Header("Day2 Letters")]
    [SerializeField] private GameObject letter4;
    [SerializeField] private GameObject letter5;
    [SerializeField] private GameObject letter6;
    [SerializeField] private GameObject letter7;

    [Header("Day3 Letters")]
    [SerializeField] private GameObject letter8;
    [SerializeField] private GameObject letter9;
    [SerializeField] private GameObject letter10;

    [Header("Day4 Letters")]
    [SerializeField] private GameObject letter11;
    [SerializeField] private GameObject letter12;

    [Header("Day5 Letters")]
    [SerializeField] private GameObject letter13;
    [SerializeField] private GameObject letter14;
    
    public static event Action OnFindAllLetters;
    
    private bool isFindAllLetters = false;

    
    protected override void Start()
    {
        base.Start();

        GameVariables.OnDayChanged += OnDayChange;

    }

    private void OnDayChange()
    {
        isFindAllLetters = false;
    }

    protected void OnDestroy()
    {
        GameVariables.OnDayChanged -= OnDayChange;
    }
    
    public override void Interact(GameObject player)
    {
        
        if(!isFindAllLetters)
            AudioManager.Instance.Play(grabSound);
        
        switch (GameVariables.Day)
        {
            case 1:
                if (isFindAllLetters)
                {
                    MessageTipManager.ShowMessage("The letter package is full.");
                    return;
                }
                var newItem1 = Instantiate(letter1);
                var newItem2 = Instantiate(letter2);
                var newItem3 = Instantiate(letter3);
                BackpackManager.Instance.StoreItem(newItem1);
                BackpackManager.Instance.StoreItem(newItem2);
                BackpackManager.Instance.StoreItem(newItem3);
                MessageTipManager.ShowMessage("Received some letters.");
                OnFindAllLetters?.Invoke();
                isFindAllLetters =  true;
                QuestTipManager.Instance.CompleteTask("FindMail");
                PlayerDialogue.Instance.SendLetter();
                break;
            case 2:
                if (isFindAllLetters)
                {
                    MessageTipManager.ShowMessage("The letter package is full.");
                    return;
                }
                var newItem4 = Instantiate(letter4);
                var newItem5 = Instantiate(letter5);
                var newItem6 = Instantiate(letter6);
                var newItem7 = Instantiate(letter7);
                BackpackManager.Instance.StoreItem(newItem4);
                BackpackManager.Instance.StoreItem(newItem5);
                BackpackManager.Instance.StoreItem(newItem6);
                BackpackManager.Instance.StoreItem(newItem7);
                MessageTipManager.ShowMessage("Received some letters.");
                OnFindAllLetters?.Invoke();
                isFindAllLetters =  true;
                break;
            case 3:
                if (isFindAllLetters)
                {
                    MessageTipManager.ShowMessage("The letter package is full.");
                    return;
                }
                var newItem8 = Instantiate(letter8);
                var newItem9 = Instantiate(letter9);
                var newItem10 = Instantiate(letter10);
                BackpackManager.Instance.StoreItem(newItem8);
                BackpackManager.Instance.StoreItem(newItem9);
                BackpackManager.Instance.StoreItem(newItem10);
                MessageTipManager.ShowMessage("Received some letters.");
                OnFindAllLetters?.Invoke();
                isFindAllLetters =  true;
                break;
            case 4:
                if (isFindAllLetters)
                {
                    MessageTipManager.ShowMessage("The letter package is full.");
                    return;
                }
                var newItem11 = Instantiate(letter11);
                var newItem12 = Instantiate(letter12);
                BackpackManager.Instance.StoreItem(newItem11);
                BackpackManager.Instance.StoreItem(newItem12);
                MessageTipManager.ShowMessage("Received some letters.");
                OnFindAllLetters?.Invoke();
                isFindAllLetters =  true;
                break;
            case 5:
                if (isFindAllLetters)
                {
                    MessageTipManager.ShowMessage("The letter package is full.");
                    return;
                }
                var newItem13 = Instantiate(letter13);
                var newItem14 = Instantiate(letter14);
                BackpackManager.Instance.StoreItem(newItem13);
                BackpackManager.Instance.StoreItem(newItem14);
                MessageTipManager.ShowMessage("Received some letters.");
                OnFindAllLetters?.Invoke();
                isFindAllLetters =  true;
                break;
        }

    }
    
}