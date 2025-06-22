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

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // ✅ 씬 전환에도 유지
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        // 첫 로딩 시 자동 불러오기
        SaveSystem.LoadPlayer(player);
        UpdateMoneyUI();
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 새 씬에서 플레이어 다시 찾고 로드
        Player newPlayer = FindObjectOfType<Player>();
        if (newPlayer != null)
        {
            player = newPlayer;
            SaveSystem.LoadPlayer(player);
        }
    }

    private void Update()
    {
#if UNITY_EDITOR
        // 디버그 단축키 (에디터 전용)
        if (Input.GetKeyDown(KeyCode.P))
            AddMoney(1000);

        if (Input.GetKeyDown(KeyCode.L))
            SaveSystem.SavePlayer(player);

        if (Input.GetKeyDown(KeyCode.O))
            SaveSystem.LoadPlayer(player);
#endif
    }

    public void AddMoney(int amount)
    {
        currentMoney += amount;
        UpdateMoneyUI();
        SaveSystem.SavePlayer(player); // ✅ 자동 저장 추가
    }

    public bool TrySpendMoney(int cost)
    {
        if (currentMoney >= cost)
        {
            currentMoney -= cost;
            UpdateMoneyUI();
            SaveSystem.SavePlayer(player); // ✅ 자동 저장 추가
            return true;
        }
        return false;
    }


    public void UpdateMoneyUI()
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
        Time.timeScale = 0f;
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
        SceneManager.LoadScene("MainMenu"); // 메인 메뉴 씬 이름으로 변경
    }
}
