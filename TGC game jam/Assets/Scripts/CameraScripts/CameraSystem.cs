using UnityEngine;
using System;
using Cysharp.Threading.Tasks; // 引入 UniTask 命名空间

public class CameraSystem : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float arrivalThreshold = 0.1f;

    [Header("References")]
    [SerializeField] private Transform initialPlayerTransform;

    private Transform currentPlayerToFollow;
    private Transform activeSpecialTarget;
    private bool hasFiredArrivalForThisSpecialTarget;

    // --- 1. 恢复原有的 event 系统，确保旧功能不受影响 ---
    /// <summary>
    /// 当相机抵达一个通过 SetSpecialCameraTarget 设置的特殊目标时触发。
    /// </summary>
    public static event Action OnCameraArrivedAtSpecialTarget;

    public static CameraSystem Instance { get; private set; }
    public float MoveSpeed { get => moveSpeed; set => moveSpeed = value; }

    // Awake 和 Start 方法保持不变
    private void Awake()
    {
        if (Instance && Instance != this) { Destroy(gameObject); }
        else { Instance = this; }
    }

    private void Start()
    {
        if (initialPlayerTransform) { SetPlayerToFollow(initialPlayerTransform); }
    }

    private void LateUpdate()
    {
        if (!currentPlayerToFollow && PlayerMove.CurrentPlayer)
        {
            SetPlayerToFollow(PlayerMove.CurrentPlayer.transform);
        }

        var targetToTrack = activeSpecialTarget ?? currentPlayerToFollow;
        if (!targetToTrack) return;

        var desiredPosition = new Vector3(targetToTrack.position.x, targetToTrack.position.y, transform.position.z);
        transform.position = Vector3.Lerp(transform.position, desiredPosition, moveSpeed * Time.deltaTime);

        if (activeSpecialTarget != null && !hasFiredArrivalForThisSpecialTarget)
        {
            if (Vector3.Distance(transform.position, desiredPosition) < arrivalThreshold)
            {
                // --- 2. 恢复 event 的触发调用 ---
                OnCameraArrivedAtSpecialTarget?.Invoke();
                hasFiredArrivalForThisSpecialTarget = true;
            }
        }
    }

    // --- 3. 新增的、基于 UniTask 的复合方法 ---
    /// <summary>
    /// 【新增】执行一个“发射后不管”的完整镜头调度：移至目标，等待，然后返回。
    /// </summary>
    public async UniTask PanAndReturnAsync(Transform temporaryTarget, float waitDuration)
    {
        // a. 设置目标
        SetSpecialCameraTarget(temporaryTarget);

        // b. 优雅地将 C# event 转换为可等待的 UniTask
        var arrivalTcs = new UniTaskCompletionSource();
        Action onArrival = null;
        onArrival = () => {
            OnCameraArrivedAtSpecialTarget -= onArrival; // 收到事件后立即注销，防止内存泄漏
            arrivalTcs.TrySetResult();
        };
        OnCameraArrivedAtSpecialTarget += onArrival;

        // 等待事件触发
        await arrivalTcs.Task;

        // c. 等待指定时间
        await UniTask.Delay(TimeSpan.FromSeconds(waitDuration), cancellationToken: this.GetCancellationTokenOnDestroy());

        // d. 返回玩家
        SetSpecialCameraTarget(PlayerMove.CurrentPlayer.transform);
    }

    // ProcessSpecialTargetSetting 和 SetPlayerToFollow 方法保持不变
    private void ProcessSpecialTargetSetting(Transform newSpecialTarget)
    {
        if (activeSpecialTarget == newSpecialTarget)
        {
            if (!newSpecialTarget || !hasFiredArrivalForThisSpecialTarget) return;
        }
        else
        {
            activeSpecialTarget = newSpecialTarget;
        }
        hasFiredArrivalForThisSpecialTarget = false;
    }

    public void SetPlayerToFollow(Transform playerTransform)
    {
        currentPlayerToFollow = playerTransform;
        if (!activeSpecialTarget)
        {
            hasFiredArrivalForThisSpecialTarget = true;
        }
    }
    
    // 静态 SetSpecialCameraTarget 方法保持不变
    public static void SetSpecialCameraTarget(Transform specialTarget)
    {
        if (Instance)
        {
            Instance.ProcessSpecialTargetSetting(specialTarget);
        }
    }
}