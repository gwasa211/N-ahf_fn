using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    public float spawnInterval = 3f;
    public int spawnCount = 18;
    public Vector2 spawnAreaMin;
    public Vector2 spawnAreaMax;

    private float timer = 0f;
    private List<GameObject> spawnedEnemies = new List<GameObject>();

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= spawnInterval)
        {
            timer = 0f;
            SpawnEnemies();
        }

        // 죽은 적 정리
        spawnedEnemies.RemoveAll(e => e == null);
    }

    void SpawnEnemies()
    {
        if (GameManager.Instance == null || GameManager.Instance.player == null) return;

        Transform playerTransform = GameManager.Instance.player.transform;

        for (int i = 0; i < spawnCount; i++)
        {
            Vector2 randomPos = new Vector2(
                Random.Range(spawnAreaMin.x, spawnAreaMax.x),
                Random.Range(spawnAreaMin.y, spawnAreaMax.y)
            );

            GameObject enemy = Instantiate(enemyPrefab, randomPos, Quaternion.identity);
            Enemy enemyScript = enemy.GetComponent<Enemy>();
            if (enemyScript != null)
            {
                enemyScript.target = playerTransform; // ✅ 자동 타겟 설정
            }

            spawnedEnemies.Add(enemy);
        }
    }
}
