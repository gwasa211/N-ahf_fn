using UnityEngine;
using UnityEngine.SceneManagement;

public class Bootstrapper : MonoBehaviour
{
    private static bool hasBootstrapped = false;
    private static bool hasCreatedGameMgr = false;

    [Header("부트스트랩 전용 씬")]
    public string bootstrapSceneName = "BootstrapScene";

    [Header("플레이용 씬")]
    public string gameplaySceneName = "senec/ingame";

    [Header("던전 씬들")]
    public string[] dungeonScenes = new string[]
    {
        "DungeonScene1",
        "DungeonScene2",
        "DungeonScene3",
        "DungeonScene"
    };

    [Header("프리팹 경로 (Resources/)")]
    public string gameManagerPath = "GameManager";

    void Awake()
    {


        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void Start()
    {
        string current = SceneManager.GetActiveScene().name;

        // 부트스트랩 씬에서만 최초 실행
        if (current == bootstrapSceneName && !hasBootstrapped)
        {
            hasBootstrapped = true;
            Debug.Log("✅ Bootstrapper: 부트스트랩 씬, 게임 시작");

            CreateGameManagerIfNeeded();
            SceneManager.LoadScene(gameplaySceneName, LoadSceneMode.Single);
        }
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        string name = scene.name;

        // 플레이용 씬에 진입했을 때 한 번만 GameManager 보장
        if (name == gameplaySceneName)
        {
            CreateGameManagerIfNeeded();
        }
        // **던전 씬에서는 절대 생성하지 않음**
    }

    void CreateGameManagerIfNeeded()
    {
        if (hasCreatedGameMgr) return;

        if (GameManager.Instance == null)
        {
            var prefab = Resources.Load<GameObject>(gameManagerPath);
            if (prefab != null)
            {
                var gm = Instantiate(prefab);
                gm.name = "GameManager";
                DontDestroyOnLoad(gm);
                Debug.Log("➡️ GameManager 인스턴스 생성");
            }
            else
            {
                Debug.LogError($"Resources/{gameManagerPath} 프리팹을 찾을 수 없습니다!");
            }
        }
        hasCreatedGameMgr = true;
    }
}
