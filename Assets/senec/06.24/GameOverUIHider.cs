using UnityEngine;

public class GameOverUIHider : MonoBehaviour
{
    void Awake()
    {
        gameObject.SetActive(false);
    }
}
