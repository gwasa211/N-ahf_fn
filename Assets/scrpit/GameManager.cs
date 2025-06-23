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
    public TextMeshProUGUI healthText;
    public GameObject gameOverUI;

    void Awake()
    {
        Debug.Log("GameManager Awake 호출됨");

        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Debug.Log("중복 GameManager 발견 -> 삭제");
            Destroy(gameObject);
            return;
        }

        Debug.Log("GameManager: Awake");
    }


    void Start()
    {
        UpdateMoneyUI();
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        player = FindObjectOfType<Player>();

        if (player != null)
        {
            SaveSystem.LoadPlayer(player);
        }

        // UI도 다시 찾기 (씬마다 다르니까)
        moneyText = GameObject.FindWithTag("MoneyText")?.GetComponent<TextMeshProUGUI>();
        healthText = GameObject.FindWithTag("HealthText")?.GetComponent<TextMeshProUGUI>();

        UpdateMoneyUI();
        UpdateHealthUI(player?.currentHealth ?? 0, player?.maxHealth ?? 0);
    }

    public void AddMoney(int amount)
    {
        currentMoney += amount;
        UpdateMoneyUI();
        SaveSystem.SavePlayer(player);
    }

    public bool TrySpendMoney(int cost)
    {
        if (currentMoney >= cost)
        {
            currentMoney -= cost;
            UpdateMoneyUI();
            SaveSystem.SavePlayer(player);
            return true;
        }
        return false;
    }

    public void UpdateMoneyUI()
    {
        if (moneyText != null)
            moneyText.text = $"현재 돈 : {currentMoney}";
        else
            Debug.LogWarning("moneyText가 연결되지 않았습니다.");
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
        Time.timeScale = 0f;
        gameOverUI?.SetActive(true);
    }

    public void RetryGame()
    {
        Time.timeScale = 1f;
        SaveSystem.SavePlayer(player);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ReturnToMainMenu()
    {
        Time.timeScale = 1f;
        SaveSystem.SavePlayer(player);
        SceneManager.LoadScene("MainMenu");
    }

    void OnDestroy()
    {
        Debug.LogWarning("GameManager 파괴됨!");
    }
}
