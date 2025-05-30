using UnityEngine;
using UnityEngine.UI;
//不再需要 using System; 因为没有静态事件了

[RequireComponent(typeof(Button))]
public class PlayerInterfaceOpenerButton : MonoBehaviour
{
    [Header("目标设置")]
    [Tooltip("点击按钮时要显示的UI组的名称")]
    public string targetUIGroupName = "PlayerPanel";

    private Button button;


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
        // 1. 打开目标UI组
        if (UIAnimationManager.Instance)
        {
            UIAnimationManager.Instance.ShowGroup(targetUIGroupName);
        }
        else
        {
            Debug.LogError($"PlayerInterfaceOpenerButton: UIAnimationManager.Instance 为空！无法显示UI组 '{targetUIGroupName}'。", this);
        }
        
        PlayerInputController.Instance?.ActivateUIControls();

        this.gameObject.SetActive(false);
    }

    /// <summary>
    /// 公开接口：用于从外部重新显示并激活此按钮。
    /// </summary>
    public void ShowAndActivateButton()
    {
        this.gameObject.SetActive(true);

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