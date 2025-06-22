using UnityEngine;

public class Tile : MonoBehaviour
{
    public Vector2 gridPosition { get; private set; }

    public void Init(Vector2 gridPos)
    {
        gridPosition = gridPos;
        gameObject.name = $"Tile_{gridPos.x:f0}_{gridPos.y:f0}";
    }
}
