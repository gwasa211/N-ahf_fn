using System.Collections.Generic;
using System.IO;
using UnityEngine;

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

        CropVisual[] crops = GameObject.FindObjectsOfType<CropVisual>();
        foreach (var crop in crops)
        {
            data.crops.Add(new CropData
            {
                cropID = crop.cropID,
                stage = crop.CurrentStage
            });
        }

        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(path, json);
    }

    public static void LoadPlayer(Player player)
    {
        if (!File.Exists(path))
        {
            Debug.LogWarning("저장 파일이 없음");
            return;
        }

        if (!File.Exists(path)) return;

   
        string json = File.ReadAllText(path);
        PlayerData data = JsonUtility.FromJson<PlayerData>(json);

        GameManager.Instance.currentMoney = data.money;

        player.bonusSwordDamage = data.bonusSwordDamage;
        player.bonusSwordRange = data.bonusSwordRange;
        player.bonusBowDamage = data.bonusBowDamage;
        player.bonusPierceCount = data.bonusPierceCount;
        player.bonusMaxHealth = data.bonusMaxHealth;
        player.bonusInvincibleTime = data.bonusInvincibleTime;

        player.RecalculateStats();


        player.currentHealth = Mathf.Clamp(data.health, 0, player.maxHealth);
        GameManager.Instance.UpdateHealthUI(player.currentHealth, player.maxHealth);

        CropVisual[] crops = GameObject.FindObjectsOfType<CropVisual>();
        foreach (var crop in crops)
        {
            var match = data.crops.Find(c => c.cropID == crop.cropID);
            if (match != null)
            {
                crop.SetStage(match.stage);
            }
        }

    }



}
