using UnityEngine;

public class GameManagerLoader : MonoBehaviour
{
    private void Awake()
    {
        if (GameManager.Instance == null)
        {
            GameObject prefab = Resources.Load<GameObject>("GameManager");
            if (prefab != null)
            {
                GameObject gm = Instantiate(prefab);
                gm.name = "GameManager (Auto)"; // 보기 편하게 이름 설정
            }
            else
            {
                Debug.LogError("Resources/GameManager 프리팹을 찾을 수 없습니다!");
            }
        }
    }
}
