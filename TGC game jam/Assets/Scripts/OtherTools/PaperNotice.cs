using UnityEngine;

// Cysharp.Threading.Tasks 在这个脚本里没有被使用，可以移除
// using Cysharp.Threading.Tasks;

public class PaperNotice : MonoBehaviour
{
    [Header("特殊剧情对象")]
    public GameObject paper;
    public GameObject ball;
    public GameObject door;

    [Header("每日激活的对象")]
    [Tooltip("请按顺序（Day1, Day2, Day3...）将对象拖入")]
    public GameObject[] dayObjects; // 使用数组统一管理每日对象

    private void Start()
    {
        // 订阅事件
        GameVariables.OnDayChanged += OnDayChange;
        
        // 游戏开始时，立即根据当前天数初始化一次所有对象的状态
        OnDayChange();
        
        paper.SetActive(false);
        ball.SetActive(false);
        door.SetActive(false);
    }

    private void OnDestroy()
    {
        // 在对象销毁时取消订阅，这是个好习惯，可以防止内存泄漏
        GameVariables.OnDayChanged -= OnDayChange;
    }

    /// <summary>
    /// 当天数变化时调用的核心方法
    /// </summary>
    private void OnDayChange()
    {
        // 1. 更新每日专属对象的显示状态
        UpdateActiveDayObject();
        
        // 2. 处理其他特殊物品的逻辑（这部分逻辑保持和你原来的一样）
        switch (GameVariables.Day)
        {
            case 2:
                door.SetActive(true);
                paper.SetActive(true);
                break;
            case 3:
                door.SetActive(false);
                ball.SetActive(true);
                break;
        }
    }
    
    /// <summary>
    /// 核心功能：根据当前天数，只激活对应的每日对象，并禁用所有其他的
    /// </summary>
    private void UpdateActiveDayObject()
    {
        // 天数通常从1开始，而数组索引从0开始，所以需要减1
        int currentDayIndex = GameVariables.Day - 1;

        // 遍历所有每日对象
        for (int i = 0; i < dayObjects.Length; i++)
        {
            // 确保数组中的对象不为空，避免报错
            if (dayObjects[i] != null)
            {
                // 如果当前遍历的索引 (i) 与今天的索引 (currentDayIndex) 匹配，则激活它
                // 否则，就禁用它。这是一个非常高效的写法。
                dayObjects[i].SetActive(i == currentDayIndex);
            }
        }
    }
}