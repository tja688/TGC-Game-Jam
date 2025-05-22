using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMove : MonoBehaviour
{
    private static readonly int Horizontal = Animator.StringToHash("Horizontal");
    private static readonly int Vertical = Animator.StringToHash("Vertical");
    private static readonly int Speed = Animator.StringToHash("Speed");

    public static event Action<GameObject> OnPlayerInitialized;
    
    private Animator animator;
    private Rigidbody2D rigbody2D;
    private InputActions inputActions;
    
    public float stopThreshold = 0.1f; 

    private void Awake()
    {
        animator = GetComponent<Animator>();
        rigbody2D = GetComponent<Rigidbody2D>();
        inputActions = new InputActions();
        
        OnPlayerInitialized?.Invoke(gameObject);

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
        var inputVector = inputActions.PlayerControl.Move.ReadValue<Vector2>();
        var horizontal = inputVector.x;
        var vertical = inputVector.y;

        var clampedHorizontal = Mathf.Abs(horizontal) < stopThreshold ? 0 : horizontal;
        var clampedVertical = Mathf.Abs(vertical) < stopThreshold ? 0 : vertical;

        if (clampedHorizontal != 0)
        {
            animator.SetFloat(Horizontal, clampedHorizontal);
            animator.SetFloat(Vertical, 0);
        }
        else if (clampedVertical != 0)
        {
            animator.SetFloat(Vertical, clampedVertical);
            animator.SetFloat(Horizontal, 0);
        }

        var dir = new Vector2(clampedHorizontal, clampedVertical);
        animator.SetFloat(Speed, dir.magnitude);
        rigbody2D.velocity = dir * 0.5f;
    }
}