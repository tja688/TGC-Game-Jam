using UnityEngine;

// System.Collections 和 System.Collections.Generic 在这个脚本中不是必需的，可以移除
// using System.Collections;
// using System.Collections.Generic;

public class GirlController : MonoBehaviour
{
    [Header("核心对象引用")]
    [Tooltip("需要控制的女孩角色对象")]
    public GameObject girl;

    [Header("位置标记")]
    [Tooltip("第1-3天（以及非第4天的其他时间）女孩所处的位置")]
    public Transform hideoutTransform; // 位置1：隐藏点

    [Tooltip("第4天时，女孩出现的位置")]
    public Transform activeTransform;  // 位置2：活动点

    // 私有变量，用于缓存女孩身上的组件，提高性能
    private SpriteRenderer girlSpriteRenderer;
    private Collider2D girlCollider;
    private bool isInitialized = false; // 初始化标记，确保设置只进行一次

    private void Start()
    {
        // 1. 初始化并进行安全检查
        Initialize();

        // 2. 订阅天数变化事件
        GameVariables.OnDayChanged += OnDayChange;

        // 3. 立即根据当前天数设置一次女孩的初始状态
        //    这能确保游戏无论从第几天开始，状态都是正确的
        OnDayChange();
    }

    private void OnDestroy()
    {
  
        GameVariables.OnDayChanged -= OnDayChange;
    }

    // 初始化方法，用于获取组件和进行错误检查
    private void Initialize()
    {
        if (isInitialized) return;

        // 检查是否在Inspector中设置了必要的对象
        if (girl == null || hideoutTransform == null || activeTransform == null)
        {
            Debug.LogError("GirlController: 请在Inspector中指定 Girl 对象和两个 Transform 位置！", this);
            this.enabled = false; // 禁用此脚本以防止运行时错误
            return;
        }

        // 获取并缓存女孩的SpriteRenderer组件
        girlSpriteRenderer = girl.GetComponent<SpriteRenderer>();
        if (girlSpriteRenderer == null)
        {
            Debug.LogError("GirlController: 'girl' 对象上没有找到 SpriteRenderer 组件！", this);
            this.enabled = false;
            return;
        }

        // 获取并缓存女孩的Collider2D组件
        // 这个不是必须的，所以只给一个警告
        girlCollider = girl.GetComponent<Collider2D>();
        if (girlCollider == null)
        {
            Debug.LogWarning("GirlController: 'girl' 对象上没有找到 Collider2D 组件。如果不需要碰撞，可以忽略此消息。", this);
        }

        isInitialized = true;
    }

    // 当GameVariables中的天数改变时，此方法会被调用
    private void OnDayChange()
    {
        if (!isInitialized) return; // 如果还没初始化，则不执行任何操作

        // 核心判断逻辑
        if (GameVariables.Day == 4 || GameVariables.Day == 5)
        {
            // 第4天:
            // 1. 将女孩移动到活动位置
            girl.transform.SetPositionAndRotation(activeTransform.position, activeTransform.rotation);
            // 2. 启用她的精灵和碰撞盒，让她可见且可交互
            SetGirlComponents(true);
        }
        else
        {
            // 其他所有天 (包括第1-3天, 以及第5天之后):
            // 1. 将女孩移动到隐藏位置
            girl.transform.SetPositionAndRotation(hideoutTransform.position, hideoutTransform.rotation);
            // 2. 禁用她的精灵和碰撞盒，让她不可见且不可交互
            SetGirlComponents(false);
        }
    }

    // 一个辅助方法，用于统一设置组件的启用/禁用状态，让代码更整洁
    private void SetGirlComponents(bool isEnabled)
    {
        // 控制SpriteRenderer的可见性
        girlSpriteRenderer.enabled = isEnabled;

        // 如果存在碰撞盒，则控制其是否生效
        if (girlCollider != null)
        {
            girlCollider.enabled = isEnabled;
        }
    }
}