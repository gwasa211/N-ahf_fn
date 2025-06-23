using UnityEngine;

public class Bootstrapper : MonoBehaviour
{
    void Awake()
    {
        if (GameManager.Instance == null)
        {
            GameObject gm = Instantiate(Resources.Load<GameObject>("GameManager"));
            gm.name = "GameManager"; // (Optional) 이름에서 "(Clone)" 제거
        }
    }
}
