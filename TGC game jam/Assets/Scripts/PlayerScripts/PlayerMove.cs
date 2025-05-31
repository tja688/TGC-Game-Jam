using System;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    private static readonly int HorizontalAnimHash = Animator.StringToHash("InputX");
    private static readonly int VerticalAnimHash = Animator.StringToHash("InputY");
    private static readonly int SpeedAnimHash = Animator.StringToHash("Speed");
    public static GameObject CurrentPlayer { get; private set; }

    // [SerializeField] private SoundEffect playerMoveSound; // <-- 移除此行
    
    public static bool CanPlayerMove { get; set; }

    // --- 新增：公开玩家是否正在行走的属性 ---
    public bool IsWalking { get; private set; }
    // --- 结束新增 ---

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
    private float lastValidInputY = -1f;

    private Vector2 movementVelocityForFixedUpdate;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        rigidbody2d = GetComponent<Rigidbody2D>();
        CurrentPlayer = gameObject;
        CanPlayerMove = true;
        IsWalking = false; // 初始状态为未行走

        targetAnimationDirection = new Vector2(lastValidInputX, lastValidInputY).normalized;
        currentAnimationDirection = targetAnimationDirection;
        UpdateAnimatorParameters(currentAnimationDirection, 0f); 
    }

    private void Start()
    {
        inputActions = PlayerInputController.Instance.InputActions;
        if (inputActions == null)
            Debug.LogError("PlayerMove : inputActions null");
    }

    private void Update()
    {
        var inputVector = Vector2.zero;
        
        if (CanPlayerMove)
        {
            inputVector = inputActions.PlayerControl.Move.ReadValue<Vector2>();
        }

        var horizontalInput = inputVector.x;
        var verticalInput = inputVector.y;

        var clampedHorizontal = Mathf.Abs(horizontalInput) < stopThreshold ? 0 : horizontalInput;
        var clampedVertical = Mathf.Abs(verticalInput) < stopThreshold ? 0 : verticalInput;

        var currentRawInputDirection = new Vector2(clampedHorizontal, clampedVertical);
        var currentRawSpeed = currentRawInputDirection.magnitude;

        // --- 更新 IsWalking 状态 ---
        IsWalking = currentRawSpeed > stopThreshold;
        // --- 结束更新 ---

        if (IsWalking) // 原来的 currentRawSpeed > stopThreshold
        {
            targetAnimationDirection = currentRawInputDirection.normalized; 
            lastValidInputX = targetAnimationDirection.x;
            lastValidInputY = targetAnimationDirection.y;
            // AudioManager.Instance.Play(playerMoveSound); // <-- 移除此处的音效播放调用
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

        if (!CanPlayerMove) // 如果不能移动，确保速度为0
        {
            movementVelocityForFixedUpdate = Vector2.zero;
             IsWalking = false; // 确保在这种情况下 IsWalking 也为 false
        }
        else
        {
            movementVelocityForFixedUpdate = currentRawInputDirection * moveSpeed;
        }
    }

    private void FixedUpdate()
    {
        if (rigidbody2d)
        {
            rigidbody2d.MovePosition(rigidbody2d.position + movementVelocityForFixedUpdate * Time.fixedDeltaTime);
        }
    }

    private void UpdateAnimatorParameters(Vector2 direction, float speed)
    {
        animator.SetFloat(HorizontalAnimHash, direction.x);
        animator.SetFloat(VerticalAnimHash, direction.y);
        animator.SetFloat(SpeedAnimHash, speed);
    }
}