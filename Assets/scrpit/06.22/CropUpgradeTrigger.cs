using UnityEngine;

public class CropUpgradeTrigger : MonoBehaviour
{
    [Header("연결된 작물들")]
    public CropVisual[] targetCrops;

    [Header("업그레이드 설정")]
    public StatType statType;
    public float upgradeAmount = 1f;

    [Header("비용 설정")]
    public int baseCost = 100;
    public int costIncrement = 50;

    void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player") && Input.GetKeyDown(KeyCode.W))
        {
            int cost = GetCurrentCost();

            if (GameManager.Instance.TrySpendMoney(cost))
            {
                foreach (var crop in targetCrops)
                {
                    crop.AdvanceStage();
                    crop.ShowFruitEffect();
                }

                Player player = other.GetComponent<Player>();
                if (player != null)
                {
                    player.ApplyUpgrade(statType, upgradeAmount);
                    player.RecalculateStats();
                    GameManager.Instance.UpdateHealthUI(player.currentHealth, player.maxHealth);
                }
            }
        }
    }

    private int GetCurrentCost()
    {
        if (targetCrops.Length == 0) return baseCost;
        return baseCost + targetCrops[0].CurrentStage * costIncrement;
    }
}
