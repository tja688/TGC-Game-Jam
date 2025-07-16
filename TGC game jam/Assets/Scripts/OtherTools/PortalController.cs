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

    /// <summary>
    /// 激活传送门的方法
    /// </summary>
    public void ActivatePortal()
    {
        if (portalGameObject != null)
        {
            portalGameObject.SetActive(true);
        }
    }
    
    public void DeactivatePortal()
    {
        if (portalGameObject != null)
        {
            portalGameObject.SetActive(false);
        }
    }
}