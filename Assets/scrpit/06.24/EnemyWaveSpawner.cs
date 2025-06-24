using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyWaveSpawner : MonoBehaviour
{
    [Header("스폰 설정")]
    public GameObject enemyPrefab;
    public float spawnInterval = 3f;
    public int spawnCount = 18;
    public Vector2Int areaSize = new Vector2Int(100, 100); // 맵 범위
    public Vector2 offsetFromPlayer = new Vector2(20f, 20f); // 플레이어와의 거리 여유

    [Header("참조")]
    public Transform player;

    private List<Vector2> spawnPoints = new List<Vector2>();
    private List<GameObject> spawnedEnemies = new List<GameObject>();

    void Start()
    {
        if (player == null && GameManager.Instance != null)
        {
            player = GameManager.Instance.player.transform;
        }

        StartCoroutine(SpawnLoop());
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
        spawnPoints.Clear();

        for (int i = 0; i < spawnCount; i++)
        {
            Vector2 randomPos = new Vector2(
                Random.Range(-areaSize.x / 2f, areaSize.x / 2f),
                Random.Range(-areaSize.y / 2f, areaSize.y / 2f)
            );

            Vector2 playerPos = player.position;
            Vector2 spawnPos = playerPos + randomPos;

            // 플레이어 주변 offset보다 가까운 위치 제외
            if (Mathf.Abs(randomPos.x) < offsetFromPlayer.x && Mathf.Abs(randomPos.y) < offsetFromPlayer.y)
            {
                i--;
                continue;
            }

            spawnPoints.Add(spawnPos);
        }
    }

    void SpawnEnemies()
    {
        foreach (Vector2 pos in spawnPoints)
        {
            GameObject enemy = Instantiate(enemyPrefab, pos, Quaternion.identity);

            // ✅ 생성한 적에게 타겟 할당
            Enemy enemyScript = enemy.GetComponent<Enemy>();
            if (enemyScript != null && player != null)
            {
                enemyScript.target = player;
            }

            spawnedEnemies.Add(enemy);
        }

        // 이미 죽은 몬스터 정리
        spawnedEnemies.RemoveAll(e => e == null);
    }

}
