using UnityEngine;
using UnityEngine.Tilemaps;

public class Chunk : MonoBehaviour
{
    public int chunkSize = 50;
    public Tilemap tilemap;
    public TileBase grassTile;
    public TileBase waterTile;

    public void Generate(Vector2Int chunkCoord)
    {
        float scale = 10f;
        float seed = 1000f;

        tilemap.ClearAllTiles();

        for (int x = 0; x < chunkSize; x++)
        {
            for (int y = 0; y < chunkSize; y++)
            {
                float noiseX = (chunkCoord.x * chunkSize + x) / scale + seed;
                float noiseY = (chunkCoord.y * chunkSize + y) / scale + seed;
                float noise = Mathf.PerlinNoise(noiseX, noiseY);

                Vector3Int tilePos = new Vector3Int(x, y, 0);
                TileBase tile = (noise > 0.5f) ? grassTile : waterTile;

                tilemap.SetTile(tilePos, tile);
            }
        }
    }
}
