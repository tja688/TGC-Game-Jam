using System;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    // Animator Parameter Hashes
    private static readonly int HorizontalAnimHash = Animator.StringToHash("InputX");
    private static readonly int VerticalAnimHash = Animator.StringToHash("InputY");
    private static readonly int SpeedAnimHash = Animator.StringToHash("Speed");
    private static readonly int WakeUpAnimHash = Animator.StringToHash("WakeUp"); // 新增：起床动画Trigger
    private static readonly int SleepAnimHash = Animator.StringToHash("Sleep");   // 新增：入眠动画Trigger

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
        EventCenter.AddEventListener(GameEvents.PlayerSleep, OnPlayerSleep);
    }

    private void Update()
    {
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
        Debug.Log("Event: GameStartsPlayerWakesUp received. Player waking up.");
        CanPlayerMove = true; // 确保玩家在醒来后可以移动
        if (animator != null)
        {
            animator.SetTrigger(WakeUpAnimHash);
        }
        // 可选：如果需要，可以在这里重置玩家的初始站立朝向的动画参数
        // UpdateAnimatorParameters(new Vector2(lastValidInputX, lastValidInputY), 0f);
    }

    private void OnPlayerSleep()
    {
        Debug.Log("Event: PlayerSleep received. Player going to sleep.");
        CanPlayerMove = false; // 玩家入睡，禁止移动
        // Update() 方法中会因为 CanPlayerMove = false 导致速度为0，并传递给 Animator
        // IsWalking 也会在 Update() 中被设置为 false
        // movementVelocityForFixedUpdate 也会在 Update() 中被设置为 Vector2.zero

        if (animator)
        {
            // 触发入眠动画前，可以确保速度参数已经是0（虽然Update中会处理）
            // animator.SetFloat(SpeedAnimHash, 0f);
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

    private void OnDestroy()
    {
        // 移除事件监听
        EventCenter.RemoveListener(GameEvents.GameStartsPlayerWakesUp, OnGameStartsPlayerWakesUp);
        EventCenter.RemoveListener(GameEvents.PlayerSleep, OnPlayerSleep);
    }
}