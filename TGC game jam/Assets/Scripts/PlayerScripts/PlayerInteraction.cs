using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem; 

public class PlayerInteraction : MonoBehaviour
{
    [Header("光标设置")]
    [Tooltip("默认状态的光标贴图")]
    [SerializeField] private Texture2D defaultCursor;
    [Tooltip("悬停在可交互对象上时的光标贴图")]
    [SerializeField] private Texture2D interactableCursor;
    [Tooltip("光标的有效点击点（Hotspot）")]
    [SerializeField] private Vector2 cursorHotspot = Vector2.zero;

    private InputActions interactActions;
    private readonly List<IInteractable> interactablesInRange = new List<IInteractable>();
    private Camera mainCamera;

    private IInteractable currentlyHovered;

    private void Awake()
    {
        interactActions = new InputActions();
        mainCamera = Camera.main;
        
        ChangeCursor(defaultCursor);
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

        ChangeCursor(defaultCursor);
        ClearAllInteractables();
    }

    private void Update()
    {
        HandleMouseLogic();
    }
    

    private void HandleMouseLogic()
    {
        Vector2 mousePosition = mainCamera.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);

        IInteractable validHoveredObject = null;
        
        if (hit.collider != null)
        {
            var interactable = hit.collider.GetComponent<IInteractable>();
            if (interactable != null && interactablesInRange.Contains(interactable))
            {
                validHoveredObject = interactable;
            }
        }


        if (validHoveredObject != currentlyHovered)
        {
            if (validHoveredObject != null)
            {
                ChangeCursor(interactableCursor); // 变为交互光标
            }
            else
            {
                ChangeCursor(defaultCursor); // 变回默认光标
            }
            currentlyHovered = validHoveredObject; // 更新当前悬停的对象
        }


        if (Mouse.current.leftButton.wasPressedThisFrame && currentlyHovered != null)
        {
            currentlyHovered.Interact(gameObject);
        }
    }

    private void OnInteractInput(InputAction.CallbackContext context)
    {
        if (interactablesInRange.Count == 0) return;

        IInteractable closestInteractable = interactablesInRange
            .OrderBy(i => Vector2.Distance( (i as MonoBehaviour).transform.position, transform.position))
            .FirstOrDefault();
        
        closestInteractable?.Interact(gameObject); 
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        var interactableComponent = other.GetComponent<IInteractable>();
        if (interactableComponent == null || interactablesInRange.Contains(interactableComponent)) return;
        
        interactablesInRange.Add(interactableComponent);
        interactableComponent.ShowInteractionPrompt(gameObject);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        var interactableComponent = other.GetComponent<IInteractable>();
        if (interactableComponent == null || !interactablesInRange.Contains(interactableComponent)) return;
        
        if (interactableComponent == currentlyHovered)
        {
            ChangeCursor(defaultCursor);
            currentlyHovered = null;
        }

        interactableComponent.DestroyInteractionPrompt();
        interactablesInRange.Remove(interactableComponent);
    }
    
    private void ClearAllInteractables()
    {
        foreach (var interactable in interactablesInRange)
        {
            interactable.DestroyInteractionPrompt();
        }
        interactablesInRange.Clear();
    }
    
    private void ChangeCursor(Texture2D cursorTexture)
    {
        Cursor.SetCursor(cursorTexture, cursorHotspot, CursorMode.Auto);
    }
}