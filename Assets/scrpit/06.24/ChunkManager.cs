using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkManager : MonoBehaviour
{
    public Transform player;
    public GameObject chunkPrefab;
    public GameObject enemyPrefab;

    public int chunkSize = 16;

    private Dictionary<Vector2Int, GameObject> chunks = new Dictionary<Vector2Int, GameObject>();
    private Vector2Int lastPlayerChunk = new Vector2Int(int.MinValue, int.MinValue);

    [Header("스폰 설정")]
    public int spawnCount = 18;
    public float spawnInterval = 3f;

    void Start()
    {
        StartCoroutine(SpawnLoop());
    }

    void Update()
    {
        Vector2Int currentChunk = WorldToChunkCoord(player.position);

        if (currentChunk != lastPlayerChunk)
        {
            UpdateChunks(currentChunk);
            lastPlayerChunk = currentChunk;
        }
    }

    Vector2Int WorldToChunkCoord(Vector3 pos)
    {
        return new Vector2Int(
            Mathf.FloorToInt(pos.x / chunkSize),
            Mathf.FloorToInt(pos.y / chunkSize)
        );
    }

    void UpdateChunks(Vector2Int centerChunk)
    {
        HashSet<Vector2Int> needed = new HashSet<Vector2Int>();

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                Vector2Int coord = centerChunk + new Vector2Int(x, y);
                needed.Add(coord);

                if (!chunks.ContainsKey(coord))
                {
                    Vector3 spawnPos = new Vector3(coord.x * chunkSize, coord.y * chunkSize, 0);
                    GameObject chunk = Instantiate(chunkPrefab, spawnPos, Quaternion.identity);
                    chunk.name = $"Chunk {coord.x},{coord.y}";
                    chunk.GetComponent<Chunk>().Generate(coord);
                    chunks.Add(coord, chunk);
                }
            }
        }

        // 멀어진 청크 제거
        List<Vector2Int> toRemove = new List<Vector2Int>();
        foreach (var chunk in chunks)
        {
            if (!needed.Contains(chunk.Key))
            {
                Destroy(chunk.Value);
                toRemove.Add(chunk.Key);
            }
        }

        foreach (var coord in toRemove)
        {
            chunks.Remove(coord);
        }
    }

    // =====================
    // 랜덤 스폰 기능
    // =====================
    IEnumerator SpawnLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);
            SpawnInVisibleChunks();
        }
    }

    void SpawnInVisibleChunks()
    {
        List<Vector3> candidatePositions = new List<Vector3>();

        foreach (var kvp in chunks)
        {
            GameObject chunk = kvp.Value;
            Vector3 chunkPos = chunk.transform.position;

            for (int x = 0; x < chunkSize; x++)
            {
                for (int y = 0; y < chunkSize; y++)
                {
                    Vector3 pos = chunkPos + new Vector3(x, y, 0);
                    candidatePositions.Add(pos);
                }
            }
        }

        Shuffle(candidatePositions);

        int spawnTotal = Mathf.Min(spawnCount, candidatePositions.Count);
        for (int i = 0; i < spawnTotal; i++)
        {
            Instantiate(enemyPrefab, candidatePositions[i], Quaternion.identity);
        }
    }

    void Shuffle<T>(List<T> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }
    }
}
