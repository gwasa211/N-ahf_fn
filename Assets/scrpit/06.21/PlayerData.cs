using System.Collections.Generic;

[System.Serializable]
public class PlayerData
{
    public int health;
    public int maxHealth;
    public int money;

    public int bonusSwordDamage;
    public float bonusSwordRange;
    public int bonusBowDamage;
    public int bonusPierceCount;
    public int bonusMaxHealth;
    public float bonusInvincibleTime;

    public float bonusDashDistance; //  추가

    public List<CropSaveData> cropStates = new List<CropSaveData>();

    public List<CropData> crops = new List<CropData>(); // 🌱 추가
}
