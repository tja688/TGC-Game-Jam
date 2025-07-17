// BackpackCleanupManager.cs (最终版)
using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 负责在天数变化时自动清理背包中过期物品的管理器。
/// 此版本直接操作BackpackManager中的GameObject列表，确保可靠性。
/// </summary>
public class BackpackCleanupManager : MonoBehaviour
{
    private void OnEnable()
    {
        GameVariables.OnDayChanged += CleanupAllLetters;
        Debug.Log("BackpackCleanupManager (最终版) 已启动并订阅每日清理事件。");
    }

    private void OnDisable()
    {
        GameVariables.OnDayChanged -= CleanupAllLetters;
        Debug.Log("BackpackCleanupManager (最终版) 已停止并取消订阅每日清理事件。");
    }

    /// <summary>
    /// 清理背包中所有名字包含"Letter"的物品。
    /// </summary>
    private void CleanupAllLetters()
    {
        int currentDay = GameVariables.Day;
        Debug.Log($"新的一天开始了 (Day {currentDay})，开始直接清理所有信件类物品...");

        // 1. 直接从BackpackManager获取当前所有物品的GameObject列表副本
        List<GameObject> currentItems = BackpackManager.Instance.GetCurrentItemObjects();
        
        // 2. 遍历这个列表，找出所有需要被销毁的信件
        foreach (GameObject itemObject in currentItems)
        {
            // 安全检查，防止列表里有空对象
            if (itemObject == null) continue;

            // 3. 核心判断：直接检查游戏对象的名字(包括(Clone)后缀)是否包含"Letter"
            //    这完全符合你的要求：“只要沾边就删”
            if (itemObject.name.Contains("Letter"))
            {
                Debug.Log($"发现信件类物品: '{itemObject.name}'。准备销毁。");

                // 4. 获取该物品的IStorable接口，以取得其在背包系统内的正式名称
                IStorable storable = itemObject.GetComponent<IStorable>();
                if (storable != null)
                {
                    // 5. 调用BackpackManager标准的DestroyItem方法来销毁它
                    //    这样可以确保背包的两个内部列表（物品对象和物品名字）都得到正确清理
                    BackpackManager.Instance.DestroyItem(storable.ItemName);
                }
                else
                {
                    // 如果物品没有IStorable接口，但名字又像信件，这里只做记录，并不会删除
                    Debug.LogWarning($"物品 '{itemObject.name}' 名字像信件，但它没有实现IStorable接口，无法通过背包系统销毁。");
                }
            }
        }
        
        Debug.Log("信件清理流程执行完毕。");
    }
}