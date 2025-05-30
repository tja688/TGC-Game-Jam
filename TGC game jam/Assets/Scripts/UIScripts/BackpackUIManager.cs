// BackpackUIManager.cs (Optimized)
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using TMPro;

public class BackpackUIManager : MonoBehaviour
{
    #region Singleton
    private static BackpackUIManager _instance;
    public static BackpackUIManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<BackpackUIManager>();
                if (_instance == null)
                {
                    GameObject singletonObject = new GameObject("BackpackUIManager");
                    _instance = singletonObject.AddComponent<BackpackUIManager>();
                }
            }
            return _instance;
        }
    }
    #endregion

    [Header("UI Content References")]
    [Tooltip("用于显示道具图片和描述的大面板。此脚本会控制其内容的显隐。")]
    public GameObject largeDisplayPanel;
    [Tooltip("用于显示道具图片的Image组件")]
    public Image itemImageDisplay;
    [Tooltip("用于显示道具描述的TextMeshProUGUI组件")]
    public TextMeshProUGUI itemDescriptionDisplay;
    [Tooltip("用于动态添加道具按钮的小面板的父对象")]
    public Transform buttonsContentParent;
    [Tooltip("动态按钮布局管理器")]
    public DynamicButtonLayout dynamicLayoutManager;

    [Header("Item Data")]
    [Tooltip("所有可能的道具信息 ScriptableObject 列表")]
    public List<ItemInformation> allPossibleItemInfos;

    private Dictionary<string, ItemInformation> itemInfoMap = new Dictionary<string, ItemInformation>();
    private Dictionary<string, GameObject> displayedItemButtons = new Dictionary<string, GameObject>();

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;

        foreach (var info in allPossibleItemInfos)
        {
            if (info != null && !string.IsNullOrEmpty(info.itemName) && !itemInfoMap.ContainsKey(info.itemName))
            {
                itemInfoMap.Add(info.itemName, info);
            }
            else if (info != null && itemInfoMap.ContainsKey(info.itemName))
            {
                Debug.LogWarning($"Duplicate ItemName '{info.itemName}' in allPossibleItemInfos. Only the first one will be used.");
            }
        }
    }
    
    private void Start()
    {
        // 初始时，确保大详情面板是隐藏的，等待道具点击
        if (largeDisplayPanel)
        {
            largeDisplayPanel.SetActive(false);
        }
        else
        {
            Debug.LogError("LargeDisplayPanel 未在 BackpackUIManager 中分配!", this);
        }
    }


    /// <summary>
    /// 刷新背包UI中显示的道具按钮列表，并确保详情面板初始隐藏。
    /// 此方法应在背包主UI面板已经由外部系统显示后调用。
    /// </summary>
    public void RefreshContentView()
    {
        if (!BackpackManager.Instance)
        {
            Debug.LogError("BackpackManager instance not found!", this);
            return;
        }
        if (buttonsContentParent == null || dynamicLayoutManager == null)
        {
            Debug.LogError("buttonsContentParent 或 dynamicLayoutManager 未在 BackpackUIManager 中分配!", this);
            return;
        }

        List<string> itemsInActualBackpack = BackpackManager.Instance.ItemNamesInBackpack;

        // 1. 找出需要移除的按钮
        List<string> buttonsToRemove = displayedItemButtons.Keys.Where(displayedItemName => !itemsInActualBackpack.Contains(displayedItemName)).ToList();
        foreach (string itemNameToRemove in buttonsToRemove)
        {
            if (displayedItemButtons.TryGetValue(itemNameToRemove, out GameObject buttonObj))
            {
                Destroy(buttonObj);
                displayedItemButtons.Remove(itemNameToRemove);
            }
        }

        // 2. 添加新的或更新现有的按钮
        foreach (string itemNameInBackpack in itemsInActualBackpack)
        {
            if (displayedItemButtons.ContainsKey(itemNameInBackpack))
            {
                continue;
            }

            if (itemInfoMap.TryGetValue(itemNameInBackpack, out ItemInformation info))
            {
                if (info.button == null)
                {
                    Debug.LogWarning($"ItemInformation for '{info.itemName}' does not have a button prefab assigned.", this);
                    continue;
                }

                GameObject buttonInstanceSource = info.button.gameObject;
                GameObject newButtonGO = Instantiate(buttonInstanceSource, buttonsContentParent);
                newButtonGO.transform.SetParent(buttonsContentParent, false);

                Button newButtonComponent = newButtonGO.GetComponent<Button>();
                if (newButtonComponent == null)
                {
                    Debug.LogError($"Instantiated button for '{info.itemName}' does not have a Button component!", newButtonGO);
                    Destroy(newButtonGO);
                    continue;
                }

                // 配置按钮视觉（如果需要从ItemInfo动态设置）
                Image[] imagesInButton = newButtonGO.GetComponentsInChildren<Image>(true);
                if (imagesInButton.Length > 0 && info.itemIcon)
                {
                    Image iconImage = imagesInButton.FirstOrDefault(img => img.gameObject.name.ToLower().Contains("icon")) ?? imagesInButton[0];
                     if(iconImage)
                    {
                         iconImage.sprite = Sprite.Create(info.itemIcon, new Rect(0, 0, info.itemIcon.width, info.itemIcon.height), new Vector2(0.5f, 0.5f));
                         iconImage.gameObject.SetActive(true);
                    }
                }
                
                TextMeshProUGUI nameText = newButtonGO.GetComponentInChildren<TextMeshProUGUI>(true);
                if (nameText) nameText.text = info.itemName;

                newButtonComponent.onClick.RemoveAllListeners();
                newButtonComponent.onClick.AddListener(() => DisplayItemDetails(info));

                displayedItemButtons.Add(itemNameInBackpack, newButtonGO);
            }
            else
            {
                Debug.LogWarning($"No ItemInformation found for item: '{itemNameInBackpack}' in backpack.", this);
            }
        }

        if (dynamicLayoutManager)
        {
            dynamicLayoutManager.UpdateLayout();
        }

        // 确保在刷新时，大详情面板是隐藏的，等待用户点击道具按钮
        if (largeDisplayPanel)
        {
            largeDisplayPanel.SetActive(false);
        }
    }

    /// <summary>
    /// 当小面板中的道具按钮被点击时调用，显示道具的详细信息。
    /// </summary>
    public void DisplayItemDetails(ItemInformation info)
    {
        if (info == null)
        {
            if (largeDisplayPanel) largeDisplayPanel.SetActive(false);
            return;
        }
        if (largeDisplayPanel == null || itemImageDisplay == null || itemDescriptionDisplay == null)
        {
            Debug.LogError("Large display panel or its components are not assigned in BackpackUIManager.", this);
            return;
        }

        if (info.itemIcon != null)
        {
            itemImageDisplay.sprite = Sprite.Create(info.itemIcon, new Rect(0, 0, info.itemIcon.width, info.itemIcon.height), new Vector2(0.5f, 0.5f));
            itemImageDisplay.gameObject.SetActive(true);
        }
        else
        {
            itemImageDisplay.sprite = null;
            itemImageDisplay.gameObject.SetActive(false);
        }
        itemDescriptionDisplay.text = info.itemDescription;
        largeDisplayPanel.SetActive(true);
    }

    /// <summary>
    /// 清理UI中显示的道具详情。由外部UI管理系统在关闭背包UI前调用。
    /// </summary>
    public void ClearUIDetailsOnClose()
    {
        if (largeDisplayPanel) largeDisplayPanel.SetActive(false);
        if (itemImageDisplay) itemImageDisplay.sprite = null;
        if (itemDescriptionDisplay) itemDescriptionDisplay.text = string.Empty;
    }

    // OnDestroy 中不再需要移除 closeButton 的监听器
}