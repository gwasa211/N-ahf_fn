using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    public float spawnInterval = 5f;
    public int maxEnemies = 5;

    private float timer;
    private List<GameObject> spawnedEnemies = new List<GameObject>();

    void Update()
    {
        timer += Time.deltaTime;

        // 일정 수 미만일 때만 생성
        if (timer >= spawnInterval && spawnedEnemies.Count < maxEnemies)
        {
            timer = 0f;
            GameObject enemy = Instantiate(enemyPrefab, transform.position, Quaternion.identity);
            spawnedEnemies.Add(enemy);
        }

        // 죽은 몬스터 정리
        spawnedEnemies.RemoveAll(e => e == null);
    }
}
