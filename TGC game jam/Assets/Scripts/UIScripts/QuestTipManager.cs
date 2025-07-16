// using UnityEngine;
// using TMPro; // 确保导入 TextMeshPro 命名空间
// using System.Collections.Generic; // 用于 List
//
// /// <summary>
// /// 简易任务提示管理器（单例模式）
// /// 最多管理4个任务提示，支持按标签添加和移除，并自动重排序。
// /// </summary>
// public class QuestTipManager : MonoBehaviour
// {
//     public static QuestTipManager Instance { get; private set; }
//
//     public bool debugText;
//
//     
//     [Header("任务显示UI元素")]
//     [Tooltip("请按顺序（从上到下）拖拽4个TextMeshProUGUI组件到这里")]
//     [SerializeField]
//     private List<TextMeshProUGUI> taskDisplaySlots = new List<TextMeshProUGUI>(MaxTasksConst);
//
//     private const int MaxTasksConst = 4; // 最大任务数量
//
//     /// <summary>
//     /// 内部结构，用于存储单个任务的信息
//     /// </summary>
//     private struct ActiveTask
//     {
//         public string Tag { get; private set; }
//         public string Description { get; private set; }
//         public bool IsOccupied { get; private set; }
//         public TextMeshProUGUI DisplayElement { get; private set; }
//
//         public ActiveTask(TextMeshProUGUI displayElement)
//         {
//             DisplayElement = displayElement;
//             Tag = null;
//             Description = null;
//             IsOccupied = false;
//             ClearDisplay();
//         }
//
//         public void Assign(string tag, string description)
//         {
//             Tag = tag;
//             Description = description;
//             IsOccupied = true;
//             if (DisplayElement)
//             {
//                 DisplayElement.text = Description;
//             }
//         }
//
//         public void Clear()
//         {
//             Tag = null;
//             Description = null;
//             IsOccupied = false;
//             ClearDisplay();
//         }
//
//         private void ClearDisplay()
//         {
//             if (DisplayElement)
//             {
//                 DisplayElement.text = string.Empty;
//             }
//         }
//     }
//
//     private readonly ActiveTask[] currentTasks = new ActiveTask[MaxTasksConst];
//
//     private void Awake()
//     {
//         // 单例模式实现
//         if (!Instance)
//         {
//             Instance = this;
//             // DontDestroyOnLoad(gameObject); // 如果希望任务提示在场景切换时不被销毁，可以取消此行注释
//         }
//         else
//         {
//             Debug.LogWarning("QuestTipManager 已存在实例，销毁当前重复的实例。");
//             Destroy(gameObject);
//             return;
//         }
//
//         InitializeTaskSlots();
//     }
//
//     private void InitializeTaskSlots()
//     {
//         // 检查 TextMeshProUGUI 列表是否正确配置
//         if (taskDisplaySlots is not { Count: MaxTasksConst })
//         {
//             Debug.LogError($"QuestTipManager: 'Task Display Slots' 未正确配置。请在Inspector中关联 {MaxTasksConst} 个TextMeshProUGUI组件。脚本将禁用。");
//             enabled = false; // 禁用此脚本的功能
//             return;
//         }
//
//         for (var i = 0; i < MaxTasksConst; i++)
//         {
//             if (!taskDisplaySlots[i])
//             {
//                 Debug.LogError($"QuestTipManager: 'Task Display Slots' 中的索引 {i} 未赋值。脚本将禁用。");
//                 enabled = false;
//                 return;
//             }
//             currentTasks[i] = new ActiveTask(taskDisplaySlots[i]);
//         }
//     }
//
//     /// <summary>
//     /// 添加一个新的任务提示。
//     /// </summary>
//     /// <param name="taskTag">任务的唯一标签（用于后续识别和移除）。</param>
//     /// <param name="taskDescription">任务的描述文本。</param>
//     /// <returns>如果成功添加返回 true，如果任务列表已满或标签已存在则返回 false。</returns>
//     public bool AddTask(string taskTag, string taskDescription)
//     {
//         if (!enabled)
//         {
//             Debug.LogError("QuestTipManager 未初始化或已禁用，无法添加任务。");
//             return false;
//         }
//
//         if (string.IsNullOrEmpty(taskTag))
//         {
//             Debug.LogWarning("QuestTipManager: 尝试添加的任务标签为空，已忽略。");
//             return false;
//         }
//
//         // 检查标签是否已存在 (通常任务标签应唯一)
//         for (var i = 0; i < MaxTasksConst; i++)
//         {
//             if (!currentTasks[i].IsOccupied || currentTasks[i].Tag != taskTag) continue;
//             Debug.LogWarning($"QuestTipManager: 标签为 '{taskTag}' 的任务已存在，无法重复添加。");
//             return false;
//         }
//
//         // 寻找空闲的槽位来显示任务
//         for (var i = 0; i < MaxTasksConst; i++)
//         {
//             if (currentTasks[i].IsOccupied) continue;
//             currentTasks[i].Assign(taskTag, taskDescription);
//             return true;
//         }
//
//         Debug.LogError($"QuestTipManager: 任务列表已满（最多 {MaxTasksConst} 个任务）。无法添加标签为 '{taskTag}' 的新任务。");
//         return false;
//     }
//
//     /// <summary>
//     /// 根据任务标签完成（移除）一个任务，并重新排序任务列表。
//     /// </summary>
//     /// <param name="taskTag">要完成的任务的标签。</param>
//     /// <returns>如果成功找到并移除任务返回 true，否则返回 false。</returns>
//     public bool CompleteTask(string taskTag)
//     {
//         if (!enabled)
//         {
//             Debug.LogError("QuestTipManager 未初始化或已禁用，无法完成任务。");
//             return false;
//         }
//
//         if (string.IsNullOrEmpty(taskTag))
//         {
//             Debug.LogWarning("QuestTipManager: 尝试完成的任务标签为空，已忽略。");
//             return false;
//         }
//
//         var foundIndex = -1;
//         for (var i = 0; i < MaxTasksConst; i++)
//         {
//             if (!currentTasks[i].IsOccupied || currentTasks[i].Tag != taskTag) continue;
//             foundIndex = i;
//             break;
//         }
//
//         if (foundIndex != -1)
//         {
//             currentTasks[foundIndex].Clear(); // 清除任务数据和UI显示
//             Debug.Log($"QuestTipManager: 任务 '{taskTag}' 已完成并从槽位 {foundIndex} 移除。");
//             RearrangeTasks(); // 重新排序任务以填补空位
//             return true;
//         }
//         else
//         {
//             Debug.LogWarning($"QuestTipManager: 未找到标签为 '{taskTag}' 的任务以进行移除。");
//             return false;
//         }
//     }
//
//     /// <summary>
//     /// 重新排序任务列表，确保任务从上到下连续显示，不留空隙。
//     /// </summary>
//     private void RearrangeTasks()
//     {
//         // 从上到下遍历所有任务槽位
//         for (var i = 0; i < MaxTasksConst; i++)
//         {
//             // 如果当前槽位 (i) 是空的
//             if (currentTasks[i].IsOccupied) continue;
//             // 那么就查找它下方的第一个被占用的槽位 (j)
//             for (var j = i + 1; j < MaxTasksConst; j++)
//             {
//                 if (!currentTasks[j].IsOccupied) continue;
//                 // 将槽位 j 的任务移动到槽位 i
//                 currentTasks[i].Assign(currentTasks[j].Tag, currentTasks[j].Description);
//                 // 清空原来槽位 j 的任务
//                 currentTasks[j].Clear();
//                 // 槽位 i 已经被填充，跳出内层循环，继续检查下一个槽位 (i+1)
//                 break;
//             }
//         }
//     }
//     
//     private void Update()
//     {
//         if(!debugText) return;
//         
//         if (Input.GetKeyDown(KeyCode.F7))
//         {
//             QuestTipManager.Instance.AddTask($"Test","Test1 test Test1 testTest1 testTest1 testTest1 testTest1 test.");
//         }
//         
//         if (Input.GetKeyDown(KeyCode.F8))
//         {
//             QuestTipManager.Instance.CompleteTask($"Test");
//         }
//     }
//     
// }