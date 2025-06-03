// // AnimationEventReceiver.cs
// using UnityEngine;
//
// public class AnimationEventReceiver : MonoBehaviour
// {
//
//     private string customLockID; // 可选的自定义锁ID
//
//     private object GetRequester()
//     {
//         // 如果设置了自定义ID，并且它不为空，则使用它
//         if (!string.IsNullOrEmpty(customLockID))
//         {
//             return customLockID;
//         }
//         // 否则，使用这个 MonoBehaviour 实例本身作为请求者
//         // 这确保了如果多个不同的对象使用相同的动画但有此脚本，它们是独立的请求者
//         return this;
//     }
//
//     // 这个公共方法将由动画事件调用以锁定移动
//     public void LockPlayerMovement()
//     {
//         if (PlayerMovementBlocker.Instance)
//         {
//             var requester = GetRequester();
//             PlayerMovementBlocker.Instance.RequestLockMovement(requester);
//         }
//         else
//         {
//             Debug.LogError($"[{gameObject.name}] PlayerMovementBlocker.Instance is null. Cannot lock movement via animation event.", this);
//         }
//     }
//
//     // 这个公共方法将由动画事件调用以解锁移动
//     public void UnlockPlayerMovement()
//     {
//         var requester = GetRequester();
//         if (PlayerMovementBlocker.Instance != null)
//         {
//             PlayerMovementBlocker.Instance.ReleaseLockMovement(requester);
//         }
//         else
//         {
//             Debug.LogError($"[{gameObject.name}] PlayerMovementBlocker.Instance is null. Cannot unlock movement via animation event.", this);
//         }
//     }
//
//     // --- 可选：如果你需要传递参数给Blocker (通常不需要) ---
//     // Unity的动画事件可以传递一个参数 (float, int, string, Object, or AnimationEvent)
//     // 如果你的PlayerMovementBlocker的Request/Release方法需要一个特定的字符串ID，
//     // 你也可以直接从动画事件传递这个ID，而不是在脚本中预设。
//
//     public void LockPlayerMovementWithID(string specificID)
//     {
//         if (PlayerMovementBlocker.Instance)
//         {
//             PlayerMovementBlocker.Instance.RequestLockMovement(specificID);
//         }
//         else
//         {
//             Debug.LogError($"[{gameObject.name}] PlayerMovementBlocker.Instance is null. Cannot lock movement with ID '{specificID}'.", this);
//         }
//     }
//
//     public void UnlockPlayerMovementWithID(string specificID)
//     {
//         if (PlayerMovementBlocker.Instance)
//         {
//             PlayerMovementBlocker.Instance.ReleaseLockMovement(specificID);
//         }
//         else
//         {
//             Debug.LogError($"[{gameObject.name}] PlayerMovementBlocker.Instance is null. Cannot unlock movement with ID '{specificID}'.", this);
//         }
//     }
// }