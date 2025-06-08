using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StorePortalController : MonoBehaviour
{
    [Header("传送门设置")]
    [Tooltip("将你要控制的传送门游戏对象拖到这里")]
    [SerializeField] private GameObject portalGameObject;

    private void Start()
    {
        GameVariables.OnDayChanged += DeactivatePortal;
    }

    // 当这个控制器对象被销毁时，取消订阅以防止内存泄漏
    void OnDestroy()
    {
        GameVariables.OnDayChanged -= DeactivatePortal;
    }


    /// <summary>
    /// 关闭（失活）传送门的方法
    /// </summary>
    private void DeactivatePortal()
    {
        if(GameVariables.Day != 2)
        {
            portalGameObject.SetActive(true);
        }
        else
        {
            Debug.Log("第二天事件，关闭影像店传送门以待重新开启...");

            portalGameObject.SetActive(false);
        }
    }
}