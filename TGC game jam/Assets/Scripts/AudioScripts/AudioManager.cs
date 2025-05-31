using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Audio; // 确保引入

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

    // --- 新增：Audio Mixer 控制相关 ---
    [Header("Audio Mixer Settings")]
    [Tooltip("在此处指定场景中的主 Audio Mixer")]
    public AudioMixer masterMixer; // 将你的 Audio Mixer 资源拖拽到 Inspector 中

    // Audio Mixer 中暴露的参数名称 (根据你的描述)
    private const string BACKGROUND_MIXER_PARAM = "BackGround";
    private const string SFX_MIXER_PARAM = "SFX";
    private const string UI_MIXER_PARAM = "UI";

    // PlayerPrefs 的键名
    private const string PREFS_BG_VOL = "AudioManager_BackgroundVolume";
    private const string PREFS_BG_MUTE = "AudioManager_BackgroundMute";
    private const string PREFS_SFX_VOL = "AudioManager_SFXVolume";
    private const string PREFS_SFX_MUTE = "AudioManager_SFXMute";
    private const string PREFS_UI_VOL = "AudioManager_UIVolume";
    private const string PREFS_UI_MUTE = "AudioManager_UIMute";

    // 存储各个通道的静音状态和静音前的线性音量值 (0.0001f - 1f)
    private bool _isBackgroundMuted = false;
    private float _lastBackgroundVolumeLinear = 0.75f; // 默认音量

    private bool _isSFXMuted = false;
    private float _lastSFXVolumeLinear = 0.75f;

    private bool _isUIMuted = false;
    private float _lastUIVolumeLinear = 0.75f;
    // --- 结束新增：Audio Mixer 控制相关 ---

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

        // --- 新增：加载并应用 Mixer 音量设置 ---
        if (masterMixer == null)
        {
            Debug.LogWarning("AudioManager: Master AudioMixer 未在 Inspector 中指定。音量控制功能将不可用。");
        }
        else
        {
            LoadAndApplyMixerSettings();
        }
        // --- 结束新增 ---
    }

    private AudioSource CreateAndPoolAudioSource()
    {
        GameObject soundGameObject = new GameObject("PooledAudioSource_" + _audioSourcePool.Count);
        soundGameObject.transform.SetParent(transform);
        AudioSource audioSource = soundGameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        soundGameObject.SetActive(false); // Start inactive
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
            source.outputAudioMixerGroup = null; // 重置 Mixer Group
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

        AudioClip clipToPlay = sfx.clip;
        float finalVolume = sfx.volume;
        float finalPitch = sfx.pitch;

        if (sfx.isRandomPool && sfx.audioClipPool != null && sfx.audioClipPool.Count > 0)
        {
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
            else if (sfx.clip == null)
            {
                 Debug.LogError($"AudioManager: SoundEffect '{sfx.name}' 配置为随机池但池为空/无效，且没有设置默认的 AudioClip。无法播放。");
                 return;
            }
        }
        else if (sfx.clip == null)
        {
            Debug.LogError($"AudioManager: SoundEffect '{sfx.name}' 的 AudioClip 为空。无法播放。");
            return;
        }
        
        if (clipToPlay == null) {
            Debug.LogError($"AudioManager: 无法为 SoundEffect '{sfx.name}' 确定有效的 AudioClip 进行播放。");
            return;
        }

        if (!sfx.loop && sfx.cooldown > 0.0f)
        {
            if (_soundEffectNextPlayTime.TryGetValue(sfx, out float nextPlayTime))
            {
                if (Time.time < nextPlayTime)
                {
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
                if (existingLoop.FadeCoroutine != null) StopCoroutine(existingLoop.FadeCoroutine);
                
                bool needsRestartOrReconfigure = existingLoop.Source.clip != clipToPlay || !existingLoop.Source.isPlaying;

                if (needsRestartOrReconfigure) {
                    ConfigureAudioSource(existingLoop.Source, sfx, clipToPlay, 0f, finalPitch);
                    existingLoop.Source.Play();
                }
                existingLoop.FadeCoroutine = StartCoroutine(FadeVolume(existingLoop.Source, sfx.loopFadeInTime, finalVolume, true));
            }
            else
            {
                sourceToPlay = GetAvailableAudioSource();
                if (sourceToPlay == null) return;

                ConfigureAudioSource(sourceToPlay, sfx, clipToPlay, 0f, finalPitch);
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
        else
        {
            sourceToPlay = GetAvailableAudioSource();
            if (sourceToPlay == null) return;

            ConfigureAudioSource(sourceToPlay, sfx, clipToPlay, finalVolume, finalPitch);
            sourceToPlay.Play();
            float duration = clipToPlay.length / Mathf.Max(0.01f, finalPitch);
            StartCoroutine(ReturnToPoolAfterDuration(sourceToPlay, clipToPlay, duration));
        }
    }

    private void ConfigureAudioSource(AudioSource source, SoundEffect sfx, AudioClip clip, float volume, float pitch)
    {
        source.clip = clip;
        source.volume = volume;
        source.pitch = pitch;
        source.loop = sfx.loop;
        source.outputAudioMixerGroup = sfx.outputAudioMixerGroup;
    }

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
                activeLoop.FadeCoroutine = StartCoroutine(FadeVolume(activeLoop.Source, sfx.loopFadeOutTime, 0f, false, () => {
                    ReturnAudioSourceToPool(activeLoop.Source);
                    _activeLoopingSounds.Remove(sfx);
                }));
            }
        }
        else
        {
            Debug.LogWarning($"AudioManager.Stop() 被调用于非循环音效: {sfx.name}。非循环音效通常会自动播放完毕。");
        }
    }

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
            if (targetVolume == 0 && !isFadingIn && audioSource.isPlaying) audioSource.Stop(); // 检查 isPlaying
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

        if (audioSource != null && audioSource.gameObject.activeInHierarchy)
        {
            audioSource.volume = targetVolume;
            if (targetVolume == 0 && !isFadingIn && audioSource.isPlaying) audioSource.Stop(); // 检查 isPlaying
        }
        onComplete?.Invoke();
    }

    private IEnumerator ReturnToPoolAfterDuration(AudioSource source, AudioClip playedClip, float duration)
    {
        float timeElapsed = 0f;
        while (timeElapsed < duration)
        {
            if (source == null || !source.gameObject.activeInHierarchy || !source.isPlaying)
            {
                ReturnAudioSourceToPool(source);
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

    // --- 音量混合器公共方法 ---
    // 此方法由你的新方法调用，用于设置实际的dB值
    public void SetMixerVolume(AudioMixer mixer, string exposedParamName, float linearValue)
    {
        if (mixer == null)
        {
            // Debug.LogError("AudioManager: AudioMixer 为空。"); // 在调用处处理此问题
            return;
        }
        // 确保 linearValue 在0.0001f到1f之间，以避免Log10(0)错误
        float clampedValue = Mathf.Clamp(linearValue, 0.0001f, 1f);
        float decibels = Mathf.Log10(clampedValue) * 20f;
        mixer.SetFloat(exposedParamName, decibels);
    }

    // --- 新增：加载并应用 Mixer 音量设置 ---
    private void LoadAndApplyMixerSettings()
    {
        // 背景音量
        _lastBackgroundVolumeLinear = PlayerPrefs.GetFloat(PREFS_BG_VOL, 0.75f);
        _isBackgroundMuted = PlayerPrefs.GetInt(PREFS_BG_MUTE, 0) == 1;
        if (_isBackgroundMuted)
        {
            SetMixerVolume(masterMixer, BACKGROUND_MIXER_PARAM, 0.0001f);
        }
        else
        {
            SetMixerVolume(masterMixer, BACKGROUND_MIXER_PARAM, _lastBackgroundVolumeLinear);
        }

        // SFX 音量
        _lastSFXVolumeLinear = PlayerPrefs.GetFloat(PREFS_SFX_VOL, 0.75f);
        _isSFXMuted = PlayerPrefs.GetInt(PREFS_SFX_MUTE, 0) == 1;
        if (_isSFXMuted)
        {
            SetMixerVolume(masterMixer, SFX_MIXER_PARAM, 0.0001f);
        }
        else
        {
            SetMixerVolume(masterMixer, SFX_MIXER_PARAM, _lastSFXVolumeLinear);
        }

        // UI 音量
        _lastUIVolumeLinear = PlayerPrefs.GetFloat(PREFS_UI_VOL, 0.75f);
        _isUIMuted = PlayerPrefs.GetInt(PREFS_UI_MUTE, 0) == 1;
        if (_isUIMuted)
        {
            SetMixerVolume(masterMixer, UI_MIXER_PARAM, 0.0001f);
        }
        else
        {
            SetMixerVolume(masterMixer, UI_MIXER_PARAM, _lastUIVolumeLinear);
        }
    }

    // --- 新增：供UI调用的公共方法 ---

    // 获取初始音量值 (用于UI Slider初始化)
    public float GetInitialBackgroundVolume() => _lastBackgroundVolumeLinear;
    public float GetInitialSFXVolume() => _lastSFXVolumeLinear;
    public float GetInitialUIVolume() => _lastUIVolumeLinear;

    // 获取初始静音状态 (用于UI Button初始化)
    public bool IsBackgroundMuted() => _isBackgroundMuted;
    public bool IsSFXMuted() => _isSFXMuted;
    public bool IsUIMuted() => _isUIMuted;


    // 背景音量控制 (由UI Slider调用)
    public void SetBackgroundVolumeSlider(float linearValue)
    {
        if (masterMixer == null) return;
        _lastBackgroundVolumeLinear = Mathf.Clamp(linearValue, 0.0001f, 1f); // 更新期望的音量
        if (!_isBackgroundMuted) // 如果没有被静音按钮静音，则实际更新Mixer
        {
            SetMixerVolume(masterMixer, BACKGROUND_MIXER_PARAM, _lastBackgroundVolumeLinear);
        }
        PlayerPrefs.SetFloat(PREFS_BG_VOL, _lastBackgroundVolumeLinear);
        PlayerPrefs.Save(); // 确保设置被保存
    }

    // 背景静音切换 (由UI Button调用)
    public void ToggleBackgroundMute()
    {
        if (masterMixer == null) return;
        _isBackgroundMuted = !_isBackgroundMuted;
        if (_isBackgroundMuted)
        {
            SetMixerVolume(masterMixer, BACKGROUND_MIXER_PARAM, 0.0001f); // 静音
        }
        else
        {
            SetMixerVolume(masterMixer, BACKGROUND_MIXER_PARAM, _lastBackgroundVolumeLinear); // 取消静音，恢复到滑块期望的音量
        }
        PlayerPrefs.SetInt(PREFS_BG_MUTE, _isBackgroundMuted ? 1 : 0);
        PlayerPrefs.Save();
    }

    // SFX 音量控制
    public void SetSFXVolumeSlider(float linearValue)
    {
        if (masterMixer == null) return;
        _lastSFXVolumeLinear = Mathf.Clamp(linearValue, 0.0001f, 1f);
        if (!_isSFXMuted)
        {
            SetMixerVolume(masterMixer, SFX_MIXER_PARAM, _lastSFXVolumeLinear);
        }
        PlayerPrefs.SetFloat(PREFS_SFX_VOL, _lastSFXVolumeLinear);
        PlayerPrefs.Save();
    }

    public void ToggleSFXMute()
    {
        if (masterMixer == null) return;
        _isSFXMuted = !_isSFXMuted;
        if (_isSFXMuted)
        {
            SetMixerVolume(masterMixer, SFX_MIXER_PARAM, 0.0001f);
        }
        else
        {
            SetMixerVolume(masterMixer, SFX_MIXER_PARAM, _lastSFXVolumeLinear);
        }
        PlayerPrefs.SetInt(PREFS_SFX_MUTE, _isSFXMuted ? 1 : 0);
        PlayerPrefs.Save();
    }

    // UI 音量控制
    public void SetUIVolumeSlider(float linearValue)
    {
        if (masterMixer == null) return;
        _lastUIVolumeLinear = Mathf.Clamp(linearValue, 0.0001f, 1f);
        if (!_isUIMuted)
        {
            SetMixerVolume(masterMixer, UI_MIXER_PARAM, _lastUIVolumeLinear);
        }
        PlayerPrefs.SetFloat(PREFS_UI_VOL, _lastUIVolumeLinear);
        PlayerPrefs.Save();
    }

    public void ToggleUIMute()
    {
        if (masterMixer == null) return;
        _isUIMuted = !_isUIMuted;
        if (_isUIMuted)
        {
            SetMixerVolume(masterMixer, UI_MIXER_PARAM, 0.0001f);
        }
        else
        {
            SetMixerVolume(masterMixer, UI_MIXER_PARAM, _lastUIVolumeLinear);
        }
        PlayerPrefs.SetInt(PREFS_UI_MUTE, _isUIMuted ? 1 : 0);
        PlayerPrefs.Save();
    }
    // --- 结束新增 ---
}