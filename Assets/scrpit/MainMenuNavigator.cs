using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuNavigator : MonoBehaviour
{
    [Header("Optional: UI Click Sound")]
    // UIAudioManager�� ���� �ִٸ� �Ҵ��� �ΰ�
    public UIAudioManager uiAudioManager;

    // �� �̸� �״�� ������ (Build Settings�� �߰��Ǿ� �־�� �մϴ�)
    [Header("Main Menu Scene Name")]
    public string mainMenuSceneName = "MainMenu";

    /// <summary>
    /// ��ư OnClick()�� �����ϼ���.
    /// Ŭ�� ���带 ����� �� ���� �޴� ������ ��ȯ�մϴ�.
    /// </summary>
    public void GoToMainMenu()
    {
        // Ŭ���� ��� (UIAudioManager�� ��� ���̶��)
        if (uiAudioManager != null)
            uiAudioManager.PlayClick();

        // �� ��ȯ
        SceneManager.LoadScene(mainMenuSceneName);
    }
}
