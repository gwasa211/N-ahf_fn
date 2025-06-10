using UnityEngine;


public class CropLevelData : ScriptableObject
{
    public string description;       // 설명
    public int upgradeCost;          // 업그레이드 비용

    public LevelData[] levels;       // 각 단계별 수치

    [System.Serializable]
    public class LevelData
    {
        public float value;          // 업그레이드 수치
        public int cost;            // 각 레벨별 개별 비용 (선택 사항)
    }
}
