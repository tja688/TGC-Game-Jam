using UnityEngine;
using System;

public class CameraSystem : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [Tooltip("How close the camera needs to be to a special target to be considered 'arrived'.")]
    [SerializeField] private float arrivalThreshold = 0.1f;


    [Header("References")]
    // 尝试在 Start 中自动获取，或者允许外部拖拽赋值
    [SerializeField] private Transform initialPlayerTransform;


    private Transform currentPlayerToFollow;      // 当前应该跟随的玩家对象
    private Transform activeSpecialTarget;        // 当前激活的特殊目标点
    private bool hasFiredArrivalForThisSpecialTarget; // 标记当前特殊目标的抵达事件是否已触发

    /// <summary>
    /// 当相机抵达一个通过 SetSpecialCameraTarget 设置的特殊目标时触发。
    /// </summary>
    public static event Action OnCameraArrivedAtSpecialTarget;
    
    public static CameraSystem Instance { get; private set; }

    private void Awake()
    {
        if (Instance && Instance != this)
        {
            Debug.LogWarning("Another instance of CameraSystem found, destroying this one.");
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    private void Start()
    {
        if (initialPlayerTransform)
        {
            SetPlayerToFollow(initialPlayerTransform);
        }
        else if (PlayerMove.CurrentPlayer) // 尝试从 PlayerMove 获取
        {
            SetPlayerToFollow(PlayerMove.CurrentPlayer.transform);
        }
        else
        {
            Debug.LogWarning("CameraSystem: No initial player transform set and PlayerMove.CurrentPlayer is null. Camera might not follow player initially.");
        }
    }

    private void LateUpdate()
    {
        if (!currentPlayerToFollow && PlayerMove.CurrentPlayer)
        {
            SetPlayerToFollow(PlayerMove.CurrentPlayer.transform);
        }

        var targetToTrack = activeSpecialTarget ?? currentPlayerToFollow;

        if (!targetToTrack)
        {
            return;
        }

        var desiredCameraPosition = new Vector3(
            targetToTrack.position.x,
            targetToTrack.position.y,
            transform.position.z
        );

        // --- 相机移动逻辑 (已修复平滑性) ---
        var distanceToTarget = Vector3.Distance(transform.position, desiredCameraPosition);

        if (distanceToTarget > 0.001f) // 可以根据需要调整这个 epsilon 值
        {
            transform.position = Vector3.Lerp(
                transform.position,
                desiredCameraPosition,
                moveSpeed * Time.deltaTime);
        }

        if (!activeSpecialTarget || hasFiredArrivalForThisSpecialTarget) return;
        // 检查的是到 desiredCameraPosition 的距离，因为当 activeSpecialTarget 不为null时,
        // targetToTrack 就是 activeSpecialTarget, 所以 desiredCameraPosition 就是根据 activeSpecialTarget 计算的。
        if (!(Vector3.Distance(transform.position, desiredCameraPosition) < arrivalThreshold)) return;
        // 为了调试，可以取消下面的注释
        // Debug.Log($"CameraSystem: Arrived at special target: {activeSpecialTarget.name}");
        OnCameraArrivedAtSpecialTarget?.Invoke();
        hasFiredArrivalForThisSpecialTarget = true;
    }


    /// <summary>
    /// 供外部调用，设置一个临时的特殊相机目标点。
    /// 当相机抵达此目标时，OnCameraArrivedAtSpecialTarget 事件会触发一次。
    /// 若要相机重新检测并为同一目标再次触发事件，需再次调用此方法设置该目标。
    /// </summary>
    /// <param name="specialTarget">要临时看向的目标 Transform。传入 null 则清除特殊目标，恢复默认跟随玩家。</param>
    public static void SetSpecialCameraTarget(Transform specialTarget)
    {
        if (!Instance)
        {
            Debug.LogError("CameraSystem.Instance is null. Cannot set special camera target. Ensure a CameraSystem object exists in the scene.");
            return;
        }
        Instance.ProcessSpecialTargetSetting(specialTarget);
    }

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

    /// <summary>
    /// 设置或更新相机默认跟随的玩家对象。
    /// </summary>
    public void SetPlayerToFollow(Transform playerTransform)
    {
        currentPlayerToFollow = playerTransform ? playerTransform : null;
        // 当切换玩家时，如果当前没有特殊目标，确保特殊目标抵达逻辑不会干扰
        if (!activeSpecialTarget)
        {
            hasFiredArrivalForThisSpecialTarget = true; // 对于玩家跟随，事件触发标记应视为“已处理”
        }
    }
}