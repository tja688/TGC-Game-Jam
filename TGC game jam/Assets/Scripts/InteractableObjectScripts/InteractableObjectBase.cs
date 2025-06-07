using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent(typeof(Collider2D))] // <--- 新增此行
public abstract class InteractableObjectBase : MonoBehaviour , IInteractable
{
    // ... 其余代码保持不变 ...
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
        if (InstantiatedPromptInstance != null) return; // 防止重复创建
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
        InstantiatedPromptInstance = null; // <--- 最好在销毁后设置为null
    }

    public abstract void Interact(GameObject instigator);

}