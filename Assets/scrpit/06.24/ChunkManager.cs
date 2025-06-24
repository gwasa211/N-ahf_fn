using System.Collections.Generic;
using UnityEngine;

public class ChunkManager : MonoBehaviour
{
    public Transform player;
    public GameObject chunkPrefab;
    public int chunkSize = 16;
    public int viewDistance = 2;

    private Dictionary<Vector2Int, GameObject> chunks = new Dictionary<Vector2Int, GameObject>();

    void Update()
    {
        Vector2Int currentChunk = WorldToChunkCoord(player.position);
        HashSet<Vector2Int> needed = new HashSet<Vector2Int>();

        for (int x = -viewDistance; x <= viewDistance; x++)
        {
            for (int y = -viewDistance; y <= viewDistance; y++)
            {
                Vector2Int coord = currentChunk + new Vector2Int(x, y);
                needed.Add(coord);

                if (!chunks.ContainsKey(coord))
                {
                    Vector3 spawnPos = new Vector3(coord.x * chunkSize, coord.y * chunkSize, 0);
                    GameObject chunk = Instantiate(chunkPrefab, spawnPos, Quaternion.identity);
                    chunk.name = $"Chunk {coord.x},{coord.y}";
                    chunk.GetComponent<Chunk>().Generate(coord); // 좌표 기반 지형 생성
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

    Vector2Int WorldToChunkCoord(Vector3 pos)
    {
        return new Vector2Int(
            Mathf.FloorToInt(pos.x / chunkSize),
            Mathf.FloorToInt(pos.y / chunkSize)
        );
    }
}
