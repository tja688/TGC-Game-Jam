using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(DialogueVariableDebugger))]
public class DialogueVariableDebuggerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // 标记开始检查，这样Unity才知道是否有内容被修改
        serializedObject.Update(); 

        // 获取主脚本实例
        var debugger = (DialogueVariableDebugger)target;

        // 绘制列表
        var list = serializedObject.FindProperty("controlledVariables");
        EditorGUILayout.PropertyField(list, new GUIContent("受控变量列表"), true);

        // 如果列表被展开，我们就自定义绘制每个元素
        if (list.isExpanded)
        {
            EditorGUI.indentLevel++;

            for (int i = 0; i < list.arraySize; i++)
            {
                var element = list.GetArrayElementAtIndex(i);
                
                // 绘制一个带折叠箭头的标题
                element.isExpanded = EditorGUILayout.Foldout(element.isExpanded, $"元素 {i}: {debugger.controlledVariables[i].name}", true);

                if (element.isExpanded)
                {
                    EditorGUI.indentLevel++;
                    
                    var nameProp = element.FindPropertyRelative("name");
                    var typeProp = element.FindPropertyRelative("type");

                    EditorGUILayout.PropertyField(nameProp, new GUIContent("变量名"));
                    EditorGUILayout.PropertyField(typeProp, new GUIContent("变量类型"));

                    EditorGUILayout.Space();

                    // --- 状态显示区域 ---
                    GUIStyle statusStyle = new GUIStyle(EditorStyles.label);
                    statusStyle.fontStyle = FontStyle.Bold;
                    
                    // 如果变量未找到，用黄色突出显示
                    if (debugger.controlledVariables[i].currentStatus.Contains("Not Found"))
                    {
                        statusStyle.normal.textColor = Color.yellow;
                    }
                    
                    EditorGUILayout.LabelField("当前状态 (Current Status)", debugger.controlledVariables[i].currentStatus, statusStyle);
                    
                    EditorGUILayout.Space();
                    
                    // --- 写入控制区域 ---
                    EditorGUILayout.LabelField("写入控制", EditorStyles.boldLabel);

                    var typeEnum = (DebugVariable.VarType)typeProp.enumValueIndex;
                    switch (typeEnum)
                    {
                        case DebugVariable.VarType.Bool:
                            EditorGUILayout.PropertyField(element.FindPropertyRelative("boolValue"), new GUIContent("设置值"));
                            break;
                        case DebugVariable.VarType.Float:
                            EditorGUILayout.PropertyField(element.FindPropertyRelative("floatValue"), new GUIContent("设置值"));
                            break;
                        case DebugVariable.VarType.String:
                            EditorGUILayout.PropertyField(element.FindPropertyRelative("stringValue"), new GUIContent("设置值"));
                            break;
                    }
                    
                    EditorGUI.indentLevel--;
                    EditorGUILayout.Space(10); // 增加一些间距
                }
            }

            EditorGUI.indentLevel--;
        }

        // 应用所有修改
        serializedObject.ApplyModifiedProperties();

        // 在播放模式下强制重绘Inspector，以实时更新状态
        if (Application.isPlaying)
        {
            EditorUtility.SetDirty(target);
        }
    }
}