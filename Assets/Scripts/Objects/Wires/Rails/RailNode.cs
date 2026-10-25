using UnityEngine;

public class RailNode : MonoBehaviour
{
    public Vector2Int RailPosition;
    public virtual bool Full => false;
    public Vector2 WorldPosition => transform.position;
}