using UnityEngine;
using UnityEngine.Assertions; // 用于断言，可以按需保留或移除
using Febucci.UI.Core;      

[RequireComponent(typeof(TypewriterCore))] 
public class ManagedTextSoundPlayer : MonoBehaviour
{
    [Header("Sound Effects Configuration")]
    [Tooltip("播放下一个音效之前的最小延迟时间（秒）。")]
    [SerializeField, Min(0f)] 
    float minSoundDelay = 0.07f;

    [Tooltip("是否从音效数组中随机选择音效进行播放？如果为 false，则按顺序播放。")]
    [SerializeField] 
    bool randomSequence = false;
    
    [Tooltip("用于每个字符显示的 SoundEffect 资产数组。请确保这些 SoundEffect 资产的 loop 属性为 false。")]
    [SerializeField] 
    SoundEffect[] soundEffects = new SoundEffect[0]; 

    // 内部状态
    private float latestTimePlayed = -1f;
    private int currentSoundIndex = 0;
    private TypewriterCore typewriter;

    private void Start()
    {
        typewriter = GetComponent<TypewriterCore>();
        Assert.IsNotNull(typewriter, "ManagedTextSoundPlayer: 未在此 GameObject 上找到 TypewriterCore 组件。");

        if (soundEffects == null || soundEffects.Length == 0)
        {
            Debug.LogError("ManagedTextSoundPlayer: 'Sound Effects' 数组为空或未分配。将禁用此组件。", this);
            enabled = false; 
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

        typewriter.onCharacterVisible.AddListener(OnCharacterVisible);

        if (soundEffects.Length > 0)
        {
            currentSoundIndex = randomSequence ? Random.Range(0, soundEffects.Length) : 0;
        }
    }

    void OnDestroy()
    {
        if (typewriter != null)
        {
            typewriter.onCharacterVisible.RemoveListener(OnCharacterVisible);
        }
    }

    void OnCharacterVisible(char character) 
    {
        if (Time.time - latestTimePlayed < minSoundDelay)
        {
            return;
        }

        if (soundEffects.Length == 0)
        {
            return;
        }

        SoundEffect soundToPlay = soundEffects[currentSoundIndex];

        if (soundToPlay != null && AudioManager.Instance != null)
        {
            AudioManager.Instance.Play(soundToPlay);
        }
        else
        {
            if(soundToPlay == null) Debug.LogWarning("ManagedTextSoundPlayer: 选中的 SoundEffect 为空，无法播放。", this);
        }

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
                currentSoundIndex = 0; 
            }
        }
    }
}
