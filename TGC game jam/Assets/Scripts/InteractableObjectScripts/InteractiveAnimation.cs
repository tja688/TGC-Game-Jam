using UnityEngine;
using DG.Tweening;

public class InteractiveAnimation : MonoBehaviour
{
    [Header("动画参数")]
    [Tooltip("目标缩放倍数")]
    public float targetScaleFactor = 1.1f;

    [Tooltip("单次缩放动画（放大或缩小）的持续时间（秒）")]
    public float scaleDuration = 0.5f;

    [Tooltip("动画缓动类型")]
    public Ease easeType = Ease.InOutSine;

    private Vector3 baseScale;
    private Tween scaleTween;
    private bool isBaseScaleInitialized = false; 
    private void Awake()
    {
        if (isBaseScaleInitialized) return;
        baseScale = transform.localScale;
        isBaseScaleInitialized = true;
    }

    private void OnEnable()
    {
        if (!isBaseScaleInitialized)
        {
            baseScale = transform.localScale;
            isBaseScaleInitialized = true;
            Debug.LogWarning("InteractiveAnimation: _baseScale 在 OnEnable 中被初始化，理想情况应在 Awake 中完成。", this);
        }
        
        transform.localScale = baseScale;

        if (baseScale == Vector3.zero && targetScaleFactor != 0)
        {
            Debug.LogWarning("InteractiveAnimation: _baseScale 是 Vector3.zero，动画可能不会有可见效果。", this);
        }

        StartPulsingAnimation();
    }

    private void OnDisable()
    {
        StopAndKillTween();
    }
    
    public void StartPulsingAnimation()
    {
        StopAndKillTween();

        if (!isBaseScaleInitialized)
        {
            Debug.LogError("InteractiveAnimation: 尝试启动动画但 _baseScale 未初始化!", this);
            return; 
        }
        
        scaleTween = transform.DOScale(baseScale * targetScaleFactor, scaleDuration)
            .SetEase(easeType)
            .SetLoops(-1, LoopType.Yoyo)
            .SetTarget(this);
    }

    private void StopAndKillTween()
    {
        if (scaleTween != null && scaleTween.IsActive())
        {
            scaleTween.Kill(); 
        }
        scaleTween = null;
    }
}