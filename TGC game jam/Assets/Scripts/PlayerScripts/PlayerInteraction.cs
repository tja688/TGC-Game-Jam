using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// 控制玩家与可交互对象进行交互的脚本。
/// 仅支持键盘交互，并提供单/多目标两种模式。
/// </summary>
public class PlayerInteraction : MonoBehaviour
{
    [Header("交互模式")]
    [Tooltip("开启后，只与范围内最近的一个对象交互，并只显示它的提示。关闭则对范围内所有对象都有效。")]
    [SerializeField] private bool singleInteractionMode = true;

    // 存储所有在交互范围内的对象
    private readonly List<IInteractable> interactablesInRange = new List<IInteractable>();
    // 仅在单交互模式下使用，追踪当前激活的（最近的）那个对象
    private IInteractable activeInteractable;

    private InputActions interactActions;
    
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
        
        ClearAllInteractables();
    }

    private void Update()
    {
        // 如果开启了单交互模式，则每帧更新哪个是最近的有效对象
        if (singleInteractionMode)
        {
            UpdateSingleActiveInteractable();
        }
    }

    /// <summary>
    /// 核心交互输入响应（例如按'E'键）
    /// </summary>
    private void OnInteractInput(InputAction.CallbackContext context)
    {
        if (interactablesInRange.Count == 0) return;

        if (singleInteractionMode)
        {
            // 单模式下，直接与已确定的激活对象交互
            activeInteractable?.Interact(gameObject);
        }
        else
        {
            // 多模式下，动态计算出最近的对象并交互
            IInteractable closestInteractable = interactablesInRange
                .OrderBy(i => Vector2.Distance((i as MonoBehaviour).transform.position, transform.position))
                .FirstOrDefault();
            
            closestInteractable?.Interact(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        var interactableComponent = other.GetComponent<IInteractable>();
        if (interactableComponent == null || interactablesInRange.Contains(interactableComponent)) return;

        interactablesInRange.Add(interactableComponent);
        
        // 如果是多交互模式，则直接显示提示
        if (!singleInteractionMode)
        {
            interactableComponent.ShowInteractionPrompt(gameObject);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        var interactableComponent = other.GetComponent<IInteractable>();
        if (interactableComponent == null || !interactablesInRange.Contains(interactableComponent)) return;

        // 如果离开的对象是当前激活的对象，需要清理它的状态
        if (interactableComponent == activeInteractable)
        {
            activeInteractable.DestroyInteractionPrompt();
            activeInteractable = null;
        }
        // 如果是多交互模式，则直接销毁提示
        else if (!singleInteractionMode)
        {
             interactableComponent.DestroyInteractionPrompt();
        }

        interactablesInRange.Remove(interactableComponent);
    }
    
    /// <summary>
    /// (单交互模式专用) 动态更新当前激活的交互对象
    /// </summary>
    private void UpdateSingleActiveInteractable()
    {
        IInteractable closest = null;
        if (interactablesInRange.Count > 0)
        {
            // 找到列表中最近的那个
            closest = interactablesInRange
                .OrderBy(i => Vector2.Distance((i as MonoBehaviour).transform.position, transform.position))
                .FirstOrDefault();
        }

        // 如果找到的最近对象和当前激活的对象不一致，则需要更新
        if (closest != activeInteractable)
        {
            // 1. 如果之前有激活的对象，先隐藏它的提示
            activeInteractable?.DestroyInteractionPrompt();

            // 2. 将找到的最近对象设为新的激活对象
            activeInteractable = closest;

            // 3. 如果新的激活对象存在，则显示它的提示
            activeInteractable?.ShowInteractionPrompt(gameObject);
        }
    }

    /// <summary>
    /// 清理所有交互对象的状态（例如禁用时或切换模式时）
    /// </summary>
    private void ClearAllInteractables()
    {
        // 多模式下，所有对象可能都有提示，需要全部清理
        if (!singleInteractionMode)
        {
            foreach (var interactable in interactablesInRange)
            {
                interactable.DestroyInteractionPrompt();
            }
        }
        // 单模式下，只需要清理那个激活的
        else
        {
            activeInteractable?.DestroyInteractionPrompt();
            activeInteractable = null;
        }
        
        interactablesInRange.Clear();
    }
}