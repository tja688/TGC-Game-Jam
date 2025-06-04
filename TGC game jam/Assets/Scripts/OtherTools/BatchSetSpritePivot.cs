using UnityEditor;
using UnityEngine;
// System.IO 不是必需的，可以移除
// using System.IO;

public class BatchSetSpritePivot : EditorWindow
{
    [MenuItem("Tools/Batch Set Sprite Pivot (Force Custom)")]
    public static void ShowWindow()
    {
        GetWindow<BatchSetSpritePivot>("Batch Set Sprite Pivot");
    }

    private Vector2 pivot = new Vector2(0.5f, 0.5f); // 默认中心点

    private void OnGUI()
    {
        GUILayout.Label("为选中的精灵图强制设置自定义中心点", EditorStyles.boldLabel);
        pivot = EditorGUILayout.Vector2Field("中心点坐标 (Pivot Position)", pivot);

        if (GUILayout.Button("应用中心点到选中的精灵图"))
        {
            SetPivotForSelectedSprites();
        }
    }

    private void SetPivotForSelectedSprites()
    {
        // 获取选中的 Texture2D 资源
        Object[] selectedAssets = Selection.GetFiltered(typeof(Texture2D), SelectionMode.DeepAssets);
        int modifiedCount = 0;

        if (selectedAssets.Length == 0)
        {
            Debug.LogWarning("没有选中任何 Texture2D 资源。请在项目窗口中选择一个或多个精灵图。");
            return;
        }

        // 显示进度条
        EditorUtility.DisplayProgressBar("批量设置中心点", "正在处理精灵图...", 0f);

        for (int i = 0; i < selectedAssets.Length; i++)
        {
            Object obj = selectedAssets[i];
            string path = AssetDatabase.GetAssetPath(obj);
            TextureImporter textureImporter = AssetImporter.GetAtPath(path) as TextureImporter;

            if (textureImporter != null)
            {
                textureImporter.textureType = TextureImporterType.Sprite;
                textureImporter.spriteImportMode = SpriteImportMode.Single;
            
                // 关键修改：通过 SpriteMetaData 设置 Custom 模式
                TextureImporterSettings settings = new TextureImporterSettings();
                textureImporter.ReadTextureSettings(settings);
            
                settings.spriteAlignment = (int)SpriteAlignment.Custom; // 强制设为 Custom
                settings.spritePivot = pivot; // 设置自定义轴心点
            
                textureImporter.SetTextureSettings(settings);
                AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
            
                modifiedCount++;
            }
            else
            {
                Debug.LogWarning($"无法获取 TextureImporter: {path}");
            }
        }

        // 清除进度条
        EditorUtility.ClearProgressBar();

        if (modifiedCount > 0)
        {
            Debug.Log($"成功为 {modifiedCount} 个精灵图更新了中心点。新的中心点为: {pivot}");
        }
        else if (selectedAssets.Length > 0)
        {
            Debug.LogWarning("没有精灵图被修改。请检查选中的资源是否为有效的 Texture2D 且可以被导入为 Sprite。");
        }
        // 如果 selectedAssets.Length == 0，前面已经有提示了
    }
}