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
        // 1) 플레이어가 W 누를 때만
        if (!other.CompareTag("Player") || !Input.GetKeyDown(KeyCode.W))
            return;

        // 2) GameManager.Instance 방어
        if (GameManager.Instance == null)
        {
            Debug.LogWarning("CropUpgradeTrigger: GameManager.Instance가 없습니다.");
            return;
        }

        int cost = GetCurrentCost();

        // 3) 돈 지불 시도
        if (!GameManager.Instance.TrySpendMoney(cost))
            return;

        // 4) targetCrops 방어: 배열이 null이거나 비어있으면 건너뜀
        if (targetCrops != null && targetCrops.Length > 0)
        {
            foreach (var crop in targetCrops)
            {
                if (crop == null)
                {
                    Debug.LogWarning("CropUpgradeTrigger: targetCrops에 null 요소가 있습니다.");
                    continue;
                }
                crop.AdvanceStage();
                crop.ShowFruitEffect();
            }
        }
        else
        {
            Debug.LogWarning("CropUpgradeTrigger: 연결된 작물이 없습니다.");
        }

        // 5) 플레이어 보너스 적용
        var player = other.GetComponent<Player>();
        if (player != null)
        {
            player.ApplyUpgrade(statType, upgradeAmount);
            player.RecalculateStats();
            GameManager.Instance.UpdateHealthUI(player.currentHealth, player.maxHealth);
        }
    }

    private int GetCurrentCost()
    {
        if (targetCrops == null || targetCrops.Length == 0)
            return baseCost;

        // 배열 첫 번째 작물 스테이지로 비용 계산
        return baseCost + targetCrops[0].CurrentStage * costIncrement;
    }
}
