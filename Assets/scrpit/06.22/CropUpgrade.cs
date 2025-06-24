using UnityEngine;

public class CropUpgrade : MonoBehaviour
{
    public StatType statType;              // ���׷��̵��� ���� ����
    public float upgradeAmount = 1f;

    public Sprite[] growthStages;          // 0~3�ܰ� �۹� ��������Ʈ
    public GameObject fruitPrefab;         // ���� ������ (4�ܰ� ȿ����)

    private int currentStage = 0;
    private SpriteRenderer sr;
    private bool isMaxed = false;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        sr.sprite = growthStages[0]; // 1�ܰ�� �ʱ�ȭ
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (isMaxed) return;

        if (other.CompareTag("Player") && Input.GetKey(KeyCode.W))

            {
                if (GameManager.Instance.TrySpendMoney(100))
            {
                currentStage++;

                if (currentStage < growthStages.Length)
                {
                    sr.sprite = growthStages[currentStage]; // ��������Ʈ ����
                }

                if (currentStage >= growthStages.Length)
                {
                    // ���� Ƣ������� + ���׷��̵�
                    SpawnFruitEffect();

                    Player player = other.GetComponent<Player>();
                    player.ApplyUpgrade(statType, upgradeAmount);
                    player.RecalculateStats();
                    GameManager.Instance.UpdateHealthUI(player.currentHealth, player.maxHealth);

                    isMaxed = true;
                }
            }
        }
    }

    void SpawnFruitEffect()
    {
        Vector3 spawnPos = transform.position + new Vector3(0f, 0.5f, 0f);
        GameObject fruit = Instantiate(fruitPrefab, spawnPos, Quaternion.identity);

        Rigidbody2D rb = fruit.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.AddForce(Vector2.up * 2f, ForceMode2D.Impulse); // Ƣ������� ����
        }

        Destroy(fruit, 1f); // 1�� �� ���� ����
    }
}
