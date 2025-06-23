using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using TMPro;

public class MainMenuUI : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject noSavePopup;        // 저장 없음 팝업
    public TextMeshProUGUI popupText;     // 팝업 메시지 텍스트
    public string gameSceneName = "GameScene"; // 게임 플레이 씬 이름

    private string SavePath => Application.persistentDataPath + "/save.json";

    public void OnClickContinue()
    {
        if (File.Exists(SavePath))
        {
            SceneManager.LoadScene(gameSceneName);
        }
        else
        {
            ShowNoSavePopup("저장된 데이터가 없습니다.");
        }
    }

    public void OnClickNewGame()
    {
        if (File.Exists(SavePath))
            File.Delete(SavePath); // 기존 저장 삭제

        SceneManager.LoadScene(gameSceneName);
    }

    public void OnClickQuit()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    private void ShowNoSavePopup(string message)
    {
        if (noSavePopup != null)
            noSavePopup.SetActive(true);
        if (popupText != null)
            popupText.text = message;
    }

    public void ClosePopup()
    {
        if (noSavePopup != null)
            noSavePopup.SetActive(false);
    }
}
