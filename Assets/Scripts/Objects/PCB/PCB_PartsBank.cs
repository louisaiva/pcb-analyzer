using System;
using System.Collections.Generic;
using UnityEngine;

public class PCB_PartsBank : MonoBehaviour
{
    [Header("Cell Sprites")]
    [SerializeField] private Sprite full;
    [SerializeField] private Sprite edge_U;
    [SerializeField] private Sprite edge_R;
    [SerializeField] private Sprite edge_D;
    [SerializeField] private Sprite edge_L;
    [SerializeField] private Sprite corner_UR;
    [SerializeField] private Sprite corner_DR;
    [SerializeField] private Sprite corner_DL;
    [SerializeField] private Sprite corner_UL;
    [SerializeField] private Sprite big_corner_UR;
    [SerializeField] private Sprite big_corner_DR;
    [SerializeField] private Sprite big_corner_DL;
    [SerializeField] private Sprite big_corner_UL;


    [Header("Line Sprites")]
    [SerializeField] private Sprite middle;
    [SerializeField] private Sprite vertical;
    [SerializeField] private Sprite horizontal;
    [SerializeField] private Sprite line_edge_U;
    [SerializeField] private Sprite line_edge_R;
    [SerializeField] private Sprite line_edge_D;
    [SerializeField] private Sprite line_edge_L;


    public Sprite GetCellSprite(CellType type)
    {
        return full;
    }
    public Sprite GetLineMid()
    {
        return middle;
    }
    public Sprite GetLineVertical() { return vertical; }
    public Sprite GetLineHorizontal() { return horizontal; }


    internal Sprite GetEdgeMatchingWith(Vector2Int neighbour)
    {
        if (neighbour == Vector2Int.down) { return edge_U; }
        else if (neighbour == Vector2Int.left) { return edge_R; }
        else if (neighbour == Vector2Int.up) { return edge_D; }
        else if (neighbour == Vector2Int.right) { return edge_L; }
        return full;
    }

    internal Sprite GetBigCornerMatchingWith(List<Vector2Int> neighbours)
    {
        if (neighbours.Contains(Vector2Int.down) && neighbours.Contains(Vector2Int.left)) { return big_corner_UR; }
        else if (neighbours.Contains(Vector2Int.up) && neighbours.Contains(Vector2Int.left)) { return big_corner_DR; }
        else if (neighbours.Contains(Vector2Int.up) && neighbours.Contains(Vector2Int.right)) { return big_corner_DL; }
        else if (neighbours.Contains(Vector2Int.down) && neighbours.Contains(Vector2Int.right)) { return big_corner_UL; }
        return full;
    }

    internal Sprite GetOutlineRailEdge(Vector2Int neighbour)
    {
        if (neighbour == Vector2Int.down) { return line_edge_U; }
        else if (neighbour == Vector2Int.left) { return line_edge_R; }
        else if (neighbour == Vector2Int.up) { return line_edge_D; }
        else if (neighbour == Vector2Int.right) { return line_edge_L; }
        return full;
    }

    internal Sprite GetOutlineRailEdgeFromBigCorner(Sprite big_corner, Vector2Int translation)
    {
        if (big_corner == big_corner_UR)
        {
            if (translation == Vector2Int.up) { return line_edge_R; }
            if (translation == Vector2Int.right) { return line_edge_U; }
        }
        else if (big_corner == big_corner_DR)
        {
            if (translation == Vector2Int.down) { return line_edge_R; }
            if (translation == Vector2Int.right) { return line_edge_D; }
        }
        else if (big_corner == big_corner_DL)
        {
            if (translation == Vector2Int.down) { return line_edge_L; }
            if (translation == Vector2Int.left) { return line_edge_D; }
        }
        else if (big_corner == big_corner_UL)
        {
            if (translation == Vector2Int.up) { return line_edge_L; }
            if (translation == Vector2Int.left) { return line_edge_U; }
        }
        return full;
    }
}