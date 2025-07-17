using UnityEngine;
using PixelCrushers.DialogueSystem;
using System.Collections.Generic;

/// <summary>
/// Dialogue System 变量的调试配置。
/// 包含了写入控制和状态显示的功能。
/// </summary>
[System.Serializable]
public class LiveDebugVariable
{
    // 自定义的变量类型枚举
    public enum VarType 
    { 
        Bool, 
        Number, // 修正: 将 Float 枚举项改为 Number，与Dialogue System保持一致
        String 
    }

    [Tooltip("要监控和修改的Dialogue System变量的准确名称")]
    public string variableName;

    [Tooltip("此变量的类型")]
    public VarType type = VarType.Bool;

    [Header("--- 写入控制 (在此处修改值) ---")]
    [Tooltip("如果类型是Bool，修改此项")]
    public bool boolValue;

    [Tooltip("如果类型是Number，修改此项")]
    public double doubleValue; // 修正: 将 float 字段改为 double 字段以保证精度

    [Tooltip("如果类型是String，修改此项")]
    public string stringValue;

    [Header("--- 实时状态 (仅供查看) ---")]
    [Tooltip("此字段仅用于显示，它会实时反映游戏内的真实值。请勿手动修改。")]
    [TextArea(1, 3)]
    public string currentStatus = "游戏未运行时不可用";

}