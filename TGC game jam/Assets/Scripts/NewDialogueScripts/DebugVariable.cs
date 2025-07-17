using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 用于在Inspector中配置的变量映射类
[System.Serializable]
public class DebugVariable
{
    // 自定义的变量类型枚举
    public enum VarType { Bool, Float, String }

    [Tooltip("要监控和修改的Dialogue System变量的准确名称")]
    public string name;

    [Tooltip("此变量的类型")]
    public VarType type = VarType.Bool;

    [Header("写 入 控 制")]
    [Tooltip("您可以在此修改值，以写入Dialogue System")]
    public bool boolValue;
    public float floatValue;
    public string stringValue;

    // 这个字段由脚本在运行时填充，用于在编辑器脚本中显示
    // [HideInInspector] 隐藏它，因为它不由用户直接编辑，而是由自定义编辑器显示
    [HideInInspector]
    public string currentStatus = "等待运行..."; 
}

