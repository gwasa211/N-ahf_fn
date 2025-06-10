using UnityEditor;
using UnityEngine;

public class CropLevelDataGenerator
{
    [MenuItem("Tools/Generate Crop Level Data")]
    public static void GenerateCropLevels()
    {
        int levelCount = 4; // ���ϴ� �ܰ� ��
        string folderPath = "Assets/CropLevelData";

        // ���� ������ ����
        if (!AssetDatabase.IsValidFolder(folderPath))
        {
            AssetDatabase.CreateFolder("Assets", "CropLevelData");
        }

        for (int i = 1; i <= levelCount; i++)
        {
            CropLevelData levelData = ScriptableObject.CreateInstance<CropLevelData>();
            levelData.upgradeCost = 50 * i;
            levelData.description = $"�۹� {i}�ܰ� �����Դϴ�.";

            string assetPath = $"{folderPath}/CropLevel_{i}.asset";
            AssetDatabase.CreateAsset(levelData, assetPath);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log($"{levelCount}���� CropLevelData ���� �Ϸ�!");
    }
}
