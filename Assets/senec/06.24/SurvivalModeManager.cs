using UnityEngine;
using TMPro;
using System.Collections;

public class SurvivalModeManager : MonoBehaviour
{
    [Header("Enemy Settings")]
    public GameObject[] enemyPrefabs;    // �ݵ�� 3��
    public int spawnCount = 10;
    public float spawnRadius = 10f;
    public float spawnInterval = 3f;

    [Header("UI (TextMeshPro)")]
    [Tooltip("ī��Ʈ�ٿ� ǥ�� ����")]
    public TextMeshProUGUI countdownText;
    [Tooltip("�ǽð� Ÿ�̸� ǥ�� ����")]
    public TextMeshProUGUI timerText;
    [Tooltip("���� ���� �ð� ǥ�ÿ�")]
    public TextMeshProUGUI finalTimeText;

    private Transform player;
    private float elapsedTime;
    private bool isRunning;

    void Start()
    {
        // �÷��̾� ã��
        if (GameManager.Instance?.player != null)
            player = GameManager.Instance.player.transform;
        else
            player = GameObject.FindWithTag("Player")?.transform;

        if (player == null)
        {
            Debug.LogError("SurvivalModeManager: Player�� ã�� �� �����ϴ�.");
            enabled = false;
            return;
        }

        // UI �ʱ�ȭ
        countdownText.gameObject.SetActive(true);
        timerText.gameObject.SetActive(false);
        finalTimeText.gameObject.SetActive(false);
        countdownText.text = "";

        // ī��Ʈ�ٿ� ������ ����
        StartCoroutine(CountdownSequence());
    }

    IEnumerator CountdownSequence()
    {
        // 3 �� 2 �� 1
        for (int count = 3; count >= 1; count--)
        {
            countdownText.text = count.ToString();
            yield return new WaitForSeconds(1f);
        }

        // GO! ǥ��
        countdownText.text = "START!";
        yield return new WaitForSeconds(1f);

        // ī��Ʈ�ٿ� ����� Ÿ�̸� ǥ��
        countdownText.gameObject.SetActive(false);
        timerText.gameObject.SetActive(true);
        elapsedTime = 0f;
        timerText.text = $"���� �ð� : {elapsedTime:0.00}��";

        // �����̹� ��� ����
        isRunning = true;
        StartCoroutine(SpawnLoop());
        StartCoroutine(UpdateTimer());
    }

    IEnumerator SpawnLoop()
    {
        // ù ���̺� ��� ����
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
            timerText.text = $"���� �ð� : {elapsedTime:0.0}��";

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
        finalTimeText.text = $"���� ���� �ð�: {elapsedTime:0.00}��";
    }
}
