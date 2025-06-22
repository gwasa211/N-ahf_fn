using UnityEngine;

public class UpgradeTrigger : MonoBehaviour
{

    public float upgradeAmount = 1f;

    public StatType statType = StatType.MeleeDamage; // 예시

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
                Debug.Log($"플레이어 능력치 {statType}가 {upgradeAmount}만큼 증가함!");
            }
        }
    }
}
