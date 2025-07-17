using UnityEngine;
using PixelCrushers.DialogueSystem;
using System.Collections.Generic;

/// <summary>
/// 一个功能更强的Dialogue System变量调试器。
/// 它能实时监控变量状态，并在检视面板中直接修改它们。
/// </summary>
public class DialogueVariableDebugger : MonoBehaviour
{
    
    [Tooltip("将您想要在运行时手动控制的变量配置在这里")]
    public List<DebugVariable> controlledVariables = new List<DebugVariable>();

    void Update()
    {
        if (controlledVariables == null) return;

        // 遍历所有配置的变量，更新它们的状态并应用修改
        foreach (var variable in controlledVariables)
        {
            if (string.IsNullOrEmpty(variable.name))
            {
                variable.currentStatus = "变量名为空";
                continue;
            }

            // 1. 读取当前状态用于显示
            Lua.Result luaResult = DialogueLua.GetVariable(variable.name);

            // 根据配置的类型来格式化显示状态
            switch (variable.type)
            {
                case DebugVariable.VarType.Bool:
                    variable.currentStatus = luaResult.asBool.ToString();
                    break;
                case DebugVariable.VarType.Float:
                    variable.currentStatus = luaResult.asFloat.ToString("F2"); // 显示两位小数
                    break;
                case DebugVariable.VarType.String:
                    variable.currentStatus = $"\"{luaResult.asString}\""; // 给字符串加上引号
                    break;
            }
            

            // 2. 写入修改的值 (与之前的逻辑相同)
            switch (variable.type)
            {
                case DebugVariable.VarType.Bool:
                    DialogueLua.SetVariable(variable.name, variable.boolValue);
                    break;
                case DebugVariable.VarType.Float:
                    DialogueLua.SetVariable(variable.name, variable.floatValue);
                    break;
                case DebugVariable.VarType.String:
                    DialogueLua.SetVariable(variable.name, variable.stringValue);
                    break;
            }
        }
    }
}