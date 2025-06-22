using UnityEngine;

public class UpgradeTrigger : MonoBehaviour
{

    public float upgradeAmount = 1f;

    public StatType statType = StatType.MeleeDamage; // ����

    void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player") && Input.GetKeyDown(KeyCode.W))
        {
            Player player = other.GetComponent<Player>();
            if (player != null && GameManager.Instance.TrySpendMoney(100))
            {
                player.ApplyUpgrade(statType, upgradeAmount);
                player.RecalculateStats();
                GameManager.Instance.UpdateHealthUI(player.currentHealth, player.maxHealth);
                Debug.Log($"�÷��̾� �ɷ�ġ {statType}�� {upgradeAmount}��ŭ ������!");
            }
        }
    }
}
