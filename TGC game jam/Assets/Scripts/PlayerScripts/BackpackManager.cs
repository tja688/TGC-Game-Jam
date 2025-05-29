// BackpackManager.cs
using System.Collections.Generic;
using UnityEngine;
using System.Linq; // 用于 ToList()

public class BackpackManager : MonoBehaviour
{
    private static BackpackManager _instance;
    public static BackpackManager Instance
    {
        get
        {
            if (_instance) return _instance;
            _instance = FindObjectOfType<BackpackManager>();
            if (_instance) return _instance;
            var singletonObject = new GameObject("BackpackManager");
            _instance = singletonObject.AddComponent<BackpackManager>();
            return _instance;
        }
    }
    
    private Transform backpackTransform;
    public static Transform BackpackObject => Instance.backpackTransform; // 静态属性公开背包对象

    private readonly List<string> itemNamesInBackpack = new List<string>();
    public List<string> ItemNamesInBackpack => new List<string>(itemNamesInBackpack); // 返回一个副本，防止外部直接修改

    private readonly List<GameObject> itemsInBackpack = new List<GameObject>(); // 存储实际的GameObject引用

    private void Awake()
    {
        if (_instance && _instance != this)
        {
            Destroy(gameObject); // 防止重复实例
            return;
        }
        _instance = this;
   
        backpackTransform = transform;
    }
    

    /// <summary>
    /// 尝试将物品存入背包
    /// </summary>
    /// <param name="itemObject">要存入的物品游戏对象</param>
    /// <param name="instigator">拾取者</param>
    public bool StoreItem(GameObject itemObject)
    {
        if (!itemObject) return false;

        var storableItem = itemObject.GetComponent<IStorable>();
        if (storableItem == null)
        {
            Debug.LogWarning($"{itemObject.name} does not implement IStorable and cannot be stored.");
            return false;
        }

        // 调用道具的OnStored方法
        storableItem.OnStored(this);

        // 添加到内部列表
        if (itemsInBackpack.Contains(itemObject)) return true;
        itemsInBackpack.Add(itemObject);
        itemNamesInBackpack.Add(storableItem.ItemName);
        return true;
        
    }

    /// <summary>
    /// 从背包中取出物品 (通过名称)
    /// </summary>
    /// <param name="itemName">要取出的物品名称</param>
    /// <param name="instigator">取出物品的游戏对象 (通常是玩家)</param>
    /// <returns>取出的物品GameObject，如果未找到则返回null</returns>
    public GameObject RetrieveItem(string itemName)
    {
        GameObject itemToRetrieve = null;
        IStorable storableToRetrieve = null;

        for (var i = itemsInBackpack.Count - 1; i >= 0; i--)
        {
            var storable = itemsInBackpack[i].GetComponent<IStorable>();
            if (storable == null || storable.ItemName != itemName) continue;
            itemToRetrieve = itemsInBackpack[i];
            storableToRetrieve = storable;
            break;
        }

        if (itemToRetrieve)
        {
            itemsInBackpack.Remove(itemToRetrieve);
            itemNamesInBackpack.Remove(itemName); // 必须名称唯一

            storableToRetrieve.OnRetrieved();

            Debug.Log($"Retrieved {itemName} from backpack. Remaining items: {string.Join(", ", itemNamesInBackpack)}");
            return itemToRetrieve;
        }
        else
        {
            Debug.LogWarning($"Item with name '{itemName}' not found in backpack.");
            return null;
        }
    }

    /// <summary>
    /// 从背包中取出物品 (通过索引)
    /// </summary>
    public GameObject RetrieveItem(int index, GameObject instigator)
    {
        if (index < 0 || index >= itemsInBackpack.Count)
        {
            Debug.LogWarning($"Invalid index {index} for retrieving item.");
            return null;
        }

        var itemToRetrieve = itemsInBackpack[index];
        var storableToRetrieve = itemToRetrieve.GetComponent<IStorable>();

        if (storableToRetrieve == null) return null;
        itemsInBackpack.RemoveAt(index);
        itemNamesInBackpack.RemoveAt(index); // 保持同步

        storableToRetrieve.OnRetrieved();
        Debug.Log($"Retrieved {storableToRetrieve.ItemName} from backpack by index. Remaining items: {string.Join(", ", itemNamesInBackpack)}");
        return itemToRetrieve;
    }
    
    /// <summary>
    /// 检查背包中是否包含指定名称的物品
    /// </summary>
    public bool HasItem(string itemName)
    {
        return itemNamesInBackpack.Contains(itemName);
    }
    
    
    /// <summary>
    /// 从背包中销毁指定名称的物品
    /// </summary>
    /// <param name="itemName">要销毁的物品名称</param>
    /// <returns>如果成功找到并计划销毁则返回true，否则返回false</returns>
    public bool DestroyItem(string itemName)
    {
        for (var i = itemsInBackpack.Count - 1; i >= 0; i--)
        {
            var storable = itemsInBackpack[i].GetComponent<IStorable>();
            if (storable == null || storable.ItemName != itemName) continue;
            var itemToDestroy = itemsInBackpack[i];
            itemsInBackpack.RemoveAt(i);
            itemNamesInBackpack.Remove(itemName); 

            Destroy(itemToDestroy); 
            return true;
        }
        Debug.LogWarning($"Item with name '{itemName}' not found in backpack to destroy.");
        return false;
    }

}