using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

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

    // ✅ 작물 상태 저장용 딕셔너리
    public Dictionary<string, int> cropStages = new Dictionary<string, int>();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        UpdateMoneyUI();
    }

    public void RegisterPlayer(Player newPlayer)
    {
        player = newPlayer;
        UpdateHealthUI(player.currentHealth, player.maxHealth);
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
    }

    public void UpdateHealthUI(int currentHp, int maxHp)
    {
        if (healthText != null)
            healthText.text = $"HP: {currentHp} / {maxHp}";
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
        SceneManager.SetActiveScene(scene);
        Debug.Log("현재 활성 씬: " + SceneManager.GetActiveScene().name);

        player = FindObjectOfType<Player>();
        if (player != null)
            SaveSystem.LoadPlayer(player);

        moneyText = GameObject.FindWithTag("MoneyText")?.GetComponent<TextMeshProUGUI>();
        healthText = GameObject.FindWithTag("HealthText")?.GetComponent<TextMeshProUGUI>();

        GameObject[] allObjects = Resources.FindObjectsOfTypeAll<GameObject>();
        foreach (GameObject obj in allObjects)
        {
            if (obj.CompareTag("GameOverUI"))
            {
                gameOverUI = obj;
                break;
            }
        }

        if (gameOverUI != null)
        {
            gameOverUI.SetActive(false);
        }
        else
        {
            Debug.LogWarning("GameOverUI 오브젝트를 찾지 못했습니다.");
        }

        UpdateMoneyUI();
        UpdateHealthUI(player?.currentHealth ?? 0, player?.maxHealth ?? 0);

        // ✅ 작물 상태 복원
        foreach (var crop in GameObject.FindObjectsOfType<CropVisual>())
        {
            int savedStage = GetCropStage(crop.cropID);
            crop.SetStage(savedStage);
        }
    }

    // ✅ 작물 상태 저장용 getter
    public Dictionary<string, int> GetAllCropStages()
    {
        return new Dictionary<string, int>(cropStages);
    }

    // ✅ 작물 상태 갱신
    public void SetCropStage(string cropID, int stage)
    {
        cropStages[cropID] = stage;
    }

    // ✅ 저장된 작물 스테이지 반환
    public int GetCropStage(string cropID)
    {
        return cropStages.ContainsKey(cropID) ? cropStages[cropID] : 0;
    }
}
