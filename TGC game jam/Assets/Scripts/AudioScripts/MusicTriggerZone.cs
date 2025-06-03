using System;
using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(BoxCollider2D))]
public class MusicTriggerZone : MonoBehaviour
{
    [Header("音乐设置")]
    public List<SoundEffect> musicList = new List<SoundEffect>();
    public int defaultMusicIndex = 0;

    private SoundEffect currentMusic;
    private int currentMusicIndex;

    // [Header("触发器设置")] // 不再需要手动跟踪这些状态
    // [SerializeField] private bool isPlayerInZone = false; // OnTriggerXXX 会处理
    // [SerializeField] private bool isMusicPlaying = false; // 播放状态由AudioManager和逻辑决定

    [Tooltip("在此处指定玩家的标签，用于区分进入触发器的对象")]
    public string playerTag = "Player"; // 或者通过比较组件类型

    private BoxCollider2D zoneCollider;

    private void Awake()
    {
        zoneCollider = GetComponent<BoxCollider2D>();
        zoneCollider.isTrigger = true; // 确保是触发器

        if (musicList.Count <= 0)
        {
            Debug.LogWarning($"MusicTriggerZone '{gameObject.name}' has no music in musicList.", this);
            return;
        }
        currentMusicIndex = Mathf.Clamp(defaultMusicIndex, 0, musicList.Count - 1);
        currentMusic = musicList[currentMusicIndex];
    }

    // 当其他碰撞体进入此触发器时调用
    private void OnTriggerEnter2D(Collider2D other)
    {
        // 检查进入的是否是玩家
        if (other.CompareTag(playerTag)) // 或者 other.GetComponent<PlayerController>() != null
        {
            // isPlayerInZone = true; // 状态现在由实际进入决定
            Debug.Log($"Player entered zone: {gameObject.name}");
            PlayCurrentMusic();
        }
    }

    // 当其他碰撞体离开此触发器时调用
    private void OnTriggerExit2D(Collider2D other)
    {
        // 检查离开的是否是玩家
        if (other.CompareTag(playerTag)) // 或者 other.GetComponent<PlayerController>() != null
        {
            // isPlayerInZone = false; // 状态现在由实际离开决定
            Debug.Log($"Player exited zone: {gameObject.name}");
            StopCurrentMusic();
        }
    }

    private void PlayCurrentMusic()
    {
        if (currentMusic == null)
        {
            Debug.LogWarning($"MusicTriggerZone '{gameObject.name}': currentMusic is null. Cannot play.", this);
            return;
        }
        // isMusicPlaying 状态可以由AudioManager内部管理，或者在这里判断是否重复播放
        // 为了简化，这里假设AudioManager.Instance.Play能处理重复播放请求（比如先停止再播放）
        AudioManager.Instance.Play(currentMusic);
        // isMusicPlaying = true; // 可以考虑移除，让AudioManager处理播放状态
    }

// In MusicTriggerZone.cs

    private void StopCurrentMusic()
    {
        if (currentMusic == null)
        {
            // Debug.LogWarning($"[{Time.frameCount}] MusicTriggerZone '{gameObject.name}': currentMusic is null, cannot stop.", this);
            return;
        }

        // 关键检查：确保 AudioManager 实例仍然存在且可用
        if (AudioManager.Instance != null)
        {
            // 检查 AudioManager 是否正在退出，如果它有这样的标志
            // if (AudioManager.Instance.IsApplicationQuitting()) return; // (需要 AudioManager 提供这个属性或方法)

            AudioManager.Instance.Stop(currentMusic);
            // isMusicPlaying = false; // 如果你还在使用这个标志位
        }
        else
        {
            // AudioManager 已经被销毁了，通常在场景切换或游戏退出时发生。
            // 在这种情况下，通常不需要再停止音乐，因为整个音频系统可能已经关闭。
            // Debug.LogWarning($"[{Time.frameCount}] MusicTriggerZone '{gameObject.name}': AudioManager instance is null. Cannot stop music. This is often normal during scene unload or app quit.", this);
        }
    }


    // ========== 外部接口 ==========

    public void SetNextMusicByIndex(int index)
    {
        if (musicList.Count == 0) return;

        index = Mathf.Clamp(index, 0, musicList.Count - 1);
        SoundEffect newMusic = musicList[index];

        // 如果玩家当前在此区域，则切换音乐
        // 需要一种方式判断玩家是否在此区域，如果移除了 isPlayerInZone，
        // 可以通过检查与玩家碰撞体的重叠状态，或者假设如果能调用这个，就应该切换
        bool playerIsCurrentlyInThisZone = IsPlayerStillInZone(); // 你需要实现这个

        if (currentMusic == newMusic && playerIsCurrentlyInThisZone) {
             // 如果是同一首音乐且正在播放，并且玩家还在区域内，则无需操作或根据需求重启
             return;
        }

        SoundEffect oldMusic = currentMusic;
        currentMusicIndex = index;
        currentMusic = newMusic;

        if (playerIsCurrentlyInThisZone)
        {
            if(oldMusic != null) AudioManager.Instance.Stop(oldMusic); // 停止旧音乐
            PlayCurrentMusic(); // 播放新音乐
        }
    }

    // 辅助方法：检查玩家是否仍在该区域 (仅当需要时实现)
    // 这可能需要你暂时跟踪进入的玩家对象，或者使用Physics2D.OverlapCollider
    private bool IsPlayerStillInZone()
    {
        // 简单的实现方式是，如果OnTriggerEnter2D被调用过且OnTriggerExit2D未被调用，则认为在区域内
        // 但由于你的SetNextMusicByIndex可能在任何时候被调用，你需要更可靠的方式
        // 一个简单（但不完全精确）的方法是再次检查包围盒，或者信任调用者的逻辑
        // 如果你的 Player 对象有一个特定的引用
        GameObject playerObject = GameObject.FindGameObjectWithTag(playerTag); // 或者通过其他方式获取玩家引用
        if (playerObject != null && zoneCollider.bounds.Intersects(playerObject.GetComponent<Collider2D>().bounds)) {
            return true;
        }
        return false; // 默认或如果玩家找不到
    }


    public void SetNextMusic(SoundEffect music)
    {
        if (!music) return;

        var index = musicList.IndexOf(music);
        if (index >= 0)
        {
            SetNextMusicByIndex(index);
        }
        else
        {
            // 如果不在列表中，直接设置
            Debug.LogWarning($"MusicTriggerZone '{gameObject.name}': Specified music '{music.name}' is not in the predefined list, but will be set as current.", this);

            bool playerIsCurrentlyInThisZone = IsPlayerStillInZone();
            SoundEffect oldMusic = currentMusic;
            currentMusic = music; // 更新 currentMusic，但不更新 index
            currentMusicIndex = -1; // 表示不在列表中

            if (playerIsCurrentlyInThisZone)
            {
                if(oldMusic != null) AudioManager.Instance.Stop(oldMusic);
                PlayCurrentMusic();
            }
        }
    }

    public int GetCurrentMusicIndex()
    {
        return currentMusicIndex;
    }

    public SoundEffect GetCurrentMusic()
    {
        return currentMusic;
    }

    public void ResetToDefaultMusic()
    {
        if (musicList.Count > 0)
        {
            SetNextMusicByIndex(defaultMusicIndex);
        }
    }

    // 如果你仍然需要传送后立即执行某些逻辑（除了区域检测）
    // 可以在 Start/OnDestroy 中保留 Portal 事件订阅，但其回调不应再负责主要的区域检测逻辑
    private void Start()
    {
        // Portal.OnTeleportationProcessHasFinished += HandleTeleportationFinished;
    }

    private void OnDestroy()
    {
        // Portal.OnTeleportationProcessHasFinished -= HandleTeleportationFinished;
    }

    // private void HandleTeleportationFinished(Transform teleportedTransform)
    // {
    //    if (teleportedTransform.CompareTag(playerTag))
    //    {
    //        // 传送完成后，Unity 的物理系统会自动处理 OnTriggerEnter/Exit
    //        // 你可能不需要在这里做额外的区域检查，除非有特殊逻辑
    //        // 例如，如果传送后音乐状态可能不一致，可以在这里强制同步
    //        // 但最好是依赖 OnTriggerEnter2D/Exit2D
    //    }
    // }
}