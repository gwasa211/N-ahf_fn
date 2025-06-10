using UnityEngine;

[CreateAssetMenu(menuName = "Crop/Level Data")]
public class CropLevelData : ScriptableObject
{
    public Sprite cropSprite;      // 작물 이미지
    public int upgradeCost;        // 업그레이드에 필요한 돈
    [TextArea] public string description; // 설명
}
