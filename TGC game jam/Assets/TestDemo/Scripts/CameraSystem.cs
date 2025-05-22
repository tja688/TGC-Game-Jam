using UnityEngine;

public class CameraSystem : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    
    [SerializeField] private Transform playerTransform;
    
    private void OnEnable()
    {
        playerTransform = PlayerMove.CurrentPlayer.transform;
    }
    
    private void LateUpdate()
    {
        if (!playerTransform) return;
        
        var targetPosition = new Vector3(
            playerTransform.position.x,
            playerTransform.position.y,
            transform.position.z);
        
        transform.position = Vector3.Lerp(
            transform.position,
            targetPosition,
            moveSpeed * Time.deltaTime);
    }
}