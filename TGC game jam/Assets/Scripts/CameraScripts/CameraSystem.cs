using UnityEngine;
using System;

public class CameraSystem : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [Tooltip("How close the camera needs to be to a special target to be considered 'arrived'.")]
    [SerializeField] private float arrivalThreshold = 0.1f;


    [Header("References")]
    [SerializeField] private Transform initialPlayerTransform;

    private Transform currentPlayerToFollow;     
    private Transform activeSpecialTarget;      
    private bool hasFiredArrivalForThisSpecialTarget; 


    public static event Action OnCameraArrivedAtSpecialTarget;
    
    public static CameraSystem Instance { get; private set; }
    
    public float MoveSpeed { get => moveSpeed;set => moveSpeed = value; }

    private void Awake()
    {
        if (Instance && Instance != this)
        {
            Debug.LogWarning("Another instance of CameraSystem found, destroying this one.");
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
        
        
    }

    private void Start()
    {
        if (initialPlayerTransform)
        {
            SetPlayerToFollow(initialPlayerTransform);
        }
        else
        {
            Debug.LogWarning("CameraSystem: No initial player transform set and PlayerMove.CurrentPlayer is null. Camera might not follow player initially.");
        }
    }

    private void LateUpdate()
    {
        if (!currentPlayerToFollow && PlayerMove.CurrentPlayer)
        {
            SetPlayerToFollow(PlayerMove.CurrentPlayer.transform);
        }

        var targetToTrack = activeSpecialTarget ?? currentPlayerToFollow;

        if (!targetToTrack)
        {
            return;
        }

        var desiredCameraPosition = new Vector3(
            targetToTrack.position.x,
            targetToTrack.position.y,
            transform.position.z
        );

        var distanceToTarget = Vector3.Distance(transform.position, desiredCameraPosition);

        if (distanceToTarget > 0.001f) 
        {
            transform.position = Vector3.Lerp(
                transform.position,
                desiredCameraPosition,
                moveSpeed * Time.deltaTime);
        }

        if (!activeSpecialTarget || hasFiredArrivalForThisSpecialTarget) return;

        if (!(Vector3.Distance(transform.position, desiredCameraPosition) < arrivalThreshold)) return;

        OnCameraArrivedAtSpecialTarget?.Invoke();
        hasFiredArrivalForThisSpecialTarget = true;
    }
    
    public static void SetSpecialCameraTarget(Transform specialTarget)
    {
        if (!Instance)
        {
            Debug.LogError("CameraSystem.Instance is null. Cannot set special camera target. Ensure a CameraSystem object exists in the scene.");
            return;
        }
        Instance.ProcessSpecialTargetSetting(specialTarget);
    }

    private void ProcessSpecialTargetSetting(Transform newSpecialTarget)
    {
        if (activeSpecialTarget == newSpecialTarget)
        {
            if (!newSpecialTarget || !hasFiredArrivalForThisSpecialTarget) return;
        }
        else
        {
            activeSpecialTarget = newSpecialTarget;
        }

        hasFiredArrivalForThisSpecialTarget = false;
    }

    public void SetPlayerToFollow(Transform playerTransform)
    {
        currentPlayerToFollow = playerTransform ? playerTransform : null;
        if (!activeSpecialTarget)
        {
            hasFiredArrivalForThisSpecialTarget = true; 
        }
    }
}