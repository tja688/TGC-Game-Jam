using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "ItemInformation", menuName = "Items/Item Information")]
public class ItemInformation : ScriptableObject
{
    [Header("道具信息")]
    [Tooltip("道具名称")]
    public string itemName;          // 道具名称
    
    [Tooltip("道具图像")]
    public Texture2D itemIcon;       // 道具图标(Texture2D)
    
    [TextArea(3, 10)]
    [Tooltip("道具详细描述")]
    public string itemDescription;   // 道具描述
    
    [Tooltip("道具按钮")]
    public Button button;
    
}