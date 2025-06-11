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
