using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileMapChunkSpawner : MonoBehaviour
{
    public Transform player;
    public GameObject tilemapChunkPrefab;  // 100x100 ≈∏¿œ∏  «¡∏Æ∆’
    public int chunkSize = 100;

    private Vector2Int lastPlayerChunk;
    private Dictionary<Vector2Int, GameObject> spawnedChunks = new();

    void Update()
    {
        Vector2Int currentChunk = WorldToChunkCoord(player.position);
        if (currentChunk != lastPlayerChunk)
        {
            lastPlayerChunk = currentChunk;
            UpdateChunks(currentChunk);
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
        HashSet<Vector2Int> needed = new();

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                Vector2Int coord = centerChunk + new Vector2Int(x, y);
                needed.Add(coord);

                if (!spawnedChunks.ContainsKey(coord))
                {
                    Vector3 pos = new Vector3(coord.x * chunkSize, coord.y * chunkSize, 0);
                    GameObject chunk = Instantiate(tilemapChunkPrefab, pos, Quaternion.identity);
                    chunk.name = $"Chunk {coord.x},{coord.y}";
                    spawnedChunks.Add(coord, chunk);
                }
            }
        }

        List<Vector2Int> toRemove = new();
        foreach (var pair in spawnedChunks)
        {
            if (!needed.Contains(pair.Key))
            {
                Destroy(pair.Value);
                toRemove.Add(pair.Key);
            }
        }

        foreach (var coord in toRemove)
        {
            spawnedChunks.Remove(coord);
        }
    }
}
