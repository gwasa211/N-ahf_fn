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

    [Header("Game Over UI (자동 연결)")]
    public GameObject gameOverUI;
    public TextMeshProUGUI finalMoneyText;

    // 작물 상태 저장용
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
        // --- 1) GameOverUI & FinalMoneyText 자동 연결 ---
        gameOverUI = null;
        finalMoneyText = null;
        foreach (var go in Resources.FindObjectsOfTypeAll<GameObject>())
        {
            if (go.CompareTag("GameOverUI"))
            {
                gameOverUI = go;
                break;
            }
        }
        if (gameOverUI != null)
        {
            var tmps = gameOverUI.GetComponentsInChildren<TextMeshProUGUI>(true);
            foreach (var tmp in tmps)
                if (tmp.gameObject.name == "FinalMoneyText")
                    finalMoneyText = tmp;
            gameOverUI.SetActive(false);
        }
        else Debug.LogWarning("GameOverUI 태그를 찾을 수 없습니다.");

        // --- 2) 돈/체력 UI 자동 연결 ---
        moneyText = null;
        healthText = null;
        foreach (var tmp in Resources.FindObjectsOfTypeAll<TextMeshProUGUI>())
        {
            if (tmp.CompareTag("MoneyText")) moneyText = tmp;
            if (tmp.CompareTag("HealthText")) healthText = tmp;
        }

        // --- 3) 새로 로드된 씬 내부에서만 Player 찾기 ---
        player = null;
        if (scene.isLoaded)
        {
            foreach (var root in scene.GetRootGameObjects())
            {
                if (root.CompareTag("Player"))
                {
                    player = root.GetComponent<Player>();
                    break;
                }
            }
        }
        if (player == null)
        {
            Debug.LogWarning($"씬 '{scene.name}'에서 Player를 찾지 못했습니다.");
        }
        else
        {
            // 저장 데이터 로드
            SaveSystem.LoadPlayer(player);
        }

        // --- 4) UI 초기 업데이트 ---
        UpdateMoneyUI();
        UpdateHealthUI(player?.currentHealth ?? 0, player?.maxHealth ?? 0);

        // --- 5) 작물 상태 복원 ---
        foreach (var crop in GameObject.FindObjectsOfType<CropVisual>())
        {
            int stage = GetCropStage(crop.cropID);
            crop.SetStage(stage);
        }
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
        if (finalMoneyText != null)
            finalMoneyText.text = $"획득 금액: {currentMoney}";
        if (gameOverUI != null)
            gameOverUI.SetActive(true);
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

    public Dictionary<string, int> GetAllCropStages()
    {
        return new Dictionary<string, int>(cropStages);
    }

    public void SetCropStage(string cropID, int stage)
    {
        cropStages[cropID] = stage;
    }

    public int GetCropStage(string cropID)
    {
        return cropStages.ContainsKey(cropID) ? cropStages[cropID] : 0;
    }
}
