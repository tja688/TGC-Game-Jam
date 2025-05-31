using UnityEngine;
using UnityEngine.Audio;
using System.Collections.Generic; // 需要 List

[CreateAssetMenu(fileName = "NewSoundEffect", menuName = "Audio/Sound Effect")]
public class SoundEffect : ScriptableObject
{
    [Header("基本设置")]
    [Tooltip("默认的音频片段。如果不是随机池，或者随机池为空且此项已设置，则播放此片段。")]
    public AudioClip clip; // 保留，可作为非随机池或随机池为空时的备选项

    [Tooltip("音效的基础音量 (0.0 到 1.0)")]
    [Range(0f, 1f)]
    public float volume = 1f;

    [Tooltip("音效的基础音高 (0.1 到 3.0)")]
    [Range(0.1f, 3f)]
    public float pitch = 1f;

    [Tooltip("音效是否循环播放。对于随机池，如果勾选，则随机选一个片段进行循环。")]
    public bool loop = false;

    [Tooltip("此音效输出到的混音器组 (用于音量控制等)")]
    public AudioMixerGroup outputAudioMixerGroup;

    [Header("播放控制")]
    [Tooltip("此音效的最小播放间隔时间（秒）。如果设置为0，则没有冷却时间。仅对非循环音效有效。(对于随机池，冷却作用于整个池的触发)")]
    [Range(0f, 5f)]
    public float cooldown = 0.0f;

    [Header("循环音效淡入淡出 (仅当 Loop 为 true 时有效)")]
    [Tooltip("循环音效开始播放时的淡入时间（秒）。如果为0，则立即以完整音量播放。")]
    [Range(0f, 2f)]
    public float loopFadeInTime = 0.1f;

    [Tooltip("循环音效停止播放时的淡出时间（秒）。如果为0，则立即停止。")]
    [Range(0f, 2f)]
    public float loopFadeOutTime = 0.1f;

    // --- 新增随机池设置 ---
    [Header("随机音频池设置")]
    [Tooltip("是否启用随机池功能。若启用，播放时将从此列表随机选择一个音频片段。")]
    public bool isRandomPool = false;

    [Tooltip("用于随机播放的音频片段列表。仅当 isRandomPool 为 true 且列表不为空时使用。")]
    public List<AudioClip> audioClipPool = new List<AudioClip>();

    [Tooltip("随机音量乘数范围 [最小值, 最大值]。最终音量 = SoundEffect.volume * Random.Range(X, Y)。建议范围在0到2之间。")]
    public Vector2 randomVolumeMultiplierRange = new Vector2(1f, 1f); // 默认为1, 即不改变基础音量

    [Tooltip("随机音高乘数范围 [最小值, 最大值]。最终音高 = SoundEffect.pitch * Random.Range(X, Y)。建议范围在0.5到1.5之间。")]
    public Vector2 randomPitchMultiplierRange = new Vector2(1f, 1f); // 默认为1, 即不改变基础音高
    // --- 结束新增 ---
}