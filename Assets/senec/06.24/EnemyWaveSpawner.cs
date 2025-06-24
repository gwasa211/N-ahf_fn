using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyWaveSpawner : MonoBehaviour
{
    [Header("스폰 설정")]
    public GameObject enemyPrefab;
    public float spawnInterval = 3f;
    public int spawnCount = 18;
    public Vector2Int areaSize = new Vector2Int(100, 100);
    public Vector2 offsetFromPlayer = new Vector2(20f, 20f);

    [Header("참조 (자동 할당)")]
    public Transform player;

    private List<Vector2> spawnPoints = new List<Vector2>();
    private List<GameObject> spawnedEnemies = new List<GameObject>();

    void Start()
    {
        AutoAssignPlayer();
        StartCoroutine(SpawnLoop());
    }

    void AutoAssignPlayer()
    {
        // 1) GameManager에 등록된 플레이어 우선
        if (GameManager.Instance != null && GameManager.Instance.player != null)
        {
            player = GameManager.Instance.player.transform;
            return;
        }
        // 2) 태그 "Player"가 붙은 객체 검색
        var go = GameObject.FindWithTag("Player");
        if (go != null)
        {
            player = go.transform;
            return;
        }
        // 3) FindObjectOfType fallback
        var pComp = FindObjectOfType<Player>();
        if (pComp != null)
            player = pComp.transform;
    }

    IEnumerator SpawnLoop()
    {
        while (true)
        {
            GenerateRandomSpawnPoints();
            SpawnEnemies();
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    void GenerateRandomSpawnPoints()
    {
        // player 참조가 없으면 자동 재할당 시도
        if (player == null)
            AutoAssignPlayer();
        if (player == null)
            return; // 여전히 없으면 생성 건너뛰기

        spawnPoints.Clear();
        Vector2 playerPos = player.position;

        for (int i = 0; i < spawnCount; i++)
        {
            Vector2 randomOffset = new Vector2(
                Random.Range(-areaSize.x / 2f, areaSize.x / 2f),
                Random.Range(-areaSize.y / 2f, areaSize.y / 2f)
            );

            // 플레이어 주변 offset보다 가까우면 재시도
            if (Mathf.Abs(randomOffset.x) < offsetFromPlayer.x &&
                Mathf.Abs(randomOffset.y) < offsetFromPlayer.y)
            {
                i--;
                continue;
            }

            spawnPoints.Add(playerPos + randomOffset);
        }
    }

    void SpawnEnemies()
    {
        if (player == null)
            AutoAssignPlayer();

        foreach (Vector2 pos in spawnPoints)
        {
            GameObject enemyGO = Instantiate(enemyPrefab, pos, Quaternion.identity);
            // 자동 타겟 할당
            if (enemyGO.TryGetComponent<Enemy>(out var enemyScript) && player != null)
            {
                enemyScript.target = player;
            }
            spawnedEnemies.Add(enemyGO);
        }

        // 죽은 몬스터 정리
        spawnedEnemies.RemoveAll(e => e == null);
    }
}
