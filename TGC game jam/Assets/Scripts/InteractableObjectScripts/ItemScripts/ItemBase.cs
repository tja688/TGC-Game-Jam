using UnityEngine;

public abstract class ItemBase : InteractableObjectBase, IStorable
{
    private bool originalColliderState;
    private bool originalRendererState;
    
    public string ItemName => objectName;

    protected override void Start()
    {
        base.Start();

        var col = GetComponent<Collider>();
        if (col)
        {
            originalColliderState = col.enabled;
        }
        var rend = GetComponent<Renderer>();
        if (rend)
        {
            originalRendererState = rend.enabled;
        }
    }

    public override void Interact(GameObject player)
    {
        BackpackManager.Instance.StoreItem(this.gameObject);
    }

    public virtual void OnStored(BackpackManager backpackManager)
    {
        var col = GetComponent<Collider>();
        if (col)
        {
            originalColliderState = col.enabled;
            col.enabled = false; // 禁用碰撞
        }

        var rend = GetComponent<Renderer>();
        if (rend)
        {
            originalRendererState = rend.enabled;
            rend.enabled = false; // 禁用渲染
        }

        // 设置为Backpack的子对象
        transform.SetParent(BackpackManager.BackpackObject);
        transform.localPosition = Vector3.zero; // 可以根据需要调整在背包内的位置

        // 隐藏交互提示 (如果它还显示的话)
        DestroyInteractionPrompt();
        
        // 禁用此脚本，因为物品在背包中时不应再被交互
        this.enabled = false;
    }

    public virtual void OnRetrieved()
    {
        // 默认回到场景保持独立
        transform.SetParent(null);

        var col = GetComponent<Collider>();
        if (col)
        {
            col.enabled = originalColliderState;
        }

        var rend = GetComponent<Renderer>();
        if (rend)
        {
            rend.enabled = originalRendererState;
        }
        
        enabled = true; // 重新启用脚本
    }
}