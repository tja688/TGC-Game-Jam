using System;
using UnityEngine;
using DG.Tweening;

public class PlayerMove : MonoBehaviour
{
    private static readonly int Horizontal = Animator.StringToHash("Horizontal");
    private static readonly int Vertical = Animator.StringToHash("Vertical");
    private static readonly int Speed = Animator.StringToHash("Speed");
    public static GameObject CurrentPlayer { get; private set; }

    public static bool CanPlayerMove { get; set; }
    
    public static Rigidbody2D PlayerRigidbody2d {get; set;}
    
    private Animator animator;
    private Rigidbody2D rigidbody2d;
    private InputActions inputActions;
    
    public float stopThreshold = 0.1f; 

    private void Awake()
    {
        animator = GetComponent<Animator>();
        
        rigidbody2d = GetComponent<Rigidbody2D>();

        PlayerRigidbody2d = rigidbody2d;
        
        inputActions = new InputActions();
        
        CurrentPlayer =  gameObject;
        
        CanPlayerMove =  true;
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
        if (!CanPlayerMove) return;
        
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
        rigidbody2d.velocity = dir * 0.5f;
    }
    
}