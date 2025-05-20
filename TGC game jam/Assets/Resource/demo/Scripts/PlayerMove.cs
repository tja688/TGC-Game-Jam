using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    private static readonly int Horizontal = Animator.StringToHash("Horizontal");
    private static readonly int Vertical = Animator.StringToHash("Vertical");
    private static readonly int Speed = Animator.StringToHash("Speed");
    private Animator _animator;
    private Rigidbody2D _rigidbody2D;
    public float stopThreshold = 0.1f; // 判定停止的阈值

    private void Start()
    {
        _animator = GetComponent<Animator>();
        _rigidbody2D = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        var horizontal = Input.GetAxisRaw("Horizontal");
        var vertical = Input.GetAxisRaw("Vertical");

        var clampedHorizontal = Mathf.Abs(horizontal) < stopThreshold ? 0 : horizontal;
        var clampedVertical = Mathf.Abs(vertical) < stopThreshold ? 0 : vertical;

        if (clampedHorizontal != 0)
        {
            _animator.SetFloat(Horizontal, clampedHorizontal);
            _animator.SetFloat(Vertical, 0);
        }
        else if (clampedVertical != 0)
        {
            _animator.SetFloat(Vertical, clampedVertical);
            _animator.SetFloat(Horizontal, 0);
        }

        var dir = new Vector2(clampedHorizontal, clampedVertical);
        _animator.SetFloat(Speed, dir.magnitude);
        _rigidbody2D.velocity = dir * 0.5f;
    }
}