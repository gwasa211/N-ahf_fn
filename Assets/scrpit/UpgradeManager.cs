using System.Collections.Generic;
using UnityEngine;

public class UpgradeManager : MonoBehaviour
{
    public static UpgradeManager Instance;

    [System.Serializable]
    public class UpgradeEntry
    {
        public string name;             // ���׷��̵� �̸� (UI ǥ�ÿ�)
        public UpgradeData data;        // ����� ScriptableObject
        public int currentLevel = 0;    // ���� ���׷��̵� ����
    }

    public List<UpgradeEntry> upgrades = new List<UpgradeEntry>();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    /// <summary>
    /// ���� ���׷��̵� ��ġ ��ȯ
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
    /// ���� ���׷��̵� ��� ��ȯ
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
    /// ���׷��̵� �������� Ȯ��
    /// </summary>
    public bool CanUpgrade(int index)
    {
        if (!IsValidIndex(index)) return false;

        var entry = upgrades[index];
        return entry.data.valuePerLevel.Length > entry.currentLevel;
    }

    /// <summary>
    /// ������ ���׷��̵� ����
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
        Debug.Log("RecalculateStats() ȣ���");
    }

}
