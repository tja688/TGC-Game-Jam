using UnityEngine;

/// <summary>
/// 定义可交互对象的接口
/// </summary>
public interface IInteractable
{
    /// <summary>
    /// 交互提示应该出现的位置点 (例如，对象头顶)
    /// </summary>
    Transform PromptAnchor { get; }

    /// <summary>
    /// 交互提示本体
    /// </summary>
    GameObject InteractNotice { get; }
    
    /// <summary>
    /// 当玩家进入交互范围时调用，显示交互提示
    /// </summary>
    /// <param name="player">发起交互的对象 (通常是玩家)</param>
    void ShowInteractionPrompt(GameObject player);

    /// <summary>
    /// 当玩家离开交互范围时调用，隐藏交互提示
    /// </summary>
    void HideInteractionPrompt();

    /// <summary>
    /// 当玩家按下交互键时调用，执行具体交互逻辑
    /// </summary>
    /// <param name="player">发起交互的对象 (通常是玩家)</param>
    void Interact(GameObject player);
}