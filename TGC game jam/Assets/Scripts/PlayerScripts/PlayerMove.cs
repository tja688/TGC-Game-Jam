using System;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    private static readonly int HorizontalAnimHash = Animator.StringToHash("InputX");
    private static readonly int VerticalAnimHash = Animator.StringToHash("InputY");
    private static readonly int SpeedAnimHash = Animator.StringToHash("Speed");
    public static GameObject CurrentPlayer { get; private set; }

    public static bool CanPlayerMove { get; set; }

    private Animator animator;
    private Rigidbody2D rigidbody2d;
    private InputActions inputActions;

    public float stopThreshold = 0.1f;
    public float moveSpeed = 1f;

    // --- 变量用于平滑转向 ---
    private Vector2 currentAnimationDirection;
    private Vector2 targetAnimationDirection;
    public float turnSmoothTime = 0.05f; // 显著减小此值以加快转向响应 (例如 0.05f)
    private Vector2 turnAnimationVelocity; // 用于 SmoothDamp
    // --- End 变量用于平滑转向 ---

    private float lastValidInputX = 0f;
    private float lastValidInputY = -1f;

    // --- 变量用于FixedUpdate移动 ---
    private Vector2 movementVelocityForFixedUpdate;
    // --- End 变量用于FixedUpdate移动 ---

    private void Awake()
    {
        animator = GetComponent<Animator>();
        rigidbody2d = GetComponent<Rigidbody2D>();
        CurrentPlayer = gameObject;
        CanPlayerMove = true;

        targetAnimationDirection = new Vector2(lastValidInputX, lastValidInputY).normalized;
        currentAnimationDirection = targetAnimationDirection;
        UpdateAnimatorParameters(currentAnimationDirection, 0f); // 初始设置动画
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


        if (currentRawSpeed > stopThreshold) 
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