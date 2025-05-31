using UnityEngine;
using UnityEngine.EventSystems; // 引入事件系统命名空间
using UnityEngine.UI; // （可选）如果需要获取Button组件

public class UIButtonHoverSound : MonoBehaviour, IPointerEnterHandler // 实现IPointerEnterHandler接口
{
    [Header("悬停音效设置")]
    [Tooltip("当鼠标指针进入此按钮区域时播放的 SoundEffect 资产。")]
    [SerializeField] private SoundEffect hoverEnterSound;

    // （可选）如果你也想在鼠标离开时播放音效，可以取消下面的注释并实现 IPointerExitHandler
    // [Tooltip("当鼠标指针离开此按钮区域时播放的 SoundEffect 资产。")]
    // [SerializeField] private SoundEffect hoverExitSound;
    // public class UIButtonHoverSound : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler

    // 当鼠标指针进入UI元素的边界时，此方法会被调用
    public void OnPointerEnter(PointerEventData eventData)
    {
        // Debug.Log("鼠标进入: " + gameObject.name); // 用于测试
        PlayHoverEnterSound();
    }

    // （可选）当鼠标指针离开UI元素的边界时，此方法会被调用
    // public void OnPointerExit(PointerEventData eventData)
    // {
    //     Debug.Log("鼠标离开: " + gameObject.name); // 用于测试
    //     PlayHoverExitSound();
    // }

    private void PlayHoverEnterSound()
    {
        if (hoverEnterSound != null)
        {
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.Play(hoverEnterSound);
            }
            else
            {
                Debug.LogError("UIButtonHoverSound: AudioManager.Instance 未找到！无法播放悬停进入音效。", this);
            }
        }
        // else
        // {
        //     Debug.LogWarning("UIButtonHoverSound: 'hoverEnterSound' 未在 " + gameObject.name + " 的 Inspector 中分配。", this);
        // }
    }

    // （可选）播放离开音效的方法
    // private void PlayHoverExitSound()
    // {
    //     if (hoverExitSound != null)
    //     {
    //         if (AudioManager.Instance != null)
    //         {
    //             AudioManager.Instance.Play(hoverExitSound);
    //         }
    //         else
    //         {
    //             Debug.LogError("UIButtonHoverSound: AudioManager.Instance 未找到！无法播放悬停离开音效。", this);
    //         }
    //     }
    // }

    void Start()
    {
        // 确保这个 GameObject 可以接收到射线检测事件
        // 通常UI元素（如带有Image组件的Button）默认是开启Raycast Target的
        // 如果没有，你可能需要确保有一个Graphic组件（如Image, RawImage, Text）
        // 并且它的 Raycast Target 属性是勾选的。
        Graphic graphic = GetComponent<Graphic>();
        if (graphic == null || !graphic.raycastTarget)
        {
            // Debug.LogWarning("UIButtonHoverSound: " + gameObject.name + " 可能无法接收鼠标悬停事件，因为它没有Graphic组件或Raycast Target未开启。", this);
        }
    }
}