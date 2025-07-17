// BackpackCleanupManager.cs
using UnityEngine;
using System.Text.RegularExpressions; // 可以选用正则，但此处用简单字符串处理更高效

/// <summary>
/// 负责在天数变化时自动清理背包中过期物品的管理器。
/// </summary>
public class BackpackCleanupManager : MonoBehaviour
{
    private void Start()
    {
        // 在对象启用时，订阅GameVariables中的OnDayChanged事件
        // 这样，每当GameVariables中的天数变化时，我们的CleanupExpiredLetters方法就会被自动调用
        GameVariables.OnDayChanged += CleanupExpiredLetters;
        Debug.Log("BackpackCleanupManager 已启动并订阅每日清理事件。");
    }

    private void OnDestroy()
    {
        // 在对象禁用或销毁时，取消订阅，这是一个好习惯，可以防止内存泄漏
        GameVariables.OnDayChanged -= CleanupExpiredLetters;
        Debug.Log("BackpackCleanupManager 已停止并取消订阅每日清理事件。");
    }

    /// <summary>
    /// 清理背包中所有已过期的信件。
    /// </summary>
    private void CleanupExpiredLetters()
    {
        int currentDay = GameVariables.Day;
        Debug.Log($"新的一天开始了 (Day {currentDay})，开始检查并清理过期的信件...");

        var itemNamesInBackpack = BackpackManager.Instance.ItemNamesInBackpack;

        // 【重要】创建一个临时列表来存储需要销毁的物品名称
        // 这是因为不能在遍历一个列表的同时修改它（BackpackManager.DestroyItem会修改原始列表）
        var itemsToDestroy = new System.Collections.Generic.List<string>();

        foreach (string originalItemName in itemNamesInBackpack)
        {
            // --- 新增的逻辑：清理名称 ---
            string cleanName = originalItemName;
            if (cleanName.EndsWith("(Clone)"))
            {
                cleanName = cleanName.Substring(0, cleanName.Length - 7); // 7是"(Clone)"的长度
            }
            // --- 修改结束 ---

            // 使用清理后的 cleanName 进行后续所有判断
            if (cleanName.StartsWith("Letter") && cleanName.Length > 6 && cleanName[6] == '-')
            {
                char dayChar = cleanName[5];

                if (int.TryParse(dayChar.ToString(), out int letterEffectiveDay))
                {
                    if (currentDay > letterEffectiveDay)
                    {
                        Debug.Log($"信件 '{originalItemName}' 已过期 (有效天数: {letterEffectiveDay}, 当前天数: {currentDay})。计划将其移除。");
                        
                        // 不要在这里直接删除，而是添加到待删除列表
                        itemsToDestroy.Add(originalItemName);
                    }
                }
                else
                {
                    Debug.LogWarning($"无法从物品名称 '{cleanName}' 中解析出有效的天数。");
                }
            }
        }
        
        // --- 新增的逻辑：统一执行销毁 ---
        // 遍历待删除列表，执行真正的销毁操作
        foreach (string itemNameToDestroy in itemsToDestroy)
        {
            BackpackManager.Instance.DestroyItem(itemNameToDestroy);
        }
        // --- 修改结束 ---

        if (itemsToDestroy.Count > 0)
        {
            Debug.Log($"总共清理了 {itemsToDestroy.Count} 个过期信件。");
        }
        else
        {
            Debug.Log("没有发现需要清理的过期信件。");
        }
    }
    
}