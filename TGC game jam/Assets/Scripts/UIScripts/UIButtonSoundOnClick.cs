using UnityEngine;
using UnityEngine.UI; // 需要访问 Button 组件
using UnityEngine.EventSystems; // (可选) 如果你想处理更复杂的事件，如 PointerDown

// [RequireComponent(typeof(Button))] // 如果你希望确保它总是挂在有Button的物体上
public class UIButtonSoundOnClick : MonoBehaviour //, IPointerDownHandler (可选)
{
    [Header("音效设置")]
    [Tooltip("按钮点击时播放的 SoundEffect 资产。请从项目文件夹拖拽到此处。")]
    [SerializeField] private SoundEffect clickSound;

    private Button buttonComponent;

    private void Start()
    {
        // 尝试获取Button组件并自动订阅onClick事件
        buttonComponent = GetComponent<Button>();
        if (buttonComponent != null)
        {
            buttonComponent.onClick.AddListener(PlayClickSound);
        }
        else
        {
            Debug.LogWarning("UIButtonSoundOnClick: 未在 " + gameObject.name + " 上找到 Button 组件。请确保此脚本挂载在有Button组件的对象上，或者手动调用 PlayClickSound()。", this);
        }

        if (clickSound == null)
        {
            Debug.LogWarning("UIButtonSoundOnClick: 'clickSound' 未在 " + gameObject.name + " 的 Inspector 中分配。", this);
        }
    }

    // 这个方法可以被 Button 的 OnClick() 事件直接调用，或者通过 Awake 中的 AddListener 调用
    public void PlayClickSound()
    {
        if (clickSound != null)
        {
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.Play(clickSound);
            }
            else
            {
                Debug.LogError("UIButtonSoundOnClick: AudioManager.Instance 未找到！无法播放声音。", this);
            }
        }
        else
        {
            // Debug.LogWarning("UIButtonSoundOnClick: clickSound 未设置，不播放声音。", this);
        }
    }

    // (可选) 如果你想在鼠标按下时就播放声音，而不是抬起时
    // public void OnPointerDown(PointerEventData eventData)
    // {
    //     PlayClickSound();
    // }

    void OnDestroy()
    {
        // 清理事件订阅，防止内存泄漏
        if (buttonComponent != null)
        {
            buttonComponent.onClick.RemoveListener(PlayClickSound);
        }
    }
}