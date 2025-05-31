using UnityEngine;

[RequireComponent(typeof(PlayerMove))]
public class PlayerMoveSound : MonoBehaviour
{
    public enum MovementSoundMode
    {
        Looping,          // 持续行走时播放循环音效 (使用我们之前实现的淡入淡出)
        IntervalOneShot   // 持续行走时按固定间隔播放一次性音效
    }

    [Header("播放模式")]
    [SerializeField] private MovementSoundMode soundMode = MovementSoundMode.Looping;

    [Header("循环音效设置 (若模式为 Looping)")]
    [Tooltip("玩家移动时循环播放的音效。请确保此 SoundEffect 资产的 Loop 属性设置为 true。")]
    [SerializeField] private SoundEffect moveLoopSound;
    private bool isLoopSoundCurrentlyPlaying = false; // 用于跟踪循环音效的播放状态

    [Header("间隔播放一次性音效设置 (若模式为 IntervalOneShot)")]
    [Tooltip("玩家移动时按间隔播放的一次性音效。其 Loop 属性应为 false。")]
    [SerializeField] private SoundEffect intervalOneShotSound;
    [Tooltip("播放一次性音效的间隔时间（秒）。")]
    [SerializeField] private float playInterval = 0.5f;
    private float intervalTimer = 0f; // 间隔计时器

    private PlayerMove playerMoveComponent;

    void Start()
    {
        playerMoveComponent = GetComponent<PlayerMove>();

        // 基本的 AudioManager 检查
        if (AudioManager.Instance == null)
        {
            Debug.LogError("PlayerMoveSound: AudioManager.Instance 未找到！请确保场景中存在 AudioManager。", this);
            enabled = false;
            return;
        }

        // 根据模式进行特定音效的检查
        if (soundMode == MovementSoundMode.Looping)
        {
            if (moveLoopSound == null)
            {
                Debug.LogError("PlayerMoveSound: 'moveLoopSound' 未在 Inspector 中分配 (Looping 模式)。", this);
                enabled = false;
                return;
            }
            if (!moveLoopSound.loop)
            {
                Debug.LogWarning("PlayerMoveSound: Looping 模式选中的 'moveLoopSound' (" + moveLoopSound.name + ") 未设置为循环。请检查设置。", this);
            }
        }
        else // IntervalOneShot 模式
        {
            if (intervalOneShotSound == null)
            {
                Debug.LogError("PlayerMoveSound: 'intervalOneShotSound' 未在 Inspector 中分配 (IntervalOneShot 模式)。", this);
                enabled = false;
                return;
            }
            if (intervalOneShotSound.loop)
            {
                Debug.LogWarning("PlayerMoveSound: IntervalOneShot 模式选中的 'intervalOneShotSound' (" + intervalOneShotSound.name + ") 被设置为循环。此模式应使用非循环音效。", this);
            }
            if (playInterval <= 0)
            {
                Debug.LogError("PlayerMoveSound: 'playInterval' 必须大于0 (IntervalOneShot 模式)。", this);
                playInterval = 0.5f; // 设置一个默认值避免问题
            }
        }
    }

    void Update()
    {
        if (!enabled || playerMoveComponent == null || AudioManager.Instance == null)
        {
            return;
        }

        bool isPlayerCurrentlyWalking = playerMoveComponent.IsWalking;

        if (soundMode == MovementSoundMode.Looping)
        {
            HandleLoopingSound(isPlayerCurrentlyWalking);
        }
        else // IntervalOneShot 模式
        {
            HandleIntervalOneShotSound(isPlayerCurrentlyWalking);
        }
    }

    private void HandleLoopingSound(bool isPlayerWalking)
    {
        if (moveLoopSound == null) return;

        if (isPlayerWalking && !isLoopSoundCurrentlyPlaying)
        {
            AudioManager.Instance.Play(moveLoopSound); // AudioManager 会处理淡入
            isLoopSoundCurrentlyPlaying = true;
        }
        else if (!isPlayerWalking && isLoopSoundCurrentlyPlaying)
        {
            AudioManager.Instance.Stop(moveLoopSound); // AudioManager 会处理淡出
            isLoopSoundCurrentlyPlaying = false;
        }
    }

    private void HandleIntervalOneShotSound(bool isPlayerWalking)
    {
        if (intervalOneShotSound == null) return;

        if (isPlayerWalking)
        {
            intervalTimer -= Time.deltaTime;
            if (intervalTimer <= 0f)
            {
                AudioManager.Instance.Play(intervalOneShotSound); // 播放一次性音效
                intervalTimer = playInterval; // 重置计时器
            }
        }
        else
        {
            // 当玩家停止时，你可能希望立即重置计时器，以便下次移动时能更快触发音效
            // 或者保持当前计时，让节奏在短暂停顿后继续。
            // 这里我们选择在停止时将计时器重置为0，以便下次移动立即触发（如果间隔允许）。
            intervalTimer = 0f;
        }
    }

    void OnDisable()
    {
        // 确保在组件禁用时停止正在播放的循环音效
        if (soundMode == MovementSoundMode.Looping && isLoopSoundCurrentlyPlaying)
        {
            if (AudioManager.Instance != null && moveLoopSound != null)
            {
                AudioManager.Instance.Stop(moveLoopSound);
            }
            isLoopSoundCurrentlyPlaying = false;
        }
        // 对于 IntervalOneShot 模式，通常不需要在 OnDisable 中特殊处理，
        // 因为它不是持续播放的音效。
    }
}