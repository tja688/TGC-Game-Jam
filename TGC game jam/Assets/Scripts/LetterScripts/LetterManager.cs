using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

// using System.Linq; // List.Contains 和 List.IndexOf 不需要显式导入Linq，但如果后续有更复杂查询则可能需要

public class LetterManager : MonoBehaviour
{
    [System.Serializable]
    public class LetterAsset
    {
        [Tooltip("信件的名称，需要与背包中物品的ItemName完全匹配")]
        public string letterName;
        [Tooltip("信件对应的Sprite图片")]
        public Sprite letterSprite;
    }

    [Header("UI 组件")]
    [Tooltip("用于显示信件内容的Image组件")]
    [SerializeField] private Image letterDisplayImage;

    [Tooltip("外部按钮，按下此按钮时会检查背包并更新可显示信件列表")]
    [SerializeField] private Button inventoryCheckButton;

    [Header("信件配置")]
    [Tooltip("所有可能的信件及其对应的图片和名称")]
    [SerializeField] private List<LetterAsset> allPossibleLetters = new List<LetterAsset>();

    private readonly List<Sprite> displayableLetterSprites = new List<Sprite>(); // 当前背包中存在并可供显示的信件图片
    private int currentDisplayIndex = -1; // 当前显示图片在 displayableLetterSprites 中的索引

    private void Start()
    {
        if (!letterDisplayImage)
        {
            Debug.LogError("LetterManager: Letter Display Image 未在Inspector中指定！脚本将禁用。");
            enabled = false; // 如果关键组件缺失，则禁用此脚本
            return;
        }

        if (inventoryCheckButton)
        {
            inventoryCheckButton.onClick.AddListener(OnInventoryCheckButtonPressed);
        }
        else
        {
            Debug.LogWarning("LetterManager: Inventory Check Button 未在Inspector中指定。信件列表将不会通过按钮点击自动更新。");
        }

        // 游戏开始时，根据当前背包状态初始化一次可显示信件列表
        UpdateDisplayableLetters();
        UpdateImageComponent(); // 根据刷新后的列表更新UI显示
    }

    private void OnDestroy()
    {
        // 移除事件监听，防止内存泄漏
        if (inventoryCheckButton)
        {
            inventoryCheckButton.onClick.RemoveListener(OnInventoryCheckButtonPressed);
        }
    }

    /// <summary>
    /// 当绑定的外部按钮被按下时调用此方法。
    /// </summary>
    private void OnInventoryCheckButtonPressed()
    {
        UpdateDisplayableLetters();
    }

    /// <summary>
    /// 核心逻辑：检查背包，并根据背包中的物品更新可显示的信件列表。
    /// </summary>
    public void UpdateDisplayableLetters()
    {
        if (!BackpackManager.Instance)
        {
            if (letterDisplayImage)
            {
                letterDisplayImage.sprite = null;
                letterDisplayImage.enabled = false;
            }
            displayableLetterSprites.Clear();
            currentDisplayIndex = -1;
            return;
        }

        var itemsInBackpack = BackpackManager.Instance.ItemNamesInBackpack;
        Sprite previouslyDisplayedSprite = null;

        // 记录当前正在显示的Sprite (如果有效)
        if (currentDisplayIndex >= 0 && currentDisplayIndex < displayableLetterSprites.Count)
        {
            previouslyDisplayedSprite = displayableLetterSprites[currentDisplayIndex];
        }

        displayableLetterSprites.Clear(); // 清空旧的可显示列表

        // 遍历所有预定义的信件
        foreach (var letterAsset in allPossibleLetters.Where(letterAsset => itemsInBackpack.Contains(letterAsset.letterName)).Where(letterAsset => letterAsset.letterSprite != null && !displayableLetterSprites.Contains(letterAsset.letterSprite)))
        {
            displayableLetterSprites.Add(letterAsset.letterSprite);
        }

        // 更新当前显示索引的逻辑
        if (displayableLetterSprites.Count > 0)
        {
            if (previouslyDisplayedSprite)
            {
                // 尝试在新列表中找到之前显示的图片
                var newIndexOfPreviousSprite = displayableLetterSprites.IndexOf(previouslyDisplayedSprite);
                // 如果找到了，保持显示它
                currentDisplayIndex = newIndexOfPreviousSprite != -1 ? newIndexOfPreviousSprite :
                    // 如果之前显示的图片在新列表中不存在了（例如被移除了），则显示新列表的第一张
                    0;
            }
            else
            {
                // 如果之前没有图片显示，或者列表是空的，现在有图片了，则显示第一张
                currentDisplayIndex = 0;
            }
        }
        else
        {
            // 如果没有可显示的信件了
            currentDisplayIndex = -1;
        }

        UpdateImageComponent(); // 根据新的列表和索引更新UI
    }

    /// <summary>
    /// 根据 currentDisplayIndex 更新 Image 组件的显示。
    /// </summary>
    private void UpdateImageComponent()
    {
        if (!letterDisplayImage) return;

        if (displayableLetterSprites.Count > 0 && currentDisplayIndex >= 0 && currentDisplayIndex < displayableLetterSprites.Count)
        {
            letterDisplayImage.sprite = displayableLetterSprites[currentDisplayIndex];
            letterDisplayImage.enabled = true; // 确保Image组件是激活的以显示图片
        }
        else
        {
            // 没有可显示的信件，或者索引无效
            letterDisplayImage.sprite = null;   // 清空Sprite
            letterDisplayImage.enabled = false; // 隐藏Image组件，避免显示空白色块
            currentDisplayIndex = -1;       // 确保索引被重置
        }
    }

    /// <summary>
    /// 公开方法：切换到下一封信件。
    /// 如果到达列表末尾，则循环到开头。
    /// </summary>
    public void ShowNextLetter()
    {
        if (displayableLetterSprites.Count == 0)
        {
            // Debug.Log("没有可供切换的信件 (下一张)。");
            UpdateImageComponent(); // 确保UI反映空状态
            return;
        }

        currentDisplayIndex++;
        if (currentDisplayIndex >= displayableLetterSprites.Count)
        {
            currentDisplayIndex = 0; // 循环到第一张
        }
        UpdateImageComponent();
    }

    /// <summary>
    /// 公开方法：切换到上一封信件。
    /// 如果到达列表开头，则循环到末尾。
    /// </summary>
    public void ShowPreviousLetter()
    {
        if (displayableLetterSprites.Count == 0)
        {
            // Debug.Log("没有可供切换的信件 (上一张)。");
            UpdateImageComponent(); // 确保UI反映空状态
            return;
        }

        currentDisplayIndex--;
        if (currentDisplayIndex < 0)
        {
            currentDisplayIndex = displayableLetterSprites.Count - 1; // 循环到最后一张
        }
        UpdateImageComponent();
    }
}