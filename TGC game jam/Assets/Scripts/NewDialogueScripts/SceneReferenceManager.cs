using UnityEngine;
using UnityEngine.UI; // 需要引入UI命名空间

public class SceneReferenceManager : MonoBehaviour
{
    // 创建一个静态的、全局唯一的实例，方便任何地方访问
    public static SceneReferenceManager Instance { get; private set; }

    [Header("全局UI引用")]
    [Tooltip("用于继续对话的全屏点击按钮")]
    public Button globalContinueButton; // 我们需要让预制体找到的按钮

    private void Awake()
    {
        // 这是单例模式的标准实现
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;

        }
    }
}