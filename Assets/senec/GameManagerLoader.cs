using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManagerLoader : MonoBehaviour
{
    [Header("GameManager 생성 허용 씬")]
    public string[] allowedScenes = new string[]
    {
        "BootstrapScene",
        "senec/ingame"
    };

    void Awake()
    {
        string current = SceneManager.GetActiveScene().name;
        bool shouldRun = false;
        foreach (var s in allowedScenes)
        {
            if (s == current)
            {
                shouldRun = true;
                break;
            }
        }
        if (!shouldRun) return;  // 허용된 씬이 아니면 종료

        if (GameManager.Instance == null)
        {
            GameObject prefab = Resources.Load<GameObject>("GameManager");
            if (prefab != null)
            {
                GameObject gm = Instantiate(prefab);
                gm.name = "GameManager (Auto)";
                DontDestroyOnLoad(gm);
            }
            else
            {
                Debug.LogError("Resources/GameManager 프리팹을 찾을 수 없습니다!");
            }
        }
    }
}
