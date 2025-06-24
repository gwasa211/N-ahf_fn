using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuNavigator : MonoBehaviour
{
    [Header("Optional: UI Click Sound")]
    // UIAudioManager를 쓰고 있다면 할당해 두고
    public UIAudioManager uiAudioManager;

    // 씬 이름 그대로 쓰세요 (Build Settings에 추가되어 있어야 합니다)
    [Header("Main Menu Scene Name")]
    public string mainMenuSceneName = "MainMenu";

    /// <summary>
    /// 버튼 OnClick()에 연결하세요.
    /// 클릭 사운드를 재생한 뒤 메인 메뉴 씬으로 전환합니다.
    /// </summary>
    public void GoToMainMenu()
    {
        // 클릭음 재생 (UIAudioManager를 사용 중이라면)
        if (uiAudioManager != null)
            uiAudioManager.PlayClick();

        // 씬 전환
        SceneManager.LoadScene(mainMenuSceneName);
    }
}
