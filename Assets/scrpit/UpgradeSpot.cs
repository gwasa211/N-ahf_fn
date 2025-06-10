using UnityEngine;

public class UpgradeSpot : MonoBehaviour
{
    public int upgradeIndex;            // 연결된 업그레이드 인덱스 (UpgradeManager에서 설정)
    public Crop crop;                  // 업그레이드와 연결된 작물 객체 (Crop.cs에서 정의)
    private bool isPlayerInRange = false;

    void Update()
    {
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.W))
        {
            TryUpgrade();
        }
    }

    void TryUpgrade()
    {
        if (!UpgradeManager.Instance.CanUpgrade(upgradeIndex))
        {
            Debug.Log("업그레이드 최대 레벨 도달");
            return;
        }

        int cost = UpgradeManager.Instance.GetUpgradeCost(upgradeIndex);

        if (GameManager.Instance.TrySpendMoney(cost))
        {
            UpgradeManager.Instance.Upgrade(upgradeIndex);

            if (crop != null)
                crop.LevelUp(); // 작물도 레벨업 시 애니메이션/변화

            GameManager.Instance.player.RecalculateStats(); // 플레이어 능력치 갱신
        }
        else
        {
            Debug.Log("돈이 부족합니다!");
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            isPlayerInRange = true;
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            isPlayerInRange = false;
    }
}
