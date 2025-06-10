using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuButten : MonoBehaviour
{
    [Header("버튼 오브젝트들")]
    public GameObject startButton;
    public GameObject exitButton;
    public GameObject recordButton;
    public GameObject settingsButton;

    [Header("팝업창들")]
    public GameObject recordPopup;
    public GameObject settingsPopup;

    [Header("팝업 닫기 버튼")]
    public GameObject closeRecordButton;
    public GameObject closeSettingsButton;

    [Header("이동할 씬 이름")]
    public string gameSceneName = "ingame";

    private void Start()
    {
        startButton.GetComponent<Button>().onClick.AddListener(StartGame);
        exitButton.GetComponent<Button>().onClick.AddListener(ExitGame);
        recordButton.GetComponent<Button>().onClick.AddListener(ToggleRecordPopup);
        settingsButton.GetComponent<Button>().onClick.AddListener(ToggleSettingsPopup);

        closeRecordButton.GetComponent<Button>().onClick.AddListener(CloseRecordPopup);
        closeSettingsButton.GetComponent<Button>().onClick.AddListener(CloseSettingsPopup);
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

    private void ToggleRecordPopup()
    {
        if (recordPopup != null)
            recordPopup.SetActive(!recordPopup.activeSelf);
    }

    private void ToggleSettingsPopup()
    {
        if (settingsPopup != null)
            settingsPopup.SetActive(!settingsPopup.activeSelf);
    }

    void CloseRecordPopup()
    {
        recordPopup.SetActive(false);
    }

    void CloseSettingsPopup()
    {
        settingsPopup.SetActive(false);
    }




}
