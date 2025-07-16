using UnityEngine;

// 这个组件依赖于AudioSource，所以我们强制要求它必须和AudioSource在同一个游戏对象上。
[RequireComponent(typeof(AudioSource))]
public class RandomAudioPlayer : MonoBehaviour
{
    // 在Inspector面板中设置音量的随机范围
    [Tooltip("最小音量")]
    [Range(0f, 1f)]
    public float minVolume = 0.8f;

    [Tooltip("最大音量")]
    [Range(0f, 1f)]
    public float maxVolume = 1.0f;

    // 在Inspector面板中设置音调的随机范围
    [Tooltip("最小音调")]
    [Range(0.1f, 3f)]
    public float minPitch = 0.95f;

    [Tooltip("最大音调")]
    [Range(0.1f, 3f)]
    public float maxPitch = 1.05f;

    private AudioSource audioSource;
    
    [Header("播放频率控制")]
    [Tooltip("每隔N个字符播放一次声音")]
    public int playEveryNCharacters = 2; // 新增：计数器间隔
    
    private int characterCounter = 0; // 新增：字符计数器


    void Start()
    {
        // 获取挂载在同一个游戏对象上的AudioSource组件
        audioSource = GetComponent<AudioSource>();
    }

    // 这是一个公开的方法，可以被UnityEvent（比如打字机的OnCharacter事件）调用
    public void PlayRandomizedSound()
    {
        if (audioSource.clip == null)
        {
            // 如果AudioSource上没有指定音频片段，则不执行任何操作
            Debug.LogWarning("RandomAudioPlayer: AudioSource has no clip assigned!");
            return;
        }
        
        characterCounter++;
        if (characterCounter % playEveryNCharacters != 0)
        {
            // 如果计数器取余不为0，则不播放
            return;
        }

        // 随机生成当前这次播放的音量和音调
        float randomVolume = Random.Range(minVolume, maxVolume);
        float randomPitch = Random.Range(minPitch, maxPitch);

        // 设置音源的音调
        audioSource.pitch = randomPitch;
        
        // 使用PlayOneShot播放音效。它允许音效重叠播放，非常适合快速的打字声。
        // PlayOneShot的第二个参数是音量缩放系数。
        audioSource.PlayOneShot(audioSource.clip, randomVolume);
    }
}