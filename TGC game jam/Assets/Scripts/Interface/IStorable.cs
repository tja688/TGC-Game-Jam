// IStorable.cs
using UnityEngine;

/// <summary>
/// 定义了可存入背包的物品必须实现的接口
/// </summary>
public interface IStorable
{
    /// <summary>
    /// 道具的名称
    /// </summary>
    string ItemName { get; }

    /// <summary>
    /// 当道具存入背包时调用的方法
    /// </summary>
    /// <param name="backpackManager">背包管理器的引用</param>
    void OnStored(BackpackManager backpackManager);

    /// <summary>
    /// 当道具从背包取出时调用的方法
    /// </summary>
    void OnRetrieved();
}