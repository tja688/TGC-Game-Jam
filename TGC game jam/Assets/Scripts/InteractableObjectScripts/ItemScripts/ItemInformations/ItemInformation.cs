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
    public Texture2D buttonIcon;

    [Tooltip("点击按钮后，在详情面板中显示的大图")]
    public Texture2D largeDisplayImage; 
    [TextArea(3, 10)]
    [Tooltip("道具详细描述")]
    public string itemDescription;

    [Tooltip("该道具使用的按钮预制件（Prefab）")]
    public Button button;
    
}