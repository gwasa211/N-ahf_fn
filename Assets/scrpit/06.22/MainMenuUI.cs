using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using TMPro;

public class MainMenuUI : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject noSavePopup;        // ���� ���� �˾�
    public TextMeshProUGUI popupText;     // �˾� �޽��� �ؽ�Ʈ
    public string gameSceneName = "GameScene"; // ���� �÷��� �� �̸�

    private string SavePath => Application.persistentDataPath + "/save.json";

    public void OnClickContinue()
    {
        if (File.Exists(SavePath))
        {
            SceneManager.LoadScene(gameSceneName);
        }
        else
        {
            ShowNoSavePopup("����� �����Ͱ� �����ϴ�.");
        }
    }

    public void OnClickNewGame()
    {
        if (File.Exists(SavePath))
            File.Delete(SavePath); // ���� ���� ����

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
