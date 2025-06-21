// GameManager.cs - 정리된 버전 (UpgradeManager 제거됨)

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
        SaveSystem.LoadPlayer(player); // 자동 불러오기
        UpdateMoneyUI();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            AddMoney(1000); // 디버그용 돈 추가
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            SaveSystem.SavePlayer(player); // 수동 저장
        }

        if (Input.GetKeyDown(KeyCode.O))
        {
            SaveSystem.LoadPlayer(player); // 수동 로드
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
