using System;
using UnityEngine;
using Cysharp.Threading.Tasks;

public class PlayerMove : MonoBehaviour
{
    // Animator Parameter Hashes
    private static readonly int HorizontalAnimHash = Animator.StringToHash("InputX");
    private static readonly int VerticalAnimHash = Animator.StringToHash("InputY");
    private static readonly int SpeedAnimHash = Animator.StringToHash("Speed");
    private static readonly int WakeUpAnimHash = Animator.StringToHash("WakeUp");
    private static readonly int SleepAnimHash = Animator.StringToHash("Sleep");
    private static readonly int SendLetterAnimHash = Animator.StringToHash("SendLetter");

    public static GameObject CurrentPlayer { get; private set; }
    public static bool CanPlayerMove { get; set; }

    public bool IsWalking { get; private set; }

    private Animator animator;
    private Rigidbody2D rigidbody2d;
    private InputActions inputActions;

    public float stopThreshold = 0.1f;
    public float moveSpeed = 1f;

    private Vector2 currentAnimationDirection;
    private Vector2 targetAnimationDirection;
    public float turnSmoothTime = 0.05f;
    private Vector2 turnAnimationVelocity;

    private float lastValidInputX = 0f;
    private float lastValidInputY = -1f; // 默认朝向下方

    private Vector2 movementVelocityForFixedUpdate;

    private bool isUnderSystemControl = false;
    
    public SoundEffect sendLetterSound;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        rigidbody2d = GetComponent<Rigidbody2D>();
        CurrentPlayer = gameObject;
        CanPlayerMove = true; // 游戏开始时默认可以移动
        IsWalking = false;

        // 初始化朝向和动画参数
        targetAnimationDirection = new Vector2(lastValidInputX, lastValidInputY).normalized;
        currentAnimationDirection = targetAnimationDirection;
        UpdateAnimatorParameters(currentAnimationDirection, 0f);
    }

    private void Start()
    {
        inputActions = PlayerInputController.Instance.InputActions;
        if (inputActions == null)
        {
            Debug.LogError("PlayerMove : inputActions is null. Ensure PlayerInputController is initialized.");
        }

        // 注册事件监听
        EventCenter.AddEventListener(GameEvents.GameStartsPlayerWakesUp, OnGameStartsPlayerWakesUp);
        EventCenter.AddEventListener(GameEvents.PlayerWakesUp, OnPlayerWakesUp);
        EventCenter.AddEventListener(GameEvents.PlayerSleep, OnPlayerSleep);
    }

    private void Update()
    {
        if (isUnderSystemControl) return;

        var inputVector = Vector2.zero;

        if (CanPlayerMove && inputActions != null)
        {
            inputVector = inputActions.PlayerControl.Move.ReadValue<Vector2>();
        }

        var horizontalInput = inputVector.x;
        var verticalInput = inputVector.y;

        var clampedHorizontal = Mathf.Abs(horizontalInput) < stopThreshold ? 0 : horizontalInput;
        var clampedVertical = Mathf.Abs(verticalInput) < stopThreshold ? 0 : verticalInput;

        var currentRawInputDirection = new Vector2(clampedHorizontal, clampedVertical);
        var currentRawSpeed = currentRawInputDirection.magnitude;

        IsWalking = currentRawSpeed > stopThreshold;

        if (IsWalking)
        {
            targetAnimationDirection = currentRawInputDirection.normalized;
            lastValidInputX = targetAnimationDirection.x;
            lastValidInputY = targetAnimationDirection.y;
        }
        else
        {
            targetAnimationDirection = new Vector2(lastValidInputX, lastValidInputY).normalized;
        }
        
        if (targetAnimationDirection.sqrMagnitude > 0.001f)
        {
            currentAnimationDirection = Vector2.SmoothDamp(currentAnimationDirection, targetAnimationDirection, ref turnAnimationVelocity, turnSmoothTime);
        }
        
        UpdateAnimatorParameters(currentAnimationDirection, currentRawSpeed);

        if (!CanPlayerMove)
        {
            movementVelocityForFixedUpdate = Vector2.zero;
            IsWalking = false;
        }
        else
        {
            movementVelocityForFixedUpdate = currentRawInputDirection.normalized * (moveSpeed * Mathf.Clamp01(currentRawSpeed));
        }
    }

    private void FixedUpdate()
    {
        if (rigidbody2d)
        {
            rigidbody2d.MovePosition(rigidbody2d.position + movementVelocityForFixedUpdate * Time.fixedDeltaTime);
        }
    }

    // --- 事件处理方法 ---
    private void OnGameStartsPlayerWakesUp()
    {
        CanPlayerMove = true;
        if (animator) animator.SetTrigger(WakeUpAnimHash);
    }

    private async void OnPlayerSleep()
    {
        CanPlayerMove = false;
        await Sleep();
    }
    
    private async void OnPlayerWakesUp()
    {
        await WakesUp();
        CanPlayerMove = true;
    }

    private async UniTask WakesUp()
    {
        ScreenFadeController.Instance.BeginFadeToClear(3f);
        await UniTask.WaitForSeconds(0.5f);
        if (animator) animator.SetTrigger(WakeUpAnimHash);
    }

    private async UniTask Sleep()
    {
        ScreenFadeController.Instance.BeginFadeToBlack(3f);
        await UniTask.WaitForSeconds(0.5f);
        if (animator) animator.SetTrigger(SleepAnimHash);
    }

    private void UpdateAnimatorParameters(Vector2 direction, float speed)
    {
        if (!animator) return;
        animator.SetFloat(HorizontalAnimHash, direction.x);
        animator.SetFloat(VerticalAnimHash, direction.y);
        animator.SetFloat(SpeedAnimHash, speed);
    }
    
    public void PlaySendLetterSound()
    {
        AudioManager.Instance.Play(sendLetterSound);
    }

    // --- 核心修改：自动移动相关方法 ---

    /// <summary>
    /// 【核心修改】私有的移动循环方法，包含了超时逻辑。
    /// </summary>
    /// <param name="targetPosition">目标位置</param>
    /// <param name="proximityThreshold">到达阈值</param>
    /// <param name="timeout">超时时间（秒）</param>
    /// <returns>如果成功到达返回 true, 超时则返回 false。</returns>
    private async UniTask<bool> MoveUntilReached(Vector2 targetPosition, float proximityThreshold, float timeout)
    {
        float startTime = Time.time;

        while (Vector2.Distance(transform.position, targetPosition) > proximityThreshold)
        {
            // 1. 超时检查：如果当前时间超过了开始时间+超时时长，则中断
            if (Time.time - startTime > timeout)
            {
                Debug.LogWarning($"AutoMove to {targetPosition} timed out after {timeout} seconds. Player might be stuck.");
                return false; // 返回 false 表示超时
            }

            // 2. 持续移动
            Vector2 currentPosition = transform.position;
            Vector2 direction = (targetPosition - currentPosition).normalized;

            UpdateAnimatorParameters(direction, 1f); // 播放走路动画
            movementVelocityForFixedUpdate = direction * moveSpeed;
            IsWalking = true;
            lastValidInputX = direction.x;
            lastValidInputY = direction.y;

            // 等待下一帧
            await UniTask.Yield(PlayerLoopTiming.Update);
        }

        return true; // 返回 true 表示成功到达
    }
    
    /// <summary>
    /// 【修改后】异步方法，自动将玩家移动到指定位置，并在到达后播放动画。增加了超时保护。
    /// </summary>
    /// <param name="targetPosition">目标世界坐标</param>
    /// <param name="proximityThreshold">判断为“到达”的距离阈值</param>
    /// <param name="timeout">超时时间（秒），如果玩家被卡住，超过此时间将自动放弃移动</param>
    public async UniTask AutoMoveToPosition(Vector2 targetPosition, float proximityThreshold = 0.1f, float timeout = 5f)
    {
        isUnderSystemControl = true;
        try
        {
            // 调用包含超时逻辑的移动方法
            bool reachedDestination = await MoveUntilReached(targetPosition, proximityThreshold, timeout);

            // 只有在成功到达时才执行后续动作
            if (reachedDestination)
            {
                animator.SetTrigger(SendLetterAnimHash);
                await UniTask.WaitForSeconds(2f, cancellationToken: this.GetCancellationTokenOnDestroy());
            }
            // 如果超时(reachedDestination为false)，则不执行任何特殊操作，直接进入finally块进行清理
        }
        finally
        {
            // 无论成功、失败还是超时，这个代码块都保证执行，用于清理和归还控制权
            movementVelocityForFixedUpdate = Vector2.zero;
            IsWalking = false;
            var lastDirection = new Vector2(lastValidInputX, lastValidInputY).normalized;
            UpdateAnimatorParameters(lastDirection, 0f); // 确保动画恢复到静止站立
            isUnderSystemControl = false; // 归还玩家控制权，防止卡死
        }
    }
    
    /// <summary>
    /// 【修改后】异步方法，自动将玩家移动到床上准备睡觉。增加了超时保护。
    /// </summary>
    /// <param name="targetPosition">目标世界坐标</param>
    /// <param name="proximityThreshold">判断为“到达”的距离阈值</param>
    /// <param name="timeout">超时时间（秒），如果玩家被卡住，超过此时间将自动放弃移动</param>
    public async UniTask AutoMoveToSleep(Vector2 targetPosition, float proximityThreshold = 0.2f, float timeout = 5f)
    {
        isUnderSystemControl = true;
        try
        {
            // 调用包含超时逻辑的移动方法。此方法不关心是否成功，因为它之后没有特殊动画。
            await MoveUntilReached(targetPosition, proximityThreshold, timeout);
        }
        finally
        {
            // 无论成功、失败还是超时，都清理并归还控制权
            movementVelocityForFixedUpdate = Vector2.zero;
            IsWalking = false;
            var lastDirection = new Vector2(lastValidInputX, lastValidInputY).normalized;
            UpdateAnimatorParameters(lastDirection, 0f); // 确保动画恢复到静止站立
            isUnderSystemControl = false; // 归还玩家控制权，防止卡死
        }
    }

    public void EnablePlayerMovement() => CanPlayerMove = true;
    public void DisablePlayerMovement() => CanPlayerMove = false;
    
    private void OnDestroy()
    {
        EventCenter.RemoveListener(GameEvents.GameStartsPlayerWakesUp, OnGameStartsPlayerWakesUp);
        EventCenter.RemoveListener(GameEvents.PlayerWakesUp, OnPlayerWakesUp);
        EventCenter.RemoveListener(GameEvents.PlayerSleep, OnPlayerSleep);
    }
}