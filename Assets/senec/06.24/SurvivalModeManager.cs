using UnityEngine;
using TMPro;
using System.Collections;

public class SurvivalModeManager : MonoBehaviour
{
    [Header("Enemy Settings")]
    public GameObject[] enemyPrefabs;    // 반드시 3개
    public int spawnCount = 10;
    public float spawnRadius = 10f;
    public float spawnInterval = 3f;

    [Header("UI (TextMeshPro)")]
    [Tooltip("카운트다운 표시 전용")]
    public TextMeshProUGUI countdownText;
    [Tooltip("실시간 타이머 표시 전용")]
    public TextMeshProUGUI timerText;
    [Tooltip("최종 생존 시간 표시용")]
    public TextMeshProUGUI finalTimeText;

    private Transform player;
    private float elapsedTime;
    private bool isRunning;

    void Start()
    {
        // 플레이어 찾기
        if (GameManager.Instance?.player != null)
            player = GameManager.Instance.player.transform;
        else
            player = GameObject.FindWithTag("Player")?.transform;

        if (player == null)
        {
            Debug.LogError("SurvivalModeManager: Player를 찾을 수 없습니다.");
            enabled = false;
            return;
        }

        // UI 초기화
        countdownText.gameObject.SetActive(true);
        timerText.gameObject.SetActive(false);
        finalTimeText.gameObject.SetActive(false);
        countdownText.text = "";

        // 카운트다운 시퀀스 시작
        StartCoroutine(CountdownSequence());
    }

    IEnumerator CountdownSequence()
    {
        // 3 → 2 → 1
        for (int count = 3; count >= 1; count--)
        {
            countdownText.text = count.ToString();
            yield return new WaitForSeconds(1f);
        }

        // GO! 표시
        countdownText.text = "START!";
        yield return new WaitForSeconds(1f);

        // 카운트다운 숨기고 타이머 표시
        countdownText.gameObject.SetActive(false);
        timerText.gameObject.SetActive(true);
        elapsedTime = 0f;
        timerText.text = $"생존 시간 : {elapsedTime:0.00}초";

        // 서바이벌 모드 시작
        isRunning = true;
        StartCoroutine(SpawnLoop());
        StartCoroutine(UpdateTimer());
    }

    IEnumerator SpawnLoop()
    {
        // 첫 웨이브 즉시 스폰
        SpawnWave();

        while (isRunning)
        {
            yield return new WaitForSeconds(spawnInterval);
            SpawnWave();
        }
    }

    void SpawnWave()
    {
        if (player == null ||
            enemyPrefabs == null ||
            enemyPrefabs.Length != 3) return;

        for (int i = 0; i < spawnCount; i++)
        {
            var prefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];
            Vector2 offset = Random.insideUnitCircle * spawnRadius;
            var go = Instantiate(prefab, player.position + (Vector3)offset, Quaternion.identity);

            if (go.TryGetComponent<Enemy>(out var e))
                e.target = player;

            if (go.TryGetComponent<SkeletonArcher>(out var archer))
            {
                var fld = typeof(SkeletonArcher)
                    .GetField("player", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                fld?.SetValue(archer, player);
            }
        }
    }

    IEnumerator UpdateTimer()
    {
        while (isRunning)
        {
            elapsedTime += Time.deltaTime;
            timerText.text = $"생존 시간 : {elapsedTime:0.0}초";

            if (GameManager.Instance != null && GameManager.Instance.gameOverUI.activeSelf)
            {
                isRunning = false;
                ShowFinalTime();
                yield break;
            }

            yield return null;
        }
    }

    void ShowFinalTime()
    {
        finalTimeText.gameObject.SetActive(true);
        finalTimeText.text = $"최종 생존 시간: {elapsedTime:0.00}초";
    }
}
