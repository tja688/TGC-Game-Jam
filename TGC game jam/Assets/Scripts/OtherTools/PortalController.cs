using System;
using UnityEngine;

public class PortalController : MonoBehaviour
{
    [Header("传送门设置")]
    [Tooltip("将你要控制的传送门游戏对象拖到这里")]
    [SerializeField] private GameObject portalGameObject;

    // Awake 在 Start 之前执行，适合用来进行初始化和事件订阅
    private void Awake()
    {
        // 确保一开始传送门是关闭的
        if (portalGameObject)
        {
            portalGameObject.SetActive(false);
        }
        else
        {
            Debug.LogError("传送门对象 (portalGameObject) 未在 Inspector 中指定！", this);
        }
    }

    private void Start()
    {
        // --- 订阅事件 ---
        // 订阅“找到当天所有信件”事件，当事件触发时，调用 ActivatePortal 方法
        RubbishItem.OnFindAllLetters += ActivatePortal;

        // 订阅“天数变化”事件，当事件触发时，调用 DeactivatePortal 方法
        GameVariables.OnDayChanged += DeactivatePortal;

    }

    // 当这个控制器对象被销毁时，取消订阅以防止内存泄漏
    void OnDestroy()
    {
        // --- 取消订阅事件 ---
        RubbishItem.OnFindAllLetters -= ActivatePortal;
        GameVariables.OnDayChanged -= DeactivatePortal;
    }

    /// <summary>
    /// 激活传送门的方法
    /// </summary>
    private void ActivatePortal()
    {
        if (portalGameObject != null)
        {
            Debug.Log("接收到 'OnFindAllLettersForDay' 事件，正在激活传送门...");
            portalGameObject.SetActive(true);
        }
    }

    /// <summary>
    /// 关闭（失活）传送门的方法
    /// </summary>
    private void DeactivatePortal()
    {
        if (portalGameObject != null)
        {
            Debug.Log("接收到 'OnDayChanged' 事件，正在关闭传送门以待重新开启...");
            portalGameObject.SetActive(false);
        }
    }
}