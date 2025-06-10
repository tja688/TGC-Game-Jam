using UnityEngine;

public class AnimationTriggerAndDestroy : MonoBehaviour
{
    private static readonly int Start = Animator.StringToHash("Start");

    private Animator animator;

    void Awake()
    {
        animator = GetComponent<Animator>();

        if (animator == null)
        {
            Debug.LogError("在对象 " + gameObject.name + " 上没有找到Animator组件！", this);
        }
    }


    public void TriggerAnimation()
    {
        if (animator != null)
        {
            animator.SetTrigger(Start);
        }
    }


    public void OnAnimationComplete()
    {
        Destroy(gameObject);
    }
}