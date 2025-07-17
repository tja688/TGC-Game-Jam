using UnityEngine;
using PixelCrushers.DialogueSystem;
using System.Collections.Generic;

/// <summary>
/// 一个稳定、可靠的单文件Dialogue System变量调试器。
/// 它能实时显示变量状态，并允许用户直接在Inspector中修改它们。
/// </summary>
public class LiveDialogueVarDebugger : MonoBehaviour
{
    [Tooltip("将您想要在运行时手动控制的变量配置在这里")]
    public List<LiveDebugVariable> variables = new List<LiveDebugVariable>();

    // 在每一帧更新
    void Update()
    {
        if (variables == null || variables.Count == 0)
        {
            return;
        }

        foreach (var v in variables)
        {
            if (string.IsNullOrEmpty(v.variableName))
            {
                v.currentStatus = "错误：变量名不能为空";
                continue;
            }

            // --- 步骤 1: 从Dialogue System读取真实值，并更新'currentStatus'字段 ---
            var luaResult = DialogueLua.GetVariable(v.variableName);

  
            // 根据配置的类型来格式化并显示当前状态
            switch (v.type)
            {
                case LiveDebugVariable.VarType.Bool:
                    v.currentStatus = $"当前值: {luaResult.asBool.ToString()}";
                    break;
                case LiveDebugVariable.VarType.Number:
                    // 修正: 读取时使用 asDouble 获取最精确的值
                    v.currentStatus = $"当前值: {luaResult.AsFloat.ToString("G")}"; // "G"为通用格式，能良好地显示整数和小数
                    break;
                case LiveDebugVariable.VarType.String:
                    v.currentStatus = $"当前值: \"{luaResult.asString}\"";
                    break;
            }
        

            // --- 步骤 2: 将“写入控制”区域的值写入Dialogue System ---
            switch (v.type)
            {
                case LiveDebugVariable.VarType.Bool:
                    DialogueLua.SetVariable(v.variableName, v.boolValue);
                    break;
                case LiveDebugVariable.VarType.Number:
                    // 修正: 写入时使用 double 类型的变量
                    DialogueLua.SetVariable(v.variableName, v.doubleValue);
                    break;
                case LiveDebugVariable.VarType.String:
                    DialogueLua.SetVariable(v.variableName, v.stringValue);
                    break;
            }
        }
    }
}