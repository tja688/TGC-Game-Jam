using UnityEngine;
using UnityEngine.Assertions; // 用于断言，可以按需保留或移除
using Febucci.UI.Core;      // 需要访问 TypewriterCore 来监听字符显示事件

[RequireComponent(typeof(TypewriterCore))] // 确保 GameObject 上有 TypewriterCore
public class ManagedTextSoundPlayer : MonoBehaviour
{
    [Header("Sound Effects Configuration")]
    [Tooltip("播放下一个音效之前的最小延迟时间（秒）。")]
    [SerializeField, Min(0f)] // 使用 UnityEngine.MinAttribute 来确保非负
    float minSoundDelay = 0.07f;

    [Tooltip("是否从音效数组中随机选择音效进行播放？如果为 false，则按顺序播放。")]
    [SerializeField] 
    bool randomSequence = false;
    
    [Tooltip("用于每个字符显示的 SoundEffect 资产数组。请确保这些 SoundEffect 资产的 loop 属性为 false。")]
    [SerializeField] 
    SoundEffect[] soundEffects = new SoundEffect[0]; // 注意这里使用的是 SoundEffect[]

    // 内部状态
    private float latestTimePlayed = -1f;
    private int currentSoundIndex = 0;
    private TypewriterCore typewriter;

    private void Start()
    {
        typewriter = GetComponent<TypewriterCore>();
        // 使用 Assert 或 Debug.LogError 进行必要的检查
        Assert.IsNotNull(typewriter, "ManagedTextSoundPlayer: 未在此 GameObject 上找到 TypewriterCore 组件。");

        if (soundEffects == null || soundEffects.Length == 0)
        {
            Debug.LogError("ManagedTextSoundPlayer: 'Sound Effects' 数组为空或未分配。将禁用此组件。", this);
            enabled = false; // 如果未正确配置则禁用组件
            return;
        }

        for(int i = 0; i < soundEffects.Length; i++)
        {
            if(soundEffects[i] == null)
            {
                Debug.LogError($"ManagedTextSoundPlayer: 'Sound Effects' 数组中索引 {i} 处的元素为空。将禁用此组件。", this);
                enabled = false;
                return;
            }
            // 推荐打字音效是非循环的
            if (soundEffects[i].loop)
            {
                Debug.LogWarning($"ManagedTextSoundPlayer: 音效 '{soundEffects[i].name}' (在数组索引 {i} 处) 被设置为循环播放。打字音效通常是非循环的。请检查设置是否符合预期。", soundEffects[i]);
            }
        }
        
        if (AudioManager.Instance == null)
        {
             Debug.LogError("ManagedTextSoundPlayer: AudioManager.Instance 未找到。请确保场景中已激活 AudioManager。将禁用此组件。", this);
             enabled = false;
             return;
        }

        // 订阅 TypewriterCore 的字符显示事件
        typewriter.onCharacterVisible.AddListener(OnCharacterVisible);

        // 初始化音效索引
        if (soundEffects.Length > 0) // 避免除以零或负数范围
        {
            currentSoundIndex = randomSequence ? Random.Range(0, soundEffects.Length) : 0;
        }
    }

    void OnDestroy()
    {
        // 组件销毁时取消订阅事件，防止内存泄漏
        if (typewriter != null)
        {
            typewriter.onCharacterVisible.RemoveListener(OnCharacterVisible);
        }
    }

    void OnCharacterVisible(char character) // TypewriterCore 会传递显示的字符，但我们这里用不到它
    {
        // 检查自上次播放以来是否已过去足够的时间
        if (Time.time - latestTimePlayed < minSoundDelay)
        {
            return; // 如果时间间隔太短，则不播放
        }

        // 再次确认音效列表不为空 (虽然 Awake 中已检查，但作为安全措施)
        if (soundEffects.Length == 0)
        {
            return;
        }

        // 选择要播放的 SoundEffect
        SoundEffect soundToPlay = soundEffects[currentSoundIndex];

        // 通过 AudioManager 播放音效
        if (soundToPlay != null && AudioManager.Instance != null)
        {
            AudioManager.Instance.Play(soundToPlay);
        }
        else
        {
            if(soundToPlay == null) Debug.LogWarning("ManagedTextSoundPlayer: 选中的 SoundEffect 为空，无法播放。", this);
            // AudioManager.Instance 为空的检查主要在 Awake 中处理
        }

        // 更新上次播放时间和下一个音效的索引
        latestTimePlayed = Time.time;

        if (randomSequence)
        {
            currentSoundIndex = Random.Range(0, soundEffects.Length);
        }
        else
        {
            currentSoundIndex++;
            if (currentSoundIndex >= soundEffects.Length)
            {
                currentSoundIndex = 0; // 如果是顺序播放，则循环回到开头
            }
        }
    }
}
