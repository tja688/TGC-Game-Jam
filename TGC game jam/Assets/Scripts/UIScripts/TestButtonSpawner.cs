using UnityEngine;
using UnityEngine.UI; // 如果你的按钮预制件上有特定的UI组件需要引用，可能需要

public class TestButtonSpawner : MonoBehaviour
{
    [Header("预制件与父对象")]
    public GameObject buttonPrefab; // 在Inspector中指定你的按钮预制件
    public Transform buttonsContentParent; // 在Inspector中指定你的 "ButtonsContent" 对象的Transform

    [Header("布局管理器")]
    public DynamicButtonLayout dynamicLayoutManager; // 在Inspector中指定带有DynamicButtonLayout脚本的对象

    void Update()
    {
        // 检测键盘 'M' 键是否被按下
        if (Input.GetKeyDown(KeyCode.M))
        {
            SpawnButtonAndRefreshLayout();
        }
    }

    void SpawnButtonAndRefreshLayout()
    {
        // --- 安全检查 ---
        if (buttonPrefab == null)
        {
            Debug.LogError("TestButtonSpawner: 按钮预制件 (Button Prefab) 未指定!");
            return;
        }
        if (buttonsContentParent == null)
        {
            Debug.LogError("TestButtonSpawner: ButtonsContent 父对象 (ButtonsContent Parent) 未指定!");
            return;
        }
        if (dynamicLayoutManager == null)
        {
            Debug.LogError("TestButtonSpawner: 动态布局管理器 (DynamicLayoutManager) 未指定!");
            return;
        }

        // 1. 实例化按钮预制件
        GameObject newButton = Instantiate(buttonPrefab);

        // 2. 将新生成的按钮设置为 ButtonsContent 的子对象
        // 对于UGUI元素，使用SetParent并设置worldPositionStays为false是一个好习惯，
        // 这样可以确保它正确地适配UI层级和缩放。
        newButton.transform.SetParent(buttonsContentParent, false);

        // (可选) 给按钮重命名，方便在Hierarchy中查看
        // newButton.name = "测试按钮 " + buttonsContentParent.childCount;

        // 3. 调用DynamicButtonLayout脚本来更新间距和布局
        dynamicLayoutManager.UpdateLayout();

        Debug.Log("已生成按钮并更新布局。当前按钮数量: " + buttonsContentParent.childCount);
    }
}