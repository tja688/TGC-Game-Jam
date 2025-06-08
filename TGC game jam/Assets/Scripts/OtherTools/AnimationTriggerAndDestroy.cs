using UnityEngine;

public class AnimationTriggerAndDestroy : MonoBehaviour
{
    private static readonly int Start = Animator.StringToHash("Start");

    // 对Animator组件的引用
    private Animator animator;

    // 在对象被加载时调用
    void Awake()
    {
        // 获取并缓存该对象上的Animator组件
        // 这样做是为了性能，避免每次都去查找组件
        animator = GetComponent<Animator>();

        // 添加一个安全检查，如果找不到Animator组件，就在控制台报错
        if (animator == null)
        {
            Debug.LogError("在对象 " + gameObject.name + " 上没有找到Animator组件！", this);
        }
    }

    /// <summary>
    /// 这是提供给外部调用的公共接口，用于触发动画。
    /// </summary>
    public void TriggerAnimation()
    {
        if (animator != null)
        {
            // 触发名为 "Start" 的动画触发器
            animator.SetTrigger(Start);
        }
    }

    /// <summary>
    /// 这个公共方法将由动画事件在动画播放结束时调用。
    /// 注意：这个方法必须是public的，这样动画事件才能找到它。
    /// </summary>
    public void OnAnimationComplete()
    {
        // 销毁挂载此脚本的整个游戏对象
        Destroy(gameObject);
    }
}