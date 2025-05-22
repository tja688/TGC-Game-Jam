using System;
using UnityEngine;
using UnityEngine.InputSystem; 

public class PlayerInteraction : MonoBehaviour
{
    [Header("交互设置")]
    [Tooltip("交互按键的输入Action (例如 E 键)")]
    private InputActions interactActions;

    private IInteractable currentInteractable;
    private GameObject currentInteractableGameObject; 

    private bool isInteracting;
    
    private void Awake()
    {
        interactActions = new InputActions();
    }

    private void OnEnable()
    {
        interactActions.Enable();
        interactActions.PlayerControl.Interact.performed += OnInteractInput;
    }

    private void OnDisable()
    {
        interactActions.PlayerControl.Interact.performed -= OnInteractInput;
        interactActions.Disable();

        if (currentInteractable == null) return;
        currentInteractable.HideInteractionPrompt();
        ClearCurrentInteractable();
    }

    private void OnInteractInput(InputAction.CallbackContext context)
    {
        currentInteractable?.Interact(gameObject); 
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(isInteracting) return;
        
        var interactableComponent = other.GetComponent<IInteractable>();
        if (interactableComponent == null) return;

        currentInteractable = interactableComponent;
        currentInteractableGameObject = other.gameObject;
        currentInteractable.ShowInteractionPrompt(gameObject);
        
        isInteracting =  true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        var interactableComponent = other.GetComponent<IInteractable>();
        if (interactableComponent == null || interactableComponent != currentInteractable) return;
        currentInteractable.HideInteractionPrompt();
        ClearCurrentInteractable();
        
        isInteracting  = false;   
    }

    private void ClearCurrentInteractable()
    {
        currentInteractable = null;
        currentInteractableGameObject = null;
    }
}