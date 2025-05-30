using UnityEngine;
using UnityEngine.UI;

public class DynamicButtonLayout : MonoBehaviour
{
    [Header("组件引用")]
    public HorizontalLayoutGroup horizontalLayoutGroup; // 你的ButtonsContent上的HorizontalLayoutGroup
    public RectTransform viewportRect;             // 你的ToolbarViewport的RectTransform
    public RectTransform contentRect;              // 你的ButtonsContent的RectTransform
    public ContentSizeFitter contentSizeFitter;    // 你的ButtonsContent上的ContentSizeFitter

    [Header("布局参数")]
    public float buttonPrefabWidth = 100f; // 单个按钮的预设宽度
    public float minSpacing = 10f;         // 当内容需要滚动时的最小固定间距
    public float maxSpacing = 50f;         // 当内容较少且适合视口时，允许的最大间距

    // 当按钮被添加或移除后，调用此方法
    public void UpdateLayout()
    {
        if (horizontalLayoutGroup == null || viewportRect == null || contentRect == null || contentSizeFitter == null)
        {
            Debug.LogError("DynamicButtonLayout: 请在Inspector中指定所有必需的组件引用!");
            return;
        }

        int buttonCount = contentRect.childCount;

        // 如果没有按钮，则进行一些清理或设置默认状态
        if (buttonCount == 0)
        {
            horizontalLayoutGroup.spacing = 0;
            // 根据需要，可以设置contentSizeFitter.horizontalFit为Unconstrained，并使contentRect宽度为0
            // contentSizeFitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
            // contentRect.sizeDelta = new Vector2(0, contentRect.sizeDelta.y);
            contentSizeFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize; // 或者让它根据padding计算最小宽度
            LayoutRebuilder.ForceRebuildLayoutImmediate(contentRect); // 强制立即刷新布局
            return;
        }

        // 计算所有按钮的总宽度 (不包括间距)
        float totalButtonsWidth = buttonCount * buttonPrefabWidth;
        // 获取视口的实际宽度
        float viewportWidth = viewportRect.rect.width;
        // 获取布局组的左右内边距总和
        float horizontalPadding = horizontalLayoutGroup.padding.left + horizontalLayoutGroup.padding.right;

        // 预估使用最小间距时内容的总宽度
        float totalWidthWithMinSpacing = totalButtonsWidth + ((buttonCount > 1) ? (buttonCount - 1) * minSpacing : 0) + horizontalPadding;

        if (totalWidthWithMinSpacing > viewportWidth)
        {
            // --- 情况1: 内容总宽度超出视口宽度，需要滚动 ---
            // 设置ContentSizeFitter使其根据内容自动调整宽度（允许滚动）
            contentSizeFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
            // 使用固定的最小间距
            horizontalLayoutGroup.spacing = minSpacing;
        }
        else
        {
            // --- 情况2: 内容总宽度小于或等于视口宽度 ---
            // 我们将手动设置Content的宽度为Viewport的宽度，并动态计算间距
            contentSizeFitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained; // 取消Fitter对宽度的自动控制
            contentRect.sizeDelta = new Vector2(viewportWidth, contentRect.sizeDelta.y); // 设置Content宽度等于Viewport宽度

            if (buttonCount > 1)
            {
                // 计算可用于间距的总空间
                float availableSpaceForSpacing = viewportWidth - totalButtonsWidth - horizontalPadding;
                // 计算理想的平均间距
                float calculatedSpacing = availableSpaceForSpacing / (buttonCount - 1);
                // 将间距限制在定义的minSpacing和maxSpacing之间
                horizontalLayoutGroup.spacing = Mathf.Clamp(calculatedSpacing, minSpacing, maxSpacing);
            }
            else // 只有一个按钮
            {
                horizontalLayoutGroup.spacing = 0; // 单个按钮不需要间距
                // 此时，单个按钮的对齐方式由HorizontalLayoutGroup的Child Alignment属性决定
                // 例如，如果Child Alignment是MiddleCenter，按钮会在contentRect（现在是viewport宽度）中居中。
            }
        }

        // 强制UI元素立即重新计算和应用布局更改
        // 在某些情况下，尤其是在同一帧内多次修改布局相关属性时，这是必要的。
        LayoutRebuilder.ForceRebuildLayoutImmediate(contentRect);
    }

    // 你可以在按钮被添加或移除后，从其他脚本调用这个公共方法
    // 例如:
    // DynamicButtonLayout layoutManager = FindObjectOfType<DynamicButtonLayout>();
    // if (layoutManager != null) layoutManager.UpdateLayout();
}