using UnityEngine;

[CreateAssetMenu(fileName = "NewUpgradeData", menuName = "Upgrades/Upgrade Data")]
public class UpgradeData : ScriptableObject
{
    public string upgradeName;
    public float[] valuePerLevel;
    public int[] costPerLevel;
    public UpgradeLevelData[] levels;

}
