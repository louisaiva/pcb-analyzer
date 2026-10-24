using UnityEngine;

public class RailNode : MonoBehaviour
{

    public Vector2Int RailPosition;

    public void Init(Vector2Int position)
    {
        RailPosition = position;
    }
}