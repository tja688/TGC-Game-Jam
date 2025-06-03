// using UnityEngine;
// using System.Collections.Generic;
//
// /// <summary>
// /// 管理玩家移动权限的锁定。
// /// 其他系统可以请求锁定玩家的移动。只有当所有锁定都被释放后，玩家才能移动。
// /// 使用 HashSet 来存储唯一的请求者，确保每个请求者只能持有一个锁定。
// /// </summary>
// public class PlayerMovementBlocker : MonoBehaviour
// {
//     private static PlayerMovementBlocker _instance;
//     public static PlayerMovementBlocker Instance
//     {
//         get
//         {
//             if (_instance == null)
//             {
//                 // 尝试在场景中查找已存在的实例
//                 _instance = FindObjectOfType<PlayerMovementBlocker>();
//
//                 if (_instance == null)
//                 {
//                     // 如果不存在，则创建一个新的GameObject并附加此脚本
//                     GameObject singletonObject = new GameObject(nameof(PlayerMovementBlocker));
//                     _instance = singletonObject.AddComponent<PlayerMovementBlocker>();
//                     Debug.Log($"[{nameof(PlayerMovementBlocker)}] Instance created dynamically.");
//                 }
//             }
//             return _instance;
//         }
//     }
//
//     // 使用 HashSet 存储请求锁定的对象，确保唯一性
//     private readonly HashSet<object> _movementLockRequesters = new HashSet<object>();
//
//     /// <summary>
//     /// 获取玩家当前是否可以移动的状态。
//     /// 仅当没有任何系统请求锁定时，才为 true。
//     /// </summary>
//     public bool CanPlayerMove => _movementLockRequesters.Count == 0;
//
//     private void Awake()
//     {
//         // 实现单例模式，确保场景中只有一个 PlayerMovementBlocker 实例
//         if (_instance != null && _instance != this)
//         {
//             Debug.LogWarning($"[{nameof(PlayerMovementBlocker)}] Multiple instances detected. Destroying duplicate on '{gameObject.name}'. The active instance is on '{_instance.gameObject.name}'.");
//             Destroy(gameObject); // 销毁重复的实例
//             return;
//         }
//         _instance = this;
//
//         // 可选：如果希望此管理器在场景切换时不被销毁，取消下面这行注释
//         // DontDestroyOnLoad(gameObject);
//     }
//
//     /// <summary>
//     /// 请求锁定玩家的移动。
//     /// 只要至少有一个请求者持有有效的锁定，玩家就无法移动。
//     /// </summary>
//     /// <param name="requester">请求锁定的对象。用于识别锁定的来源，方便调试。</param>
//     public void RequestLockMovement(object requester)
//     {
//         if (requester == null)
//         {
//             Debug.LogWarning($"[{nameof(PlayerMovementBlocker)}] A null requester attempted to lock movement. Ignoring.");
//             return;
//         }
//
//         // HashSet.Add() 返回 true 如果元素被成功添加 (即之前不存在)
//         if (_movementLockRequesters.Add(requester))
//         {
//             // 使用 GetContextObject 来尝试获取 Unity 编辑器中可点击的上下文
//             Debug.Log($"[{nameof(PlayerMovementBlocker)}] Movement lock REQUESTED by: '{requester}'. Total locks: {_movementLockRequesters.Count}. Player CanMove: {CanPlayerMove}", GetContextObject(requester));
//         }
//         else
//         {
//             // 如果 requester 已经存在于 HashSet 中，Add() 会返回 false
//             Debug.LogWarning($"[{nameof(PlayerMovementBlocker)}] Requester '{requester}' attempted to lock movement AGAIN, but it already holds a lock. Total locks: {_movementLockRequesters.Count}.", GetContextObject(requester));
//         }
//     }
//
//     /// <summary>
//     /// 释放之前请求的移动锁定。
//     /// 如果这是最后一个活动的锁定，玩家将能够再次移动。
//     /// </summary>
//     /// <param name="requester">最初请求锁定的对象。</param>
//     public void ReleaseLockMovement(object requester)
//     {
//         if (requester == null)
//         {
//             Debug.LogWarning($"[{nameof(PlayerMovementBlocker)}] A null requester attempted to release a movement lock. Ignoring.");
//             return;
//         }
//
//         // HashSet.Remove() 返回 true 如果元素被成功移除
//         if (_movementLockRequesters.Remove(requester))
//         {
//             Debug.Log($"[{nameof(PlayerMovementBlocker)}] Movement lock RELEASED by: '{requester}'. Total locks: {_movementLockRequesters.Count}. Player CanMove: {CanPlayerMove}", GetContextObject(requester));
//         }
//         else
//         {
//             // 如果 requester 不存在于 HashSet 中，Remove() 会返回 false
//             Debug.LogWarning($"[{nameof(PlayerMovementBlocker)}] Requester '{requester}' attempted to release a movement lock, but it was NOT FOUND in the active requesters list. Total locks: {_movementLockRequesters.Count}.", GetContextObject(requester));
//         }
//     }
//
//     /// <summary>
//     /// 强制移除所有的移动锁定。请谨慎使用。
//     /// 这在调试或某些重置场景中可能有用。
//     /// </summary>
//     /// <param name="caller">可选参数，记录是谁调用了此方法。</param>
//     public void ClearAllLocks(object caller = null)
//     {
//         if (_movementLockRequesters.Count > 0)
//         {
//             string callerName = caller?.ToString() ?? "Unknown";
//             Debug.LogWarning($"[{nameof(PlayerMovementBlocker)}] Clearing ALL ({_movementLockRequesters.Count}) movement locks. Called by: '{callerName}'. Player CanMove will now be true.", GetContextObject(caller));
//             _movementLockRequesters.Clear();
//         }
//         else
//         {
//             // Debug.Log($"[{nameof(PlayerMovementBlocker)}] ClearAllLocks called, but there were no active locks.");
//         }
//     }
//
//     /// <summary>
//     /// 辅助方法，如果请求者是 UnityEngine.Object，则返回它作为 Debug.Log 的上下文。
//     /// 这样在编辑器中点击日志时可以高亮显示对应的 GameObject。
//     /// </summary>
//     private Object GetContextObject(object requester)
//     {
//         return requester is Object unityObject ? unityObject : null;
//     }
//
// #if UNITY_EDITOR || DEVELOPMENT_BUILD
//     /// <summary>
//     /// (仅编辑器/开发版本) 在 Inspector 中添加一个按钮来打印当前所有的锁定请求者。
//     /// </summary>
//     [ContextMenu("Log Current Movement Lock Requesters")]
//     private void LogCurrentRequestersForDebug()
//     {
//         if (_movementLockRequesters.Count == 0)
//         {
//             Debug.Log($"[{nameof(PlayerMovementBlocker)}] No active movement lock requesters.");
//             return;
//         }
//
//         System.Text.StringBuilder sb = new System.Text.StringBuilder();
//         sb.AppendLine($"[{nameof(PlayerMovementBlocker)}] Active movement lock requesters ({_movementLockRequesters.Count}):");
//         int index = 1;
//         foreach (var req in _movementLockRequesters)
//         {
//             string reqString = req.ToString();
//             string reqType = req.GetType().FullName;
//             sb.AppendLine($"  {index++}. Requester: '{reqString}' (Type: {reqType})");
//         }
//         Debug.Log(sb.ToString(), this); // 使用 this 作为上下文，点击日志会高亮 PlayerMovementBlocker 对象
//     }
// #endif
//
// }