using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuButten : MonoBehaviour
{
    [Header("��ư ������Ʈ��")]
    public GameObject startButton;
    public GameObject exitButton;
    public GameObject recordButton;
    public GameObject settingsButton;

    [Header("�˾�â��")]
    public GameObject recordPopup;
    public GameObject settingsPopup;

    [Header("�˾� �ݱ� ��ư")]
    public GameObject closeRecordButton;
    public GameObject closeSettingsButton;

    [Header("�̵��� �� �̸�")]
    public string gameSceneName = "ingame";

    private void Start()
    {
        startButton.GetComponent<Button>().onClick.AddListener(StartGame);
        exitButton.GetComponent<Button>().onClick.AddListener(ExitGame);

    }

    public void StartGame()
    {
        SceneManager.LoadScene(gameSceneName);
    }

    public void ExitGame()
    {
        Application.Quit();
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

   
   
}
