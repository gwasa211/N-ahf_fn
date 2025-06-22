using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using TMPro;

public class MainMenuUI : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject noSavePopup; // ���� ���� �˾�
    public TextMeshProUGUI popupText;     // ��� �ؽ�Ʈ

    private string savePath => Application.persistentDataPath + "/save.json";

    public void OnClickContinue()
    {
        if (File.Exists(savePath))
        {
            SceneManager.LoadScene("GameScene"); // ���� �� �̸����� ����
        }
        else
        {
            ShowNoSavePopup("����� ������ �����ϴ�.");
        }
    }

    public void OnClickNewGame()
    {
        if (File.Exists(savePath))
            File.Delete(savePath); // �� �����̸� ���� ���� ����

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
