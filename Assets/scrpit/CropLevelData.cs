using UnityEngine;


public class CropLevelData : ScriptableObject
{
    public string description;       // ����
    public int upgradeCost;          // ���׷��̵� ���

    public LevelData[] levels;       // �� �ܰ躰 ��ġ

    [System.Serializable]
    public class LevelData
    {
        public float value;          // ���׷��̵� ��ġ
        public int cost;            // �� ������ ���� ��� (���� ����)
    }
}
