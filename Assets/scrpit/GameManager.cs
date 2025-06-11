using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("플레이어 참조")]
    public Player player;

    [Header("돈 관련")]
    public int currentMoney;
    public TextMeshProUGUI moneyText;

    [Header("체력 관련")]
    public TextMeshProUGUI healthText; // HP 텍스트

    public GameObject gameOverUI;


    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        UpdateMoneyUI();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            AddMoney(1000);
        }
    }

    public void AddMoney(int amount)
    {
        currentMoney += amount;
        UpdateMoneyUI();
    }

    public bool TrySpendMoney(int cost)
    {
        if (currentMoney >= cost)
        {
            currentMoney -= cost;
            UpdateMoneyUI();
            return true;
        }
        return false;
    }

    void UpdateMoneyUI()
    {
        if (moneyText != null)
            moneyText.text = $"현재 돈 : {currentMoney}";
        else
            Debug.LogWarning("moneyText가 연결되어 있지 않습니다.");
    }

    public void UpdateHealthUI(int currentHp, int maxHp)
    {
        if (healthText != null)
            healthText.text = $"HP: {currentHp} / {maxHp}";
        else
            Debug.LogWarning("healthText가 연결되지 않았습니다.");
    }


    public bool TryUpgrade(int upgradeIndex)
    {
        var upgradeManager = UpgradeManager.Instance;

        if (upgradeIndex < 0 || upgradeIndex >= upgradeManager.upgrades.Count)
            return false;

        var entry = upgradeManager.upgrades[upgradeIndex];
        var data = entry.data;

        if (data == null || data.valuePerLevel == null || data.costPerLevel == null)
            return false;

        int currentLevel = entry.currentLevel;
        if (currentLevel >= data.valuePerLevel.Length)
            return false;

        int cost = data.costPerLevel[currentLevel];

        if (TrySpendMoney(cost))
        {
            entry.currentLevel++;

            player.RecalculateStats(); // ✅ 이거 반드시 있어야 함!
            GameManager.Instance.UpdateHealthUI(player.currentHealth, player.maxHealth); // ✅ UI 갱신

            return true;
        }

        return false;
    }

    public void PlayerDied()
    {
        Time.timeScale = 0f; // 게임 일시정지
        gameOverUI.SetActive(true);
    }

    public void RetryGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ReturnToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu"); // 메인 메뉴 씬 이름에 맞게 수정
    }

}
