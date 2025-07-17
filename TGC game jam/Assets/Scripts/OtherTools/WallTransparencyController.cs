using UnityEngine;
using System.Collections; // 使用协程需要这个命名空间

public class WallTransparencyController : MonoBehaviour
{
    [Header("目标墙壁的精灵渲染器")]
    [Tooltip("将你希望变透明的墙壁对象拖到这里")]
    public SpriteRenderer wallSprite;

    [Header("透明度设置")]
    [Range(0, 1)]
    [Tooltip("玩家进入时，墙壁的目标透明度 (0=完全透明, 1=完全不透明)")]
    public float transparentAlpha = 0.5f;

    [Tooltip("墙壁恢复到的原始不透明度")]
    private float opaqueAlpha = 1.0f;

    [Header("渐变速度")]
    [Tooltip("透明度变化的快慢，值越大变化越快")]
    public float fadeSpeed = 5f;

    // 用一个计数器来处理多个碰撞体进入的情况，确保逻辑正确
    private int triggerEnterCount = 0;
    private Coroutine currentFadeCoroutine;

    void Awake()
    {
        if (wallSprite == null)
        {
            Debug.LogError("请在Inspector中指定墙壁的SpriteRenderer！", this);
            this.enabled = false; // 如果没有设置，禁用此脚本
            return;
        }
        // 保存墙壁原始的Alpha值
        opaqueAlpha = wallSprite.color.a;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 检查进入的是否是玩家
        if (other.CompareTag("Player"))
        {
            triggerEnterCount++;
            
            // 只有当第一个玩家碰撞体进入时，才开始渐变
            if (triggerEnterCount == 1)
            {
                // 如果当前有正在执行的协程，先停止它
                if (currentFadeCoroutine != null)
                {
                    StopCoroutine(currentFadeCoroutine);
                }
                // 开始渐变到透明
                currentFadeCoroutine = StartCoroutine(FadeTo(transparentAlpha));
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        // 检查离开的是否是玩家
        if (other.CompareTag("Player"))
        {
            triggerEnterCount--;

            // 只有当最后一个玩家碰撞体离开时，才恢复不透明
            if (triggerEnterCount <= 0)
            {
                triggerEnterCount = 0; // 防止计数器为负
                
                if (currentFadeCoroutine != null)
                {
                    StopCoroutine(currentFadeCoroutine);
                }
                // 开始渐变回不透明
                currentFadeCoroutine = StartCoroutine(FadeTo(opaqueAlpha));
            }
        }
    }

    /// <summary>
    /// 使用协程平滑地改变透明度
    /// </summary>
    /// <param name="targetAlpha">目标Alpha值</param>
    private IEnumerator FadeTo(float targetAlpha)
    {
        Color currentColor = wallSprite.color;
        
        // 当颜色Alpha值与目标值差距很小时，停止循环
        while (Mathf.Abs(currentColor.a - targetAlpha) > 0.01f)
        {
            // 使用Lerp进行平滑插值
            float newAlpha = Mathf.Lerp(currentColor.a, targetAlpha, Time.deltaTime * fadeSpeed);
            currentColor.a = newAlpha;
            wallSprite.color = currentColor;
            
            yield return null; // 等待下一帧
        }

        // 循环结束后，直接设置为目标值，确保精确
        currentColor.a = targetAlpha;
        wallSprite.color = currentColor;
        currentFadeCoroutine = null; // 协程结束
    }
}