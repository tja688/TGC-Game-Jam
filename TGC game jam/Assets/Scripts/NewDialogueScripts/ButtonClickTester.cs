using UnityEngine;

// 这是一个简单的测试脚本，用于验证UI按钮的点击事件是否能被正确触发。
public class ButtonClickTester : MonoBehaviour
{
    // 这是一个公开(public)的方法，因此可以从Unity的Inspector面板中被UI事件（如 OnClick）调用。
    public void LogClickMessage()
    {
        // 当这个方法被调用时，它会在Unity的Console窗口打印一条带有时间戳的调试信息。
        // 使用醒目的格式，方便在众多日志中快速找到。
        Debug.Log("========== [按钮点击测试成功] ==========\n按钮 '" + gameObject.name + "' 的点击事件已触发！\n当前时间: " + Time.time);
    }
}