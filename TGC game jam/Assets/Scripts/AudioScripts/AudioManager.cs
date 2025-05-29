using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Audio;

/// <summary>
/// 音频管理器单例，负责播放音效和管理 AudioSource 池。
/// </summary>
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Tooltip("AudioSource 对象池的初始大小")]
    [SerializeField] private int initialPoolSize = 10;

    private List<AudioSource> _audioSourcePool;
    private Dictionary<SoundEffect, AudioSource> _activeLoopingSounds; // 存储正在播放的循环音效

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 确保 AudioManager 在场景切换时不被销毁
            InitializeManager();
        }
        else
        {
            Destroy(gameObject); // 如果已存在实例，则销毁当前这个重复的
            return;
        }
    }

    private void InitializeManager()
    {
        _audioSourcePool = new List<AudioSource>(initialPoolSize);
        _activeLoopingSounds = new Dictionary<SoundEffect, AudioSource>();

        // 创建并初始化 AudioSource 池
        for (int i = 0; i < initialPoolSize; i++)
        {
            CreateAndPoolAudioSource();
        }
    }

    /// <summary>
    /// 创建一个新的 AudioSource 并将其添加到对象池中。
    /// </summary>
    private AudioSource CreateAndPoolAudioSource()
    {
        GameObject soundGameObject = new GameObject("PooledAudioSource_" + _audioSourcePool.Count);
        soundGameObject.transform.SetParent(transform); // 将 AudioSource GameObject 作为 AudioManager 的子对象，保持层级整洁
        AudioSource audioSource = soundGameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false; // 禁止在唤醒时自动播放
        soundGameObject.SetActive(false); // 初始状态设置为非激活，表示可用
        _audioSourcePool.Add(audioSource);
        return audioSource;
    }

    /// <summary>
    /// 从对象池中获取一个可用的 AudioSource。
    /// </summary>
    private AudioSource GetAvailableAudioSource()
    {
        // 查找池中是否有未激活的 AudioSource
        foreach (AudioSource source in _audioSourcePool)
        {
            if (!source.gameObject.activeInHierarchy) // GameObject 非激活表示 AudioSource 可用
            {
                return source;
            }
        }

        // 如果池中所有 AudioSource 都在使用中，则创建一个新的并添加到池中
        Debug.LogWarning("AudioManager: AudioSource 池已耗尽，正在创建新的 AudioSource。可以考虑增加 Initial Pool Size。");
        return CreateAndPoolAudioSource();
    }

    /// <summary>
    /// 播放指定的音效。
    /// </summary>
    /// <param name="sfx">要播放的 SoundEffect 资产。</param>
    public void Play(SoundEffect sfx)
    {
        if (sfx == null || sfx.clip == null)
        {
            Debug.LogError("AudioManager: 尝试播放的 SoundEffect 或其 AudioClip 为空。");
            return;
        }

        AudioSource sourceToPlay;

        if (sfx.loop)
        {
            // 如果是循环音效
            if (_activeLoopingSounds.TryGetValue(sfx, out AudioSource existingSource))
            {
                // 如果这个循环音效已在播放列表中，则使用现有的 AudioSource 并重新开始播放
                sourceToPlay = existingSource;
                sourceToPlay.Stop(); // 先停止当前的播放
            }
            else
            {
                // 否则，从池中获取一个新的 AudioSource，并将其添加到循环音效的跟踪列表
                sourceToPlay = GetAvailableAudioSource();
                _activeLoopingSounds[sfx] = sourceToPlay;
            }
        }
        else
        {
            // 如果是一次性音效，直接从池中获取 AudioSource
            sourceToPlay = GetAvailableAudioSource();
        }

        if (sourceToPlay == null) {
             Debug.LogError("AudioManager: 无法获取 AudioSource 来播放声音。");
             return;
        }

        // 配置并播放 AudioSource
        ConfigureAndPlay(sourceToPlay, sfx);

        // 如果不是循环音效，则在播放完毕后将其返回对象池
        if (!sfx.loop)
        {
            float duration = sfx.clip.length / (sfx.pitch <= 0.01f ? 0.01f : sfx.pitch); // 防止 pitch 为0或过小导致除零
            StartCoroutine(ReturnToPoolAfterDuration(sourceToPlay, sfx.clip, duration));
        }
    }

    /// <summary>
    /// 配置 AudioSource 并播放音效。
    /// </summary>
    private void ConfigureAndPlay(AudioSource source, SoundEffect sfx)
    {
        source.gameObject.SetActive(true); // 激活 AudioSource 所在的 GameObject
        source.clip = sfx.clip;
        source.volume = sfx.volume;
        source.pitch = sfx.pitch;
        source.loop = sfx.loop;
        source.outputAudioMixerGroup = sfx.outputAudioMixerGroup; // 设置混音器组
        source.Play();
    }

    /// <summary>
    /// 协程：在音效播放完毕后，将其关联的 AudioSource 返回到对象池。
    /// </summary>
    private IEnumerator ReturnToPoolAfterDuration(AudioSource source, AudioClip playedClip, float duration)
    {
        yield return new WaitForSeconds(duration);

        // 确保 AudioSource 仍然存在，并且播放的是同一个片段，且未被转为循环音效
        if (source != null && source.gameObject.activeInHierarchy && source.clip == playedClip && !source.loop)
        {
            source.Stop(); // 确保停止
            source.clip = null; // 清除 AudioClip 引用
            source.gameObject.SetActive(false); // 将 GameObject 设置为非激活，表示 AudioSource 可用
        }
        // 如果在此期间 source 被用于播放其他循环音效，则 Stop(SoundEffect) 方法会处理其停用。
    }

    /// <summary>
    /// 停止指定的循环音效。
    /// </summary>
    /// <param name="sfx">要停止的 SoundEffect 资产 (必须是循环音效)。</param>
    public void Stop(SoundEffect sfx)
    {
        if (sfx == null)
        {
            Debug.LogError("AudioManager: 尝试停止的 SoundEffect 为空。");
            return;
        }

        if (!sfx.loop)
        {
            // 对于非循环音效，它们会自动播放完毕并返回池中。
            // 如果需要停止特定的非循环音效实例，系统需要更复杂的跟踪机制。
            Debug.LogWarning($"AudioManager.Stop() 被调用于非循环音效: {sfx.name}。非循环音效通常会自动播放完毕。");
            return;
        }

        if (_activeLoopingSounds.TryGetValue(sfx, out AudioSource sourceToStop))
        {
            sourceToStop.Stop();
            sourceToStop.clip = null;
            sourceToStop.gameObject.SetActive(false); // 停用并返回池中
            _activeLoopingSounds.Remove(sfx); // 从活动循环音效列表中移除
        }
        else
        {
            Debug.LogWarning($"AudioManager.Stop(): 音效 {sfx.name} 未在活动的循环音效中找到。");
        }
    }

    // --- 可选的扩展功能 ---

    /// <summary>
    /// (可选) 通过代码设置 AudioMixer 中暴露的参数值（例如总音量、音乐音量等）。
    /// </summary>
    /// <param name="mixer">目标 AudioMixer。</param>
    /// <param name="exposedParamName">在 AudioMixer 中暴露的参数名称。</param>
    /// <param name="value">要设置的值 (通常为 0.0 到 1.0，会被转换为分贝)。</param>
    public void SetMixerVolume(AudioMixer mixer, string exposedParamName, float value)
    {
        if (mixer == null)
        {
            Debug.LogError("AudioManager: AudioMixer 为空。");
            return;
        }
        // 将线性值 (0-1) 转换为分贝 (-80dB 到 0dB)
        // Mathf.Log10(0) 是负无穷，所以用 Mathf.Clamp 避免错误
        float decibels = Mathf.Log10(Mathf.Clamp(value, 0.0001f, 1f)) * 20f;
        mixer.SetFloat(exposedParamName, decibels);
    }
}