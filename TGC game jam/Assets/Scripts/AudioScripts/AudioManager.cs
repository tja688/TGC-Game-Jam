using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Tooltip("AudioSource 对象池的初始大小")]
    [SerializeField] private int initialPoolSize = 10;

    private List<AudioSource> _audioSourcePool;
    
    private class ActiveLoopingSound
    {
        public AudioSource Source;
        public Coroutine FadeCoroutine;
        public SoundEffect SfxAsset; 
    }
    private Dictionary<SoundEffect, ActiveLoopingSound> _activeLoopingSounds;
    private Dictionary<SoundEffect, float> _soundEffectNextPlayTime;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeManager();
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void InitializeManager()
    {
        _audioSourcePool = new List<AudioSource>(initialPoolSize);
        _activeLoopingSounds = new Dictionary<SoundEffect, ActiveLoopingSound>();
        _soundEffectNextPlayTime = new Dictionary<SoundEffect, float>();

        for (int i = 0; i < initialPoolSize; i++)
        {
            CreateAndPoolAudioSource();
        }
    }

    private AudioSource CreateAndPoolAudioSource()
    {
        GameObject soundGameObject = new GameObject("PooledAudioSource_" + _audioSourcePool.Count);
        soundGameObject.transform.SetParent(transform);
        AudioSource audioSource = soundGameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        soundGameObject.SetActive(false);
        _audioSourcePool.Add(audioSource);
        return audioSource;
    }

    private AudioSource GetAvailableAudioSource()
    {
        foreach (AudioSource source in _audioSourcePool)
        {
            if (!source.gameObject.activeInHierarchy)
            {
                source.gameObject.SetActive(true);
                return source;
            }
        }
        Debug.LogWarning("AudioManager: AudioSource 池已耗尽，正在创建新的 AudioSource。可以考虑增加 Initial Pool Size。");
        AudioSource newSource = CreateAndPoolAudioSource();
        newSource.gameObject.SetActive(true);
        return newSource;
    }
    
    private void ReturnAudioSourceToPool(AudioSource source)
    {
        if (source != null)
        {
            source.Stop();
            source.clip = null;
            source.loop = false;
            source.volume = 1f; 
            source.pitch = 1f;  
            source.gameObject.SetActive(false);
        }
    }

    public void Play(SoundEffect sfx)
    {
        if (sfx == null)
        {
            Debug.LogError("AudioManager: 尝试播放的 SoundEffect 为空。");
            return;
        }

        AudioClip clipToPlay = sfx.clip; // 默认或备用
        float finalVolume = sfx.volume;
        float finalPitch = sfx.pitch;

        // --- 处理随机池逻辑 ---
        if (sfx.isRandomPool && sfx.audioClipPool != null && sfx.audioClipPool.Count > 0)
        {
            // 过滤掉列表中可能的 null 元素
            List<AudioClip> validClips = new List<AudioClip>();
            for(int i=0; i < sfx.audioClipPool.Count; i++) {
                if (sfx.audioClipPool[i] != null) {
                    validClips.Add(sfx.audioClipPool[i]);
                } else {
                    Debug.LogWarning($"AudioManager: SoundEffect '{sfx.name}' 的随机池中索引 {i} 处的 AudioClip 为空，已跳过。");
                }
            }

            if (validClips.Count > 0)
            {
                clipToPlay = validClips[Random.Range(0, validClips.Count)];
                finalVolume = sfx.volume * Random.Range(sfx.randomVolumeMultiplierRange.x, sfx.randomVolumeMultiplierRange.y);
                finalPitch = sfx.pitch * Random.Range(sfx.randomPitchMultiplierRange.x, sfx.randomPitchMultiplierRange.y);
            }
            else if (sfx.clip == null) // 随机池为空或无效，且默认clip也为空
            {
                 Debug.LogError($"AudioManager: SoundEffect '{sfx.name}' 配置为随机池但池为空/无效，且没有设置默认的 AudioClip。无法播放。");
                 return;
            }
            // 如果随机池无效但 sfx.clip 有值，则会自动使用 sfx.clip (clipToPlay 的初始值)
        }
        else if (sfx.clip == null) // 非随机池模式，但默认clip为空
        {
            Debug.LogError($"AudioManager: SoundEffect '{sfx.name}' 的 AudioClip 为空。无法播放。");
            return;
        }
        // --- 结束随机池逻辑 ---
        
        // 最终检查 clipToPlay 是否有效
        if (clipToPlay == null) {
            Debug.LogError($"AudioManager: 无法为 SoundEffect '{sfx.name}' 确定有效的 AudioClip 进行播放。");
            return;
        }


        // --- 冷却时间检查 (作用于整个 SoundEffect 资产，无论是否随机池) ---
        if (!sfx.loop && sfx.cooldown > 0.0f)
        {
            if (_soundEffectNextPlayTime.TryGetValue(sfx, out float nextPlayTime))
            {
                if (Time.time < nextPlayTime)
                {
                    // Debug.Log($"AudioManager: SoundEffect {sfx.name} 尚在冷却中，跳过播放。");
                    return; 
                }
            }
            _soundEffectNextPlayTime[sfx] = Time.time + sfx.cooldown;
        }
        
        AudioSource sourceToPlay;

        if (sfx.loop)
        {
            if (_activeLoopingSounds.TryGetValue(sfx, out ActiveLoopingSound existingLoop))
            {
                // 如果同一个SoundEffect资产的循环已在播放
                if (existingLoop.FadeCoroutine != null) StopCoroutine(existingLoop.FadeCoroutine);

                // 检查当前播放的片段是否与新选中的片段不同 (主要针对随机池循环)
                // 或者音源是否已停止 (例如完全淡出后)
                bool needsRestartOrReconfigure = existingLoop.Source.clip != clipToPlay || !existingLoop.Source.isPlaying;

                if (needsRestartOrReconfigure) {
                    // 如果需要重新配置（例如随机到了不同的片段），则重新设置音源
                    ConfigureAudioSource(existingLoop.Source, sfx, clipToPlay, 0f, finalPitch); // 音量从0开始淡入
                    existingLoop.Source.Play();
                }
                // 总是启动/更新淡入到计算出的最终音量
                existingLoop.FadeCoroutine = StartCoroutine(FadeVolume(existingLoop.Source, sfx.loopFadeInTime, finalVolume, true));
            }
            else
            {
                sourceToPlay = GetAvailableAudioSource();
                if (sourceToPlay == null) return;
                
                ConfigureAudioSource(sourceToPlay, sfx, clipToPlay, 0f, finalPitch); // 音量从0开始淡入
                sourceToPlay.Play();

                ActiveLoopingSound newLoop = new ActiveLoopingSound
                {
                    Source = sourceToPlay,
                    SfxAsset = sfx
                };
                newLoop.FadeCoroutine = StartCoroutine(FadeVolume(sourceToPlay, sfx.loopFadeInTime, finalVolume, true));
                _activeLoopingSounds[sfx] = newLoop;
            }
        }
        else // 非循环音效 (一次性播放)
        {
            sourceToPlay = GetAvailableAudioSource();
            if (sourceToPlay == null) return;

            ConfigureAudioSource(sourceToPlay, sfx, clipToPlay, finalVolume, finalPitch);
            sourceToPlay.Play();
            // 注意：这里的 duration 是基于实际播放的 clipToPlay
            float duration = clipToPlay.length / Mathf.Max(0.01f, finalPitch); 
            StartCoroutine(ReturnToPoolAfterDuration(sourceToPlay, clipToPlay, duration));
        }
    }

    // ConfigureAudioSource 方法需要接收实际要播放的 clip, volume, 和 pitch
    private void ConfigureAudioSource(AudioSource source, SoundEffect sfx, AudioClip clip, float volume, float pitch)
    {
        source.clip = clip;
        source.volume = volume; // 应用计算后的最终音量
        source.pitch = pitch;   // 应用计算后的最终音高
        source.loop = sfx.loop; // loop 属性直接来自 SoundEffect 资产
        source.outputAudioMixerGroup = sfx.outputAudioMixerGroup;
    }

    // Stop 方法基本保持不变，它通过 SoundEffect 资产来管理循环音效的停止
    public void Stop(SoundEffect sfx)
    {
        if (sfx == null)
        {
            Debug.LogError("AudioManager: 尝试停止的 SoundEffect 为空。");
            return;
        }

        if (sfx.loop)
        {
            if (_activeLoopingSounds.TryGetValue(sfx, out ActiveLoopingSound activeLoop))
            {
                if (activeLoop.FadeCoroutine != null)
                {
                    StopCoroutine(activeLoop.FadeCoroutine);
                }
                // 启动淡出，完成后回收
                activeLoop.FadeCoroutine = StartCoroutine(FadeVolume(activeLoop.Source, sfx.loopFadeOutTime, 0f, false, () => {
                    ReturnAudioSourceToPool(activeLoop.Source);
                    _activeLoopingSounds.Remove(sfx); // 确保在回调中移除
                }));
            }
        }
        else
        {
            Debug.LogWarning($"AudioManager.Stop() 被调用于非循环音效: {sfx.name}。非循环音效通常会自动播放完毕。");
        }
    }

    // FadeVolume 协程保持不变
    private IEnumerator FadeVolume(AudioSource audioSource, float duration, float targetVolume, bool isFadingIn, System.Action onComplete = null)
    {
        if (audioSource == null || !audioSource.gameObject.activeInHierarchy || audioSource.clip == null)
        {
            onComplete?.Invoke();
            yield break;
        }
        
        float startVolume = audioSource.volume;
        float time = 0;

        if (duration <= 0)
        {
            audioSource.volume = targetVolume;
            if (targetVolume == 0 && !isFadingIn) audioSource.Stop();
            onComplete?.Invoke();
            yield break;
        }

        while (time < duration)
        {
             if (audioSource == null || !audioSource.gameObject.activeInHierarchy || audioSource.clip == null)
            {
                 onComplete?.Invoke();
                 yield break;
            }
            time += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(startVolume, targetVolume, time / duration);
            yield return null;
        }

        if (audioSource != null && audioSource.gameObject.activeInHierarchy) // 再次检查，因为协程可能在对象销毁后继续一帧
        {
            audioSource.volume = targetVolume;
            if (targetVolume == 0 && !isFadingIn) audioSource.Stop();
        }
        onComplete?.Invoke();
    }

    // ReturnToPoolAfterDuration 协程需要接收 AudioClip playedClip
    private IEnumerator ReturnToPoolAfterDuration(AudioSource source, AudioClip playedClip, float duration)
    {
        float timeElapsed = 0f;
        while (timeElapsed < duration)
        {
            if (source == null || !source.gameObject.activeInHierarchy || !source.isPlaying)
            {
                ReturnAudioSourceToPool(source); // 如果源在中途失效或停止，也尝试回收
                yield break;
            }
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        
        if (source != null && source.gameObject.activeInHierarchy && source.clip == playedClip && !source.loop) 
        {
            ReturnAudioSourceToPool(source);
        }
    }
    
    public void SetMixerVolume(AudioMixer mixer, string exposedParamName, float value)
    {
        if (mixer == null)
        {
            Debug.LogError("AudioManager: AudioMixer 为空。");
            return;
        }
        float decibels = Mathf.Log10(Mathf.Clamp(value, 0.0001f, 1f)) * 20f;
        mixer.SetFloat(exposedParamName, decibels);
    }
}