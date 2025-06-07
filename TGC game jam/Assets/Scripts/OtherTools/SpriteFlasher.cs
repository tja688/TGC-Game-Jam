using UnityEngine;

/// <summary>
/// 控制 SpriteRenderer 组件实现周期性闪烁效果。
/// </summary>
[RequireComponent(typeof(SpriteRenderer))] // 确保该脚本的物体上一定有SpriteRenderer组件
public class SpriteFlasher : MonoBehaviour
{
    [Tooltip("闪烁的速度，值越大闪烁越快")]
    [SerializeField] 
    private float flashSpeed = 2.0f;

    [Tooltip("闪烁时能达到的最大透明度")]
    [Range(0f, 1f)] // 使用Range特性可以在Inspector中显示为滑动条，方便调节
    [SerializeField] 
    private float maxAlpha = 1.0f;

    private SpriteRenderer spriteRenderer;

    void Start()
    {
        // 获取附加在同一个游戏对象上的 SpriteRenderer 组件
        spriteRenderer = GetComponent<SpriteRenderer>();

        // 安全检查：如果找不到 SpriteRenderer 组件，则在控制台打印错误并禁用此脚本
        if (spriteRenderer == null)
        {
            Debug.LogError("SpriteFlasher 脚本需要一个 SpriteRenderer 组件！", this.gameObject);
            this.enabled = false; // 禁用此脚本以防止后续Update中报错
            return;
        }
    }

    void Update()
    {
        // 1. 使用 Time.time * flashSpeed 作为 Sin 函数的输入，使其随时间变化
        // Mathf.Sin() 的返回值在 [-1, 1] 区间内
        float sinValue = Mathf.Sin(Time.time * flashSpeed);

        // 2. 将 [-1, 1] 的范围映射到 [0, 1]
        // (sinValue + 1) -> [0, 2]
        // (sinValue + 1) / 2.0f -> [0, 1]
        float normalizedAlpha = (sinValue + 1) / 2.0f;

        // 3. 将 [0, 1] 的范围映射到 [0, maxAlpha]
        float targetAlpha = normalizedAlpha * maxAlpha;

        // 4. 获取当前的颜色
        Color currentColor = spriteRenderer.color;

        // 5. 只修改颜色的 alpha 值
        currentColor.a = targetAlpha;

        // 6. 将修改后的颜色应用回 SpriteRenderer
        spriteRenderer.color = currentColor;
    }
}