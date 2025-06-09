using UnityEngine;

public class SpriteSwitcher : MonoBehaviour
{
    // 公开变量，可以在Inspector面板中设置
    public SpriteRenderer firstSpriteRenderer;  // 第一个精灵渲染器
    public SpriteRenderer secondSpriteRenderer; // 第二个精灵渲染器
    public Sprite firstNewSprite;              // 第一个新精灵
    public Sprite secondNewSprite;             // 第二个新精灵

    // 原始精灵备份，用于可能需要恢复的情况
    private Sprite firstOriginalSprite;
    private Sprite secondOriginalSprite;

    private void Start()
    {
        // 保存原始精灵
        if (firstSpriteRenderer != null)
        {
            firstOriginalSprite = firstSpriteRenderer.sprite;
        }
        
        if (secondSpriteRenderer != null)
        {
            secondOriginalSprite = secondSpriteRenderer.sprite;
        }
        
        GameVariables.OnDayChanged += OndayChange;

    }

    private void OnDestroy()
    {
        GameVariables.OnDayChanged -= OndayChange;
    }

    private void OndayChange()
    {
        
        if(GameVariables.Day == 5)
        {
            // 替换第一个精灵渲染器的精灵
            if (firstSpriteRenderer != null && firstNewSprite != null)
            {
                firstSpriteRenderer.sprite = firstNewSprite;
            }
            
            // 替换第二个精灵渲染器的精灵
            if (secondSpriteRenderer != null && secondNewSprite != null)
            {
                secondSpriteRenderer.sprite = secondNewSprite;
            }
        }
    }
}