using System;
using UnityEngine;
using Cysharp.Threading.Tasks;

public class PlayerMove : MonoBehaviour
{
    // Animator Parameter Hashes
    private static readonly int HorizontalAnimHash = Animator.StringToHash("InputX");
    private static readonly int VerticalAnimHash = Animator.StringToHash("InputY");
    private static readonly int SpeedAnimHash = Animator.StringToHash("Speed");
    private static readonly int WakeUpAnimHash = Animator.StringToHash("WakeUp"); // 新增：起床动画Trigger
    private static readonly int SleepAnimHash = Animator.StringToHash("Sleep");   // 新增：入眠动画Trigger
    private static readonly int SendLetterAnimHash = Animator.StringToHash("SendLetter");   // 新增：入眠动画Trigger

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
        
        // 如果正由系统自动控制，则跳过玩家输入的所有逻辑
        if (isUnderSystemControl) return;

        var inputVector = Vector2.zero;

        if (CanPlayerMove && inputActions != null) // 确保 inputActions 也不是 null
        {
            inputVector = inputActions.PlayerControl.Move.ReadValue<Vector2>();
        }

        var horizontalInput = inputVector.x;
        var verticalInput = inputVector.y;

        // 应用停止阈值
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
            // 如果没有输入，保持上一次的有效朝向
            targetAnimationDirection = new Vector2(lastValidInputX, lastValidInputY).normalized;
        }

        // 平滑动画朝向的转变
        if (targetAnimationDirection.sqrMagnitude > 0.001f) // 避免target为零向量时产生NaN
        {
            currentAnimationDirection = Vector2.SmoothDamp(currentAnimationDirection, targetAnimationDirection, ref turnAnimationVelocity, turnSmoothTime);
        }
        
        // 更新Animator的朝向和速度参数
        // 如果 CanPlayerMove 为 false, currentRawSpeed 将为 0 (因为 inputVector 为 zero)
        UpdateAnimatorParameters(currentAnimationDirection, currentRawSpeed);

        // 根据是否可以移动来计算最终的移动速度
        if (!CanPlayerMove)
        {
            movementVelocityForFixedUpdate = Vector2.zero;
            IsWalking = false; // 确保在不可移动时 IsWalking 也为 false
        }
        else
        {
            movementVelocityForFixedUpdate = currentRawInputDirection.normalized * (moveSpeed * Mathf.Clamp01(currentRawSpeed)); // 使用 normalized 和 Clamp01 来确保速度与输入强度一致且不超过 moveSpeed
        }
    }

    private void FixedUpdate()
    {
        if (rigidbody2d)
        {
            rigidbody2d.MovePosition(rigidbody2d.position + movementVelocityForFixedUpdate * Time.fixedDeltaTime);
        }
    }

    private void OnGameStartsPlayerWakesUp()
    {
        CanPlayerMove = true; // 确保玩家在醒来后可以移动
        if (animator)
        {
            animator.SetTrigger(WakeUpAnimHash);
        }
    }

    private async void OnPlayerSleep()
    {
        CanPlayerMove = false; // 玩家入睡，禁止移动
        
        await Sleep();
    }
    
    private async void OnPlayerWakesUp()
    {
        await WakesUp();
        
        CanPlayerMove = true; // 玩家醒来，允许移动
    }

    private async UniTask WakesUp()
    {
        ScreenFadeController.Instance.BeginFadeToClear(3f);
        
        await UniTask.WaitForSeconds(0.5f);
        
        if (animator)
        {
            animator.SetTrigger(WakeUpAnimHash);
        }
    }
    private async UniTask Sleep()
    {
        ScreenFadeController.Instance.BeginFadeToBlack(3f);
        
        await UniTask.WaitForSeconds(0.5f);
        
        if (animator)
        {
            animator.SetTrigger(SleepAnimHash);
        }
    }

    private void UpdateAnimatorParameters(Vector2 direction, float speed)
    {
        if (!animator) return;

        animator.SetFloat(HorizontalAnimHash, direction.x);
        animator.SetFloat(VerticalAnimHash, direction.y);
        animator.SetFloat(SpeedAnimHash, speed);
    }
    
    // 位于 PlayerMove.cs 中

    /// <summary>
    /// 异步方法，自动将玩家移动到指定位置，并在到达后播放动画。
    /// </summary>
    /// <param name="targetPosition">目标世界坐标</param>
    /// <param name="proximityThreshold">判断为“到达”的距离阈值</param>
    public async UniTask AutoMoveToPosition(Vector2 targetPosition, float proximityThreshold = 0.1f)
    {
        // 夺取控制权
        isUnderSystemControl = true;
        
        try
        {
            // 循环直到玩家接近目标点
            while (Vector2.Distance(transform.position, targetPosition) > proximityThreshold)
            {
                Vector2 currentPosition = transform.position;
                Vector2 direction = (targetPosition - currentPosition).normalized;

                UpdateAnimatorParameters(direction, 1f);
                movementVelocityForFixedUpdate = direction * moveSpeed;
                IsWalking = true;
                lastValidInputX = direction.x;
                lastValidInputY = direction.y;

                await UniTask.Yield(PlayerLoopTiming.Update);
            }
        }
        finally
        {
            // 1. 停止物理移动
            movementVelocityForFixedUpdate = Vector2.zero;
            IsWalking = false;
        
            // 2. 确保动画过渡到与最终朝向一致的Idle状态，防止滑动感
            var lastDirection = new Vector2(lastValidInputX, lastValidInputY).normalized;
            UpdateAnimatorParameters(lastDirection, 0f);
        
            // 3. 触发投信动画
            animator.SetTrigger(SendLetterAnimHash);

            // 4. 【核心修改】异步等待2秒，让动画有时间播放
            await UniTask.WaitForSeconds(2f, cancellationToken: this.GetCancellationTokenOnDestroy());
        
            // 5. 等待结束后，再归还玩家的输入控制权
            isUnderSystemControl = false;
        }
    }
    
    
    public async UniTask AutoMoveToSleep(Vector2 targetPosition, float proximityThreshold = 0.2f)
    {
        // 夺取控制权
        isUnderSystemControl = true;
        
        try
        {
            // 循环直到玩家接近目标点
            while (Vector2.Distance(transform.position, targetPosition) > proximityThreshold)
            {
                Vector2 currentPosition = transform.position;
                Vector2 direction = (targetPosition - currentPosition).normalized;

                UpdateAnimatorParameters(direction, 1f);
                movementVelocityForFixedUpdate = direction * moveSpeed;
                IsWalking = true;
                lastValidInputX = direction.x;
                lastValidInputY = direction.y;

                await UniTask.Yield(PlayerLoopTiming.Update);
            }
        }
        finally
        {
            // 1. 停止物理移动
            movementVelocityForFixedUpdate = Vector2.zero;
            IsWalking = false;
        
            // 2. 确保动画过渡到与最终朝向一致的Idle状态，防止滑动感
            var lastDirection = new Vector2(lastValidInputX, lastValidInputY).normalized;
            UpdateAnimatorParameters(lastDirection, 0f);
            
            // 5. 等待结束后，再归还玩家的输入控制权
            isUnderSystemControl = false;
        }
    }

    public void EnablePlayerMovement() => CanPlayerMove = true;
    public void DisablePlayerMovement() => CanPlayerMove = false;

    public void PlaySendLetterSound()
    {
        AudioManager.Instance.Play(sendLetterSound);
    }
    
    private void OnDestroy()
    {
        // 移除事件监听
        EventCenter.RemoveListener(GameEvents.GameStartsPlayerWakesUp, OnGameStartsPlayerWakesUp);
        
        EventCenter.RemoveListener(GameEvents.PlayerWakesUp, OnPlayerWakesUp);
        
        EventCenter.RemoveListener(GameEvents.PlayerSleep, OnPlayerSleep);
    }
}