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

    // 新增：用于追踪当前悬停的对象，避免每帧都设置光标
    private IInteractable currentlyHovered;

    private void Awake()
    {
        interactActions = new InputActions();
        mainCamera = Camera.main;
        
        // 初始化光标为默认样式
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

        // 脚本禁用时，恢复默认光标并清理列表
        ChangeCursor(defaultCursor);
        ClearAllInteractables();
    }

    private void Update()
    {
        // 将鼠标相关的逻辑都放在一起
        HandleMouseLogic();
    }
    
    /// <summary>
    /// 统一处理所有与鼠标相关的逻辑：悬停检测和点击交互
    /// </summary>
    private void HandleMouseLogic()
    {
        // 从鼠标位置发射射线
        Vector2 mousePosition = mainCamera.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);

        IInteractable validHoveredObject = null;
        
        // 检查射线是否击中了在范围内的可交互对象
        if (hit.collider != null)
        {
            var interactable = hit.collider.GetComponent<IInteractable>();
            if (interactable != null && interactablesInRange.Contains(interactable))
            {
                validHoveredObject = interactable;
            }
        }

        // --- 1. 处理光标变化 ---
        // 如果当前悬停的对象与上一帧不同，则更新光标
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

        // --- 2. 处理点击交互 ---
        // 仅当鼠标左键按下，并且正悬停在一个有效的对象上时，才进行交互
        if (Mouse.current.leftButton.wasPressedThisFrame && currentlyHovered != null)
        {
            currentlyHovered.Interact(gameObject);
        }
    }

    /// <summary>
    /// E键交互逻辑：与最近的那个对象交互
    /// </summary>
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
        
        // 如果离开范围的对象正好是当前悬停的对象，立即重置光标
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

    /// <summary>
    /// 辅助方法，用于改变光标样式
    /// </summary>
    /// <param name="cursorTexture">要设置的光标贴图</param>
    private void ChangeCursor(Texture2D cursorTexture)
    {
        Cursor.SetCursor(cursorTexture, cursorHotspot, CursorMode.Auto);
    }
}