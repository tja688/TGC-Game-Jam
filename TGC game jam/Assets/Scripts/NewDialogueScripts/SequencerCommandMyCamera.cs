// using UnityEngine;
// using PixelCrushers.DialogueSystem; // 引入Dialogue System的核心命名空间
// using PixelCrushers.DialogueSystem.SequencerCommands; // 引入Sequencer Commands的命名空间
//
// // 指令的类名必须是 "SequencerCommand" + 您想在序列中使用的指令名称。
// // 例如，我们想使用 MyCamera(target)，类名就是 SequencerCommandMyCamera。
// public class SequencerCommandMyCamera : SequencerCommand
// {
//     private Transform commandTarget; // 用于存储此指令要移动到的目标
//
//     public void Start()
//     {
//         // --- 1. 获取并解析参数 ---
//         // GetParameter(0) 会获取指令括号里的第一个参数。
//         // 例如 MyCamera(CameraPoint1)，它就会得到字符串 "CameraPoint1"。
//         string targetName = GetParameter(0);
//
//         // --- 2. 查找目标对象 ---
//         // 使用 Dialogue System 的辅助方法来查找场景中的对象，这比 GameObject.Find 更健壮。
//         // 它能找到在 Dialogue Manager 的 "Transforms" 列表中注册的对象。
//         // 如果参数是 "null" 或 "speaker" 或 "listener"，它也能正确处理。
//         commandTarget = Sequencer.GetSubject(targetName);
//
//         // 如果传入的参数是 "null" 字符串或者找不到对象，并且我们确实是想恢复跟随玩家
//         if (string.Equals(targetName, "null", System.StringComparison.OrdinalIgnoreCase))
//         {
//             commandTarget = null;
//         }
//         else if (commandTarget == null)
//         {
//             // 如果目标名称不是"null"，但在场景中又找不到这个对象，就发出警告。
//             if (DialogueDebug.LogWarnings)
//             {
//                 Debug.LogWarning($"Dialogue System: Sequencer Command 'MyCamera' can't find target '{targetName}'.");
//             }
//             Stop(); // 停止此指令，让序列继续。
//             return;
//         }
//
//         // --- 3. 订阅事件并执行相机移动 ---
//         // 订阅您自定义的相机抵达事件。这是等待相机移动完成的关键。
//         CameraSystem.OnCameraArrivedAtSpecialTarget += OnCameraArrived;
//
//         // 调用您脚本的静态方法来开始移动相机
//         CameraSystem.SetSpecialCameraTarget(commandTarget);
//
//         // 如果目标是 null (恢复跟随玩家)，我们认为它“立即抵达”，直接触发完成。
//         if (commandTarget == null)
//         {
//             OnCameraArrived();
//         }
//     }
//
//     // 当 CameraSystem.OnCameraArrivedAtSpecialTarget 事件被触发时，此方法会被调用。
//     private void OnCameraArrived()
//     {
//         // --- 4. 清理并结束指令 ---
//         // 非常重要：取消订阅，防止内存泄漏或被其他指令的事件错误触发。
//         CameraSystem.OnCameraArrivedAtSpecialTarget -= OnCameraArrived;
//
//         // 调用 Stop() 方法告诉 Sequencer，这个指令已经执行完毕，可以继续执行下一个了。
//         Stop();
//     }
//
//     // 当 Sequencer 命令因为某些原因（如对话中断）被提前停止时，OnDestroy 会被调用。
//     // 在这里确保取消订阅事件，保证健壮性。
//     public void OnDestroy()
//     .
//         CameraSystem.OnCameraArrivedAtSpecialTarget -= OnCameraArrived;
//     }
// }