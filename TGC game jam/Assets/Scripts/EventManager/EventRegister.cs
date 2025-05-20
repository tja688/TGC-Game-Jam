using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using Sirenix.OdinInspector;
 
#if UNITY_EDITOR
using UnityEditor;
#endif
 
[Serializable,HideReferenceObjectPicker]
public class EventRecord
{
    [HideInInspector]
    public string ScriptPath;
 
    [HideInInspector]
    public int Line;
 
    [ShowInInspector, HideLabel]
    public string Name;
 
#if UNITY_EDITOR
    [Button("$Name", ButtonSizes.Small), GUIColor(0.8f, 1f, 0.6f)]
    private void Open()
    {
        var mono = AssetDatabase.LoadAssetAtPath<MonoScript>(ScriptPath);
        if (mono != null)
            AssetDatabase.OpenAsset(mono, Line);
        else
            Debug.LogError($"无法加载脚本: {ScriptPath}");
    }
#endif
}
 
[CreateAssetMenu(fileName = "EventRegisterSettings", menuName = "MieMieFrameTools/EventCenter/EventRegister")]
public class EventRegister : SerializedScriptableObject
{
    [FoldoutGroup("事件注册信息", expanded: true)]
    [DictionaryDrawerSettings(KeyLabel = "监听脚本", ValueLabel = "事件列表", DisplayMode = DictionaryDisplayOptions.ExpandedFoldout)]
    public Dictionary<Type, List<EventRecord>> eventAddLisenerInfo = new Dictionary<Type, List<EventRecord>>();
 
    [FoldoutGroup("事件触发信息", expanded: true)]
    [DictionaryDrawerSettings(KeyLabel = "触发脚本", ValueLabel = "事件列表", DisplayMode = DictionaryDisplayOptions.ExpandedFoldout)]
    public Dictionary<Type, List<EventRecord>> eventTriggerInfo = new Dictionary<Type, List<EventRecord>>();
 
    [SerializeField, ReadOnly]
    private int totalScriptsScanned = 0;
 
    [SerializeField, ReadOnly]
    private float scanTime = 0f;
 
#if UNITY_EDITOR
    [Button("刷新事件记录", ButtonSizes.Large), GUIColor(0.4f, 0.8f, 1f)]
    public void RefreshEventRecord()
    {
        try
        {
            var startTime = DateTime.Now;
            EditorUtility.DisplayProgressBar("事件中心", "开始扫描脚本...", 0f);
 
            ClearEventRecord();
            ScanTargetScripts();
 
            scanTime = (float)(DateTime.Now - startTime).TotalSeconds;
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
 
            Debug.Log($"扫描完成! 处理 {totalScriptsScanned} 个脚本，耗时 {scanTime:F2}s");
        }
        finally
        {
            EditorUtility.ClearProgressBar();
        }
    }
 
    private void ScanTargetScripts()
    {
        string[] guids = AssetDatabase.FindAssets("t:Script");
        for (int i = 0; i < guids.Length; i++)
        {
            string path = AssetDatabase.GUIDToAssetPath(guids[i]);
            EditorUtility.DisplayProgressBar("扫描进度", $"处理: {Path.GetFileName(path)}", (float)i / guids.Length);
            ProcessScript(path);
        }
    }
 
    private void ProcessScript(string path)
    {
        try
        {
            MonoScript monoScript = AssetDatabase.LoadAssetAtPath<MonoScript>(path);
            if (monoScript == null) return;
 
            Type type = monoScript.GetClass();
            if (type == null)
            {
                Debug.LogWarning($"无法解析类型: {Path.GetFileName(path)}");
                return;
            }
 
            if (!IsTargetAssembly(type))
            {
                Debug.Log($"跳过程序集: {type.Assembly.GetName().Name} -> {type.FullName}");
                return;
            }
 
            string content = File.ReadAllText(path);
            FindEventCalls(content, type, path);
            totalScriptsScanned++;
        }
        catch (Exception e)
        {
            Debug.LogWarning($"解析失败 {path}: {e.Message}");
        }
    }
 
    private bool IsTargetAssembly(Type type)
    {
        string[] validAssemblies = { "Assembly-CSharp", "Assembly-CSharp-firstpass", "MyGameScripts" };
        return validAssemblies.Contains(type.Assembly.GetName().Name);
    }
 
    private void FindEventCalls(string content, Type type, string path)
    {
        const string eventPattern =
            @"EventCenter\.(AddEventListener|RemoveListener|TriggerEvent)\s*" +
            @"(?:<[^>]+>)?\s*\(\s*[^,]*?""([^""]+)""";
 
        var matches = Regex.Matches(content, eventPattern, RegexOptions.Singleline | RegexOptions.Compiled);
        foreach (Match match in matches)
        {
            string callType = match.Groups[1].Value;
            string evtName = match.Groups[2].Value.Trim();
            int line = GetLineNumber(content, match.Index);
 
            var targetDict = callType == "AddEventListener"
                ? eventAddLisenerInfo
                : callType == "TriggerEvent"
                    ? eventTriggerInfo
                    : null;
 
            if (targetDict != null)
            {
                AddToDictionary(targetDict, type, new EventRecord { Name = evtName, ScriptPath = path, Line = line });
            }
        }
    }
 
    private int GetLineNumber(string content, int index)
    {
        return content.Take(index).Count(c => c == '\n') + 1;
    }
 
    private void AddToDictionary(Dictionary<Type, List<EventRecord>> dict, Type type, EventRecord record)
    {
        if (string.IsNullOrEmpty(record.Name)) return;
        if (!dict.TryGetValue(type, out var list))
        {
            list = new List<EventRecord>();
            dict[type] = list;
        }
        if (!list.Any(r => r.Name == record.Name && r.ScriptPath == record.ScriptPath && r.Line == record.Line))
        {
            list.Add(record);
        }
    }
#endif
 
    [Button("清空记录", ButtonSizes.Large), GUIColor(1f, 0.6f, 0.6f)]
    public void ClearEventRecord()
    {
        eventAddLisenerInfo.Clear();
        eventTriggerInfo.Clear();
        totalScriptsScanned = 0;
        scanTime = 0f;
    }
}