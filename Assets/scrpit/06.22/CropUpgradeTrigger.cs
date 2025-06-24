using UnityEngine;

public class CropUpgradeTrigger : MonoBehaviour
{
    [Header("����� �۹���")]
    public CropVisual[] targetCrops;

    [Header("���׷��̵� ����")]
    public StatType statType;
    public float upgradeAmount = 1f;

    [Header("��� ����")]
    public int baseCost = 100;
    public int costIncrement = 50;

    void OnTriggerStay2D(Collider2D other)
    {
        // 1) �÷��̾ W ���� ����
        if (!other.CompareTag("Player") || !Input.GetKeyDown(KeyCode.W))
            return;

        // 2) GameManager.Instance ���
        if (GameManager.Instance == null)
        {
            Debug.LogWarning("CropUpgradeTrigger: GameManager.Instance�� �����ϴ�.");
            return;
        }

        int cost = GetCurrentCost();

        // 3) �� ���� �õ�
        if (!GameManager.Instance.TrySpendMoney(cost))
            return;

        // 4) targetCrops ���: �迭�� null�̰ų� ��������� �ǳʶ�
        if (targetCrops != null && targetCrops.Length > 0)
        {
            foreach (var crop in targetCrops)
            {
                if (crop == null)
                {
                    Debug.LogWarning("CropUpgradeTrigger: targetCrops�� null ��Ұ� �ֽ��ϴ�.");
                    continue;
                }
                crop.AdvanceStage();
                crop.ShowFruitEffect();
            }
        }
        else
        {
            Debug.LogWarning("CropUpgradeTrigger: ����� �۹��� �����ϴ�.");
        }

        // 5) �÷��̾� ���ʽ� ����
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

        // �迭 ù ��° �۹� ���������� ��� ���
        return baseCost + targetCrops[0].CurrentStage * costIncrement;
    }
}
