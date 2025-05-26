using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public abstract class InteractableObjectBase : MonoBehaviour , IInteractable
{
    public Transform PromptAnchor => promptAnchorTransform;
    [SerializeField] protected Transform promptAnchorTransform;
    
    public GameObject InteractNotice => interactNotice;
    [SerializeField] private GameObject interactNotice;

    private GameObject instantiatedPromptInstance;

    protected Vector3 PromptAnchorScreenPoint;

    protected virtual void Start()
    {
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
        instantiatedPromptInstance = Instantiate(InteractNotice,PromptAnchor.position, PromptAnchor.rotation,transform);
    }

    public void HideInteractionPrompt()
    {
        if (!instantiatedPromptInstance ) return;

        Destroy(instantiatedPromptInstance);
    }

    public abstract void Interact(GameObject instigator);

}
