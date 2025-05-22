using UnityEngine;

public class CameraSystem : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    
    private Transform playerTransform;
    
    private void OnEnable()
    {
        PlayerMove.OnPlayerInitialized += HandlePlayerInitialized;
    }
    
    private void OnDisable()
    {
        PlayerMove.OnPlayerInitialized -= HandlePlayerInitialized;
    }
    
    // 修正方法签名和实现
    private void HandlePlayerInitialized(GameObject playerObject)
    {
        if (!playerObject) return;
        playerTransform = playerObject.transform;
            
        transform.position = new Vector3(
            playerTransform.position.x,
            playerTransform.position.y,
            transform.position.z);
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