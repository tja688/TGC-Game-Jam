using UnityEngine;
using UnityEngine.Audio; // 需要引入这个命名空间来使用 AudioMixerGroup

/// <summary>
/// ScriptableObject 代表一个可配置的音效。
/// 你可以在项目中创建多个此类型的资产，每个代表一种声音。
/// </summary>
[CreateAssetMenu(fileName = "NewSoundEffect", menuName = "Audio/Sound Effect")]
public class SoundEffect : ScriptableObject
{
    [Tooltip("要播放的音频片段")]
    public AudioClip clip;

    [Tooltip("音效的基础音量 (0.0 到 1.0)")]
    [Range(0f, 1f)]
    public float volume = 1f;

    [Tooltip("音效的基础音高 (0.1 到 3.0)")]
    [Range(0.1f, 3f)]
    public float pitch = 1f;

    [Tooltip("音效是否循环播放")]
    public bool loop = false;

    [Tooltip("此音效输出到的混音器组 (用于音量控制等)")]
    public AudioMixerGroup outputAudioMixerGroup;
}