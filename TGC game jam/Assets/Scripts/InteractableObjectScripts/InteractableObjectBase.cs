using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent(typeof(Collider2D))] 
public abstract class InteractableObjectBase : MonoBehaviour , IInteractable
{
    [SerializeField] protected string objectName = "Eco"; 

    public Transform PromptAnchor => promptAnchorTransform;
    [SerializeField] protected Transform promptAnchorTransform;
    
    public GameObject InteractNotice => interactNotice;
    [SerializeField] private GameObject interactNotice;

    protected GameObject InstantiatedPromptInstance;

    protected Vector3 PromptAnchorScreenPoint;

    protected Camera Camera1;

    protected virtual void Start()
    {
        Camera1 = Camera.main;
        if (!PromptAnchor)
        {
            Debug.LogWarning("No prompt anchor found");;
        }

        if (!InteractNotice)
        {
            Debug.LogWarning("Instantiated prompt instance");
        }
    }
    
    
    public void ShowInteractionPrompt(GameObject instigator)
    {
        if (InstantiatedPromptInstance != null) return;
        InstantiatedPromptInstance = Instantiate(InteractNotice,PromptAnchor.position, PromptAnchor.rotation);
    }

    protected virtual void FixedUpdate()
    {
        if (!InstantiatedPromptInstance) return;
        InstantiatedPromptInstance.transform.position = PromptAnchor.position;
        
    }

    public void DestroyInteractionPrompt()
    {
        if (!InstantiatedPromptInstance ) return;

        Destroy(InstantiatedPromptInstance);
        InstantiatedPromptInstance = null; 
    }

    public abstract void Interact(GameObject instigator);

}