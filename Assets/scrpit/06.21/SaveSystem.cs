using System.IO;
using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class CropSaveData
{
    public string cropID;
    public int stage;
}

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

    public List<CropSaveData> crops = new List<CropSaveData>();
}

public static class SaveSystem
{
    private static string path => Application.persistentDataPath + "/save.json";

    public static void SavePlayer(Player player)
    {
        PlayerData data = new PlayerData
        {
            health = player.currentHealth,
            maxHealth = player.maxHealth,
            money = GameManager.Instance.currentMoney,
            bonusSwordDamage = player.bonusSwordDamage,
            bonusSwordRange = player.bonusSwordRange,
            bonusBowDamage = player.bonusBowDamage,
            bonusPierceCount = player.bonusPierceCount,
            bonusMaxHealth = player.bonusMaxHealth,
            bonusInvincibleTime = player.bonusInvincibleTime
        };

        // 작물 정보 저장
        var crops = GameObject.FindObjectsOfType<CropVisual>();
        foreach (var crop in crops)
        {
            data.crops.Add(new CropSaveData
            {
                cropID = crop.cropID,
                stage = crop.CurrentStage
            });
        }

        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(path, json);
        Debug.Log("저장 완료: " + path);
    }

    public static void LoadPlayer(Player player)
    {
        if (!File.Exists(path))
        {
            Debug.LogWarning("저장 파일이 없음");
            return;
        }

        string json = File.ReadAllText(path);
        PlayerData data = JsonUtility.FromJson<PlayerData>(json);

        GameManager.Instance.currentMoney = data.money;
        GameManager.Instance.UpdateMoneyUI();

        player.bonusSwordDamage = data.bonusSwordDamage;
        player.bonusSwordRange = data.bonusSwordRange;
        player.bonusBowDamage = data.bonusBowDamage;
        player.bonusPierceCount = data.bonusPierceCount;
        player.bonusMaxHealth = data.bonusMaxHealth;
        player.bonusInvincibleTime = data.bonusInvincibleTime;

        player.RecalculateStats();
        player.currentHealth = Mathf.Clamp(data.health, 0, player.maxHealth);
        GameManager.Instance.UpdateHealthUI(player.currentHealth, player.maxHealth);

        // 작물 복원
        var crops = GameObject.FindObjectsOfType<CropVisual>();
        foreach (var savedCrop in data.crops)
        {
            foreach (var crop in crops)
            {
                if (crop.cropID == savedCrop.cropID)
                {
                    crop.SetStage(savedCrop.stage);
                }
            }
        }
    }
}
