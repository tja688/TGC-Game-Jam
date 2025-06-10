using System;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class PlayerInterfaceOpenerButton : MonoBehaviour
{
    [Header("目标设置")]
    [Tooltip("点击按钮时要显示的UI组的名称")]
    public string targetUIGroupName = "PlayerPanel";

    private Button button;

    public static event Action OpenPlayerPanelUI;
    
    public static event Action ClosePlayerPanelUI;
    
    public static bool OnOpenUI { get; private set; }
    
    private void Awake()
    {
        button = GetComponent<Button>();
        if (button) return;
        Debug.LogError("PlayerInterfaceOpenerButton: 未在此GameObject上找到Button组件!", this);
        enabled = false;
    }

    private void Start()
    {
        button.onClick.AddListener(HandleButtonClick);
    }

    private void HandleButtonClick()
    {
        if (UIAnimationManager.Instance)
        {
            UIAnimationManager.Instance.ShowGroup(targetUIGroupName);
            OpenPlayerPanelUI?.Invoke();
        }
        else
        {
            Debug.LogError($"PlayerInterfaceOpenerButton: UIAnimationManager.Instance 为空！无法显示UI组 '{targetUIGroupName}'。", this);
        }
    
        if (BackpackUIManager.Instance)
        {
            BackpackUIManager.Instance.RefreshContentView(); 
        }
        else
        {
            Debug.LogError("PlayerInterfaceOpenerButton: BackpackUIManager.Instance 为空！无法更新背包UI。", this);
        }
    
        PlayerInputController.Instance?.ActivateUIControls();
    
        OnOpenUI = true;

        this.gameObject.SetActive(false); 
    }

    
    public void ShowAndActivateButton()
    {
        this.gameObject.SetActive(true);

        OnOpenUI = false;
        
        ClosePlayerPanelUI?.Invoke();
        
        PlayerInputController.Instance?.ActivatePlayerControls();
    }

    public void OnDestroy()
    {
        if (button)
        {
            button.onClick.RemoveListener(HandleButtonClick);
        }
    }
    

    
    
}