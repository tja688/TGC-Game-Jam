using System.Collections;
using UnityEngine;

public class LiftItem : ItemBase // 假设 ItemBase 是你已有的基类
{
    [Header("灯光设置")]
    [SerializeField] private SpriteRenderer lightSprite; // 将子对象的灯(SpriteRenderer)拖拽到这里
    [SerializeField] private float colorChangeDuration = 0.5f; // 颜色平滑变化的时长
    
    private Color initialLightColor;
    private Color poweredColor;      // EC0000
    private Color movingColor;       // 3BB600
    private Color defaultDestroyColor; // 500000 (在OnDestroy时恢复的颜色)

    [Header("动画设置")]
    [SerializeField] private string upAnimationTrigger = "UP";
    [SerializeField] private string downAnimationTrigger = "DOWN";
    [SerializeField] private string upStateName = "测试电梯上升"; 
    [SerializeField] private string downStateName = "测试电梯下降"; 

    private Animator animator;

    private bool isPoweredUp = false;
    private bool isAtUpperLevel = false;
    private bool isBusy = false; // 防止重复触发协程

    private Rigidbody2D playerRigidbody;
    
    protected override void Start()
    {
        base.Start();
        animator = GetComponent<Animator>();

        if (lightSprite)
        {
            initialLightColor = lightSprite.color;
        }
        else
        {
            Debug.LogError("LiftItem: 灯的SpriteRenderer (lightSprite) 未在Inspector中指定!");
        }

        // 解析颜色字符串
        if (!ColorUtility.TryParseHtmlString("#EC0000", out poweredColor))
        {
            Debug.LogError("无法解析 poweredColor: #EC0000");
            poweredColor = Color.red; // 设置一个默认值
        }
        if (!ColorUtility.TryParseHtmlString("#3BB600", out movingColor))
        {
            Debug.LogError("无法解析 movingColor: #3BB600");
            movingColor = Color.green; // 设置一个默认值
        }

        if (ColorUtility.TryParseHtmlString("#500000", out defaultDestroyColor)) return;
        Debug.LogError("无法解析 defaultDestroyColor: #500000");
        defaultDestroyColor = Color.gray; // 设置一个默认值
    }

    public override void Interact(GameObject player)
    {
        if (isBusy) return; // 如果正在执行某个操作，则不响应新的交互

        if (!isPoweredUp)
        {
            if (!BackpackManager.Instance.HasItem("battery"))
            {
                EventCenter.TriggerEvent<Vector2, string>
                    (GameEvents.ShowDialogue, UIUtility.WorldToScreenSpaceOverlayPosition(promptAnchorTransform.position),
                    "{offset f=2} <color=red> No power!");
            }
            else
            {
                StartCoroutine(PowerUpSequence());
            }
        }
        else // 已通电
        {
            StartCoroutine(!isAtUpperLevel ? GoUpSequence() : GoDownSequence());
        }
    }

    private IEnumerator PowerUpSequence()
    {
        isBusy = true;

        // 1. 平滑改变颜色至 EC0000
        yield return StartCoroutine(SmoothColorChange(lightSprite, poweredColor, colorChangeDuration));
        
        // 2. 销毁电池
        var destroyed = BackpackManager.Instance.DestroyItem("battery");
        if (!destroyed)
            Debug.LogWarning("未能从背包销毁电池！检查电池名称是否为 'battery'。");

        isPoweredUp = true;
        isBusy = false;
        
        StartCoroutine(GoUpSequence());
    }

    private IEnumerator GoUpSequence()
    {
        isBusy = true;
        
        InstantiatedPromptInstance.SetActive(false);
        
        PlayerControl();
        
        // 1. 触发 UP 动画
        animator.SetTrigger(upAnimationTrigger);

        // 2. 平滑改变灯颜色至 3BB600
        yield return StartCoroutine(SmoothColorChange(lightSprite, movingColor, colorChangeDuration));
        
        yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).IsName(upStateName) && 
                                       animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f);

        yield return StartCoroutine(SmoothColorChange(lightSprite, poweredColor, colorChangeDuration));

        isAtUpperLevel = true;

        PlayerRelease();
            
        InstantiatedPromptInstance.SetActive(true);

        isBusy = false;
    }

    private IEnumerator GoDownSequence()
    {
        isBusy = true;
        
        InstantiatedPromptInstance.SetActive(false);
        
        PlayerControl();

        animator.SetTrigger(downAnimationTrigger);

        yield return StartCoroutine(SmoothColorChange(lightSprite, movingColor, colorChangeDuration));
        
        yield return null; 
        
        yield return new WaitUntil(() => !animator.GetCurrentAnimatorStateInfo(0).IsName(downStateName));
        
        yield return StartCoroutine(SmoothColorChange(lightSprite, poweredColor, colorChangeDuration));
        
        isAtUpperLevel = false;
        
        PlayerRelease();
        
        InstantiatedPromptInstance.SetActive(true);
        
        isBusy = false;
    }

    private static IEnumerator SmoothColorChange(SpriteRenderer sprite, Color targetColor, float duration)
    {
        if (!sprite) yield break;

        var currentColor = sprite.color;
        var timer = 0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            sprite.color = Color.Lerp(currentColor, targetColor, timer / duration);
            yield return null;
        }
        sprite.color = targetColor; // 确保最终颜色准确
    }

    private void PlayerControl()
    {
        PlayerMove.CanPlayerMove = false;
        
        PlayerMove.CurrentPlayer.transform.position = transform.position;

        PlayerMove.CurrentPlayer.transform.SetParent(transform);
    }
    
    private void PlayerRelease()
    {
        PlayerMove.CurrentPlayer.transform.SetParent(null);
        
        PlayerMove.CanPlayerMove = true;

    }
    
    protected void OnDestroy() 
    {
        if (!lightSprite) return;
        lightSprite.color = defaultDestroyColor; // 还原灯颜色至指定的默认值
    }
}