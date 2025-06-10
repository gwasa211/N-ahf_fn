using System.Collections.Generic;
using UnityEngine;

public class UpgradeManager : MonoBehaviour
{
    public static UpgradeManager Instance;

    [System.Serializable]
    public class UpgradeEntry
    {
        public string name;             // 업그레이드 이름 (UI 표시용)
        public UpgradeData data;        // 연결된 ScriptableObject
        public int currentLevel = 0;    // 현재 업그레이드 레벨
    }

    public List<UpgradeEntry> upgrades = new List<UpgradeEntry>();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    /// <summary>
    /// 현재 업그레이드 수치 반환
    /// </summary>
    public float GetUpgradeValue(int index)
    {
        if (!IsValidIndex(index)) return 0f;

        var entry = upgrades[index];
        return entry.data.valuePerLevel.Length > entry.currentLevel
            ? entry.data.valuePerLevel[entry.currentLevel]
            : 0f;
    }

    /// <summary>
    /// 현재 업그레이드 비용 반환
    /// </summary>
    public int GetUpgradeCost(int index)
    {
        if (!IsValidIndex(index)) return 0;

        var entry = upgrades[index];
        return entry.data.costPerLevel.Length > entry.currentLevel
            ? entry.data.costPerLevel[entry.currentLevel]
            : 0;
    }

    /// <summary>
    /// 업그레이드 가능한지 확인
    /// </summary>
    public bool CanUpgrade(int index)
    {
        if (!IsValidIndex(index)) return false;

        var entry = upgrades[index];
        return entry.data.valuePerLevel.Length > entry.currentLevel;
    }

    /// <summary>
    /// 실제로 업그레이드 수행
    /// </summary>
    public void Upgrade(int index)
    {
        if (CanUpgrade(index))
            upgrades[index].currentLevel++;
    }

    private bool IsValidIndex(int index)
    {
        return index >= 0 && index < upgrades.Count && upgrades[index].data != null;
    }

    void RecalculateStats()
    {
        Debug.Log("RecalculateStats() 호출됨");
    }

}
