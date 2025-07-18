using PixelCrushers.DialogueSystem;
using UnityEngine;
using UnityEngine.SceneManagement; // 必须引用此命名空间来操作场景

/// <summary>
/// 负责管理游戏关卡的核心逻辑，如加载、重启等。
/// </summary>
public class LevelManager : MonoBehaviour
{
    // --- 单例模式设置 ---
    // 创建一个静态的、全局可访问的实例
    public static LevelManager Instance { get; private set; }

    private void Awake()
    {
        // 确保场景中只有一个LevelManager实例
        if (Instance != null && Instance != this)
        {
            // 如果已经存在一个实例，则销毁这个新的
            Destroy(gameObject);
        }
        else
        {
            // 否则，将此实例设为当前的单例
            Instance = this;
            // （可选）让这个管理器在切换场景时不被销毁
            // DontDestroyOnLoad(gameObject); 
        }
    }
    // --- 单例模式结束 ---

    /// <summary>
    /// 重启当前激活的场景。这是一个公开方法，可以被其他任何脚本调用。
    /// </summary>
    public void RestartCurrentLevel()
    {
        Debug.Log("接收到重启指令，正在重启当前关卡...");

        // 获取当前激活的场景
        Scene currentScene = SceneManager.GetActiveScene();
        
        DialogueManager.ResetDatabase(DatabaseResetOptions.RevertToDefault);
        
        // 重新加载该场景
        SceneManager.LoadScene(currentScene.buildIndex);

        // 进阶提示：你可以在这里加入一个黑屏淡出效果，让体验更平滑
        // 例如：StartCoroutine(FadeAndRestart());
    }
}