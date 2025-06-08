// ItemInformation.cs
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "ItemInformation", menuName = "Items/Item Information")]
public class ItemInformation : ScriptableObject
{
    [Header("道具信息")]
    [Tooltip("道具名称")]
    public string itemName;

    [Tooltip("在背包按钮上显示的图标")]
    public Texture2D buttonIcon; // 原来的 itemIcon，改个更清晰的名字

    [Tooltip("点击按钮后，在详情面板中显示的大图")]
    public Texture2D largeDisplayImage; // 新增的字段，用于大图显示

    [TextArea(3, 10)]
    [Tooltip("道具详细描述")]
    public string itemDescription;

    [Tooltip("该道具使用的按钮预制件（Prefab）")]
    public Button button;

    // 优化建议：如果你的项目主要和UI打交道，直接用Sprite类型会更方便，可以省去代码里Sprite.Create的步骤。
    // 如果想优化，可以改成这样：
    // public Sprite buttonIconSprite;
    // public Sprite largeDisplayImageSprite;
    // 但我们先按你现有的Texture2D来修改。
}