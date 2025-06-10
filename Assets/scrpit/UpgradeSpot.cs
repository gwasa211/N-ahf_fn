using UnityEngine;

public class UpgradeSpot : MonoBehaviour
{
    public int upgradeIndex;            // ����� ���׷��̵� �ε��� (UpgradeManager���� ����)
    public Crop crop;                  // ���׷��̵�� ����� �۹� ��ü (Crop.cs���� ����)
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
            Debug.Log("���׷��̵� �ִ� ���� ����");
            return;
        }

        int cost = UpgradeManager.Instance.GetUpgradeCost(upgradeIndex);

        if (GameManager.Instance.TrySpendMoney(cost))
        {
            UpgradeManager.Instance.Upgrade(upgradeIndex);

            if (crop != null)
                crop.LevelUp(); // �۹��� ������ �� �ִϸ��̼�/��ȭ

            GameManager.Instance.player.RecalculateStats(); // �÷��̾� �ɷ�ġ ����
        }
        else
        {
            Debug.Log("���� �����մϴ�!");
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
