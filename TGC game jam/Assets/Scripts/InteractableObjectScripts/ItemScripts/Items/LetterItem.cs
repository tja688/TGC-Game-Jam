using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LetterItem : MonoBehaviour, IStorable
{
    [SerializeField] protected string objectName = "Eco"; 

    public string ItemName => objectName;

    public void OnStored(BackpackManager backpackManager)
    {
        transform.SetParent(BackpackManager.BackpackObject);
        transform.localPosition = Vector3.zero;
        
        this.enabled = false;
    }

    public void OnRetrieved()
    {
    }
}
