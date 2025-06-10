using UnityEngine;

[CreateAssetMenu(menuName = "Crop/Level Data")]
public class CropLevelData : ScriptableObject
{
    public Sprite cropSprite;      // �۹� �̹���
    public int upgradeCost;        // ���׷��̵忡 �ʿ��� ��
    [TextArea] public string description; // ����
}
