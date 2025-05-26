using System;
using UnityEngine;

public class CameraSystem : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    private Transform currentTarget; 
    private bool isMovingToTarget = false; // 标记是否正在移动到目标

    public static event Action<Transform> OnSetCameraTarget;
    public static event Action OnCameraArrived;
    
    private void OnEnable()
    {
        SetTarget(PlayerMove.CurrentPlayer?.transform);
        OnSetCameraTarget += HandleSetCameraTarget;
    }

    private void OnDisable()
    {
        OnSetCameraTarget -= HandleSetCameraTarget;
    }

    private void LateUpdate()
    {
        if (!currentTarget)
        {
            // 如果没有目标，尝试设置玩家为目标
            SetTarget(PlayerMove.CurrentPlayer?.transform);
            return;
        }

        if (!isMovingToTarget && Vector3.Distance(transform.position, currentTarget.position) > 0.1f)
        {
            isMovingToTarget = true;
        }

        if (!isMovingToTarget) return;
        transform.position = Vector3.Lerp(
            transform.position,
            new Vector3(
                currentTarget.position.x,
                currentTarget.position.y,
                transform.position.z),
            moveSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, currentTarget.position) < 0.1f)
        {
            OnCameraArrivedOnce();
        }
    }

    private void HandleSetCameraTarget(Transform newTarget)
    {
        SetTarget(newTarget);
    }

    // 动态设置新目标（如果设置为空则默认回位玩家位置）
    private void SetTarget(Transform newTarget)
    {
        currentTarget = newTarget;
        
        if (newTarget)
        {
            isMovingToTarget = false;
        }
    }

    public static void SetCameraTarget(Transform obj)
    {
        OnSetCameraTarget?.Invoke(obj);
    }

    private void OnCameraArrivedOnce()
    {
        if (!currentTarget) return;
        OnCameraArrived?.Invoke();
        isMovingToTarget = false; // 重置状态，防止重复触发
    }
}