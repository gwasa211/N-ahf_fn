using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using TMPro;

public class MainMenuUI : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject noSavePopup; // 저장 없음 팝업
    public TextMeshProUGUI popupText;     // 경고 텍스트

    private string savePath => Application.persistentDataPath + "/save.json";

    public void OnClickContinue()
    {
        if (File.Exists(savePath))
        {
            SceneManager.LoadScene("GameScene"); // 게임 씬 이름으로 수정
        }
        else
        {
            ShowNoSavePopup("저장된 파일이 없습니다.");
        }
    }

    public void OnClickNewGame()
    {
        if (File.Exists(savePath))
            File.Delete(savePath); // 새 게임이면 기존 저장 삭제

        SceneManager.LoadScene("GameScene");
    }

    public void OnClickQuit()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    void ShowNoSavePopup(string message)
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
