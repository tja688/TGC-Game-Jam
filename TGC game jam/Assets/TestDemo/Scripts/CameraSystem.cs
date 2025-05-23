using System;
using UnityEngine;

public class CameraSystem : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    private Transform currentTarget; 

    public static event Action<Transform> OnSetCameraTarget;
    
    private void OnEnable()
    {
        SetTarget(PlayerMove.CurrentPlayer.transform);
        
        OnSetCameraTarget += SetTarget;
    }

    private void OnDisable()
    {
        OnSetCameraTarget -= SetTarget;
    }

    private void LateUpdate()
    {
        if (currentTarget)
        {
            var targetPosition = new Vector3(
                currentTarget.position.x,
                currentTarget.position.y,
                transform.position.z);

            transform.position = Vector3.Lerp(
                transform.position,
                targetPosition,
                moveSpeed * Time.deltaTime);
        }
        else
        {
            SetTarget(PlayerMove.CurrentPlayer.transform);
        }
    }

    // 动态设置新目标（如果设置为空则默认回位玩家位置）
    private void SetTarget(Transform newTarget)
    {
        currentTarget = newTarget;
    }

    public static void SetCameraTarget(Transform obj)
    {
        OnSetCameraTarget?.Invoke(obj);
    }
    
}