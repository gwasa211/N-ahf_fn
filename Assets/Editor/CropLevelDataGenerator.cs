using UnityEditor;
using UnityEngine;

public class CropLevelDataGenerator
{
    [MenuItem("Tools/Generate Crop Level Data")]
    public static void GenerateCropLevels()
    {
        int levelCount = 4; // 원하는 단계 수
        string folderPath = "Assets/CropLevelData";

        // 폴더 없으면 생성
        if (!AssetDatabase.IsValidFolder(folderPath))
        {
            AssetDatabase.CreateFolder("Assets", "CropLevelData");
        }

        for (int i = 1; i <= levelCount; i++)
        {
            CropLevelData levelData = ScriptableObject.CreateInstance<CropLevelData>();
            levelData.upgradeCost = 50 * i;
            levelData.description = $"작물 {i}단계 설명입니다.";

            string assetPath = $"{folderPath}/CropLevel_{i}.asset";
            AssetDatabase.CreateAsset(levelData, assetPath);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log($"{levelCount}개의 CropLevelData 생성 완료!");
    }
}
