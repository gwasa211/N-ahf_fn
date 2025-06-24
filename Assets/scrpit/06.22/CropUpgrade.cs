using UnityEngine;

public class CropUpgrade : MonoBehaviour
{
    public StatType statType;              // 업그레이드할 스탯 종류
    public float upgradeAmount = 1f;

    public Sprite[] growthStages;          // 0~3단계 작물 스프라이트
    public GameObject fruitPrefab;         // 열매 프리팹 (4단계 효과용)

    private int currentStage = 0;
    private SpriteRenderer sr;
    private bool isMaxed = false;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        sr.sprite = growthStages[0]; // 1단계로 초기화
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
                    sr.sprite = growthStages[currentStage]; // 스프라이트 변경
                }

                if (currentStage >= growthStages.Length)
                {
                    // 열매 튀어오르기 + 업그레이드
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
            rb.AddForce(Vector2.up * 2f, ForceMode2D.Impulse); // 튀어오르기 연출
        }

        Destroy(fruit, 1f); // 1초 후 열매 제거
    }
}
