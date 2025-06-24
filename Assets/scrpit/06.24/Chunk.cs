using UnityEngine;

public class Chunk : MonoBehaviour
{
    public int tileSize = 1;
    public GameObject grassTile;
    public GameObject waterTile;

    public void Generate(Vector2Int chunkCoord)
    {
        int chunkSize = 16;
        float scale = 10f;
        float seed = 1000f;

        for (int x = 0; x < chunkSize; x++)
        {
            for (int y = 0; y < chunkSize; y++)
            {
                float noiseX = (chunkCoord.x * chunkSize + x) / scale + seed;
                float noiseY = (chunkCoord.y * chunkSize + y) / scale + seed;
                float noise = Mathf.PerlinNoise(noiseX, noiseY);

                Vector3 pos = transform.position + new Vector3(x * tileSize, y * tileSize, 0);

                GameObject tileToSpawn = (noise > 0.5f) ? grassTile : waterTile;
                Instantiate(tileToSpawn, pos, Quaternion.identity, transform);
            }
        }
    }
}
