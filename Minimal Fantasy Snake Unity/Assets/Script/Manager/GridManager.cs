using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    [Header("Grid Size")]
    [SerializeField] int width;
    public int Width => width;
    [SerializeField] int height;
    public int Height => height;
    [SerializeField] Tile tilePrefab;

    public Dictionary<Vector2, Tile> gridTiles = new Dictionary<Vector2, Tile>();

    public bool GenerateGrid()
    {
        if (gridTiles.Count > 0)
        {
            foreach (var tile in gridTiles.Values)
            {
                Destroy(tile.gameObject);
            }

            gridTiles.Clear();
        }

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector3 spawnPosition = new Vector3(x, 0, y);
                var tileObject = Instantiate(tilePrefab, spawnPosition, Quaternion.identity, transform);

                Vector2 gridPos = new Vector2(x, y);
                tileObject.Init(gridPos);
                gridTiles.Add(gridPos, tileObject);
            }
        }

        return true;
    }

    public bool IsPositionInGrid(Vector2 pos)
    {
        return pos.x >= 0 && pos.x < width && pos.y >= 0 && pos.y < height;
    }

    public Tile GetTileAtGridPosition(Vector2 gridPos)
    {
        if (gridTiles.ContainsKey(gridPos))
        {
            return gridTiles[gridPos];
        }

        return null;
    }
}
