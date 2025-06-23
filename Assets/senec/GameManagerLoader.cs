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
                gm.name = "GameManager (Auto)"; // ���� ���ϰ� �̸� ����
            }
            else
            {
                Debug.LogError("Resources/GameManager �������� ã�� �� �����ϴ�!");
            }
        }
    }
}
