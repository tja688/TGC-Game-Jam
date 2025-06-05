using UnityEngine;

public class TimeSpeedController : MonoBehaviour
{
    [SerializeField] private float normalTimeScale = 1f;  // 正常速度
    [SerializeField] private float fastTimeScale = 2f;    // 加速后的速度

    private void Update()
    {
        // 检测是否按住 Ctrl 键（Windows/Linux）或 Command 键（Mac）
        var isCtrlPressed = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);

        // 根据按键状态调整 Time.timeScale
        Time.timeScale = isCtrlPressed ? fastTimeScale : normalTimeScale;
        
    }

    private void OnDestroy()
    {
        // 确保脚本销毁时恢复默认时间（避免游戏卡在加速状态）
        Time.timeScale = normalTimeScale;
    }
}