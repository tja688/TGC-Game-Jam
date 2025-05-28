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
        inputActions = new InputActions();
        CurrentPlayer = gameObject;
        CanPlayerMove = true;

        targetAnimationDirection = new Vector2(lastValidInputX, lastValidInputY).normalized;
        currentAnimationDirection = targetAnimationDirection;
        UpdateAnimatorParameters(currentAnimationDirection, 0f); // 初始设置动画
    }

    private void OnEnable()
    {
        inputActions.Enable();
    }

    private void OnDisable()
    {
        inputActions.Disable();
    }

    private void Update()
    {
        Vector2 inputVector = Vector2.zero;
        if (CanPlayerMove)
        {
            inputVector = inputActions.PlayerControl.Move.ReadValue<Vector2>();
        }

        float horizontalInput = inputVector.x;
        float verticalInput = inputVector.y;

        float clampedHorizontal = Mathf.Abs(horizontalInput) < stopThreshold ? 0 : horizontalInput;
        float clampedVertical = Mathf.Abs(verticalInput) < stopThreshold ? 0 : verticalInput;

        Vector2 currentRawInputDirection = new Vector2(clampedHorizontal, clampedVertical);
        // 注意：如果 clampedHorizontal 和 clampedVertical 都为0，currentRawInputDirection.magnitude 会是0。
        // 如果它们有值，currentRawInputDirection.normalized 会得到单位向量。
        // currentRawSpeed 用于动画，表示移动意图的强度。
        float currentRawSpeed = currentRawInputDirection.magnitude;


        // 1. 确定目标动画方向 (Target Animation Direction)
        if (currentRawSpeed > stopThreshold) // 如果有有效输入 (这里用 currentRawSpeed 更直接)
        {
            targetAnimationDirection = currentRawInputDirection.normalized; // 使用归一化的原始输入作为目标
            lastValidInputX = targetAnimationDirection.x;
            lastValidInputY = targetAnimationDirection.y;
        }
        else // 如果没有有效输入 (停止移动)
        {
            targetAnimationDirection = new Vector2(lastValidInputX, lastValidInputY).normalized;
        }

        // 2. 平滑地更新当前动画方向 (Current Animation Direction)
        if (targetAnimationDirection.sqrMagnitude > 0.001f)
        {
            currentAnimationDirection = Vector2.SmoothDamp(currentAnimationDirection, targetAnimationDirection, ref turnAnimationVelocity, turnSmoothTime);
            // 可选的额外归一化，通常SmoothDamp对方向处理得不错
            // if (currentAnimationDirection.sqrMagnitude > 0.001f) currentAnimationDirection.Normalize();
        }

        // 3. 更新 Animator 参数
        // Speed 参数使用 currentRawSpeed，它反映了输入的大小/强度
        UpdateAnimatorParameters(currentAnimationDirection, currentRawSpeed);

        // 4. 准备移动的速度向量给 FixedUpdate
        if (!CanPlayerMove)
        {
            movementVelocityForFixedUpdate = Vector2.zero;
        }
        else
        {
            // 速度向量 = 方向 * 速度值。
            // 如果 currentRawSpeed > 0, currentRawInputDirection.normalized * currentRawSpeed == currentRawInputDirection
            // 所以可以直接用 currentRawInputDirection * moveSpeed
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

    // 辅助方法，用于更新Animator的参数
    private void UpdateAnimatorParameters(Vector2 direction, float speed)
    {
        animator.SetFloat(HorizontalAnimHash, direction.x);
        animator.SetFloat(VerticalAnimHash, direction.y);
        animator.SetFloat(SpeedAnimHash, speed);
    }
}