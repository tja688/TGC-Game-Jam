using System;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]

public class UIStartSettingShowHideButton : MonoBehaviour
{
    [Header("UI组设置")]
    [Tooltip("要控制的UI组名称")]
    [SerializeField] private string targetGroupName;

    private Button button;
    private bool isGroupShown = false; // 用于跟踪当前组的状态
    
    public static event Action OpenBeginSettingUI;
    
    public static event Action CloseBeginSettingUI;


    private void Awake()
    {
        // 获取Button组件
        button = GetComponent<Button>();
        
        // 添加点击事件监听器
        button.onClick.AddListener(ToggleGroup);
    }

    private void ToggleGroup()
    {
        // 获取UIAnimationManager实例
        UIAnimationManager animationManager = UIAnimationManager.Instance;
        
        if (animationManager == null)
        {
            Debug.LogError("UIGroupToggleButton: 无法找到UIAnimationManager实例！");
            return;
        }

        // 检查目标组是否存在
        if (string.IsNullOrEmpty(targetGroupName))
        {
            Debug.LogWarning("UIGroupToggleButton: 目标组名未设置！");
            return;
        }

        // 切换组的显示状态
        if (isGroupShown)
        {
            animationManager.HideGroup(targetGroupName);
            CloseBeginSettingUI?.Invoke();
        }
        else
        {
            animationManager.ShowGroup(targetGroupName);
            OpenBeginSettingUI?.Invoke();
        }

        // 切换状态
        isGroupShown = !isGroupShown;
    }

    // 可选：在Inspector中设置目标组名后更新状态
    private void OnValidate()
    {
        if (string.IsNullOrEmpty(targetGroupName))
        {
            Debug.LogWarning("UIGroupToggleButton: 目标组名未设置！");
        }
    }
}
