using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManagerLoader : MonoBehaviour
{
    [Header("GameManager ���� ��� ��")]
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
        if (!shouldRun) return;  // ���� ���� �ƴϸ� ����

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
                Debug.LogError("Resources/GameManager �������� ã�� �� �����ϴ�!");
            }
        }
    }
}
