using UnityEngine;
using UnityEngine.SceneManagement;

public class Bootstrapper : MonoBehaviour
{
    private static bool hasLoadedGame = false;

    void Start()
    {
        if (!hasLoadedGame)
        {
            hasLoadedGame = true; // 씬 한 번만 로드하도록 설정

            Debug.Log("✅ Bootstrapper Start - 씬 전환 시도");

            if (GameManager.Instance == null)
            {
                GameObject gm = Instantiate(Resources.Load<GameObject>("GameManager"));
                gm.name = "GameManager";
            }

            SceneManager.LoadScene("senec/ingame", LoadSceneMode.Single);
        }
    }
}
