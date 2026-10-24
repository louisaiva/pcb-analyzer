using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// USEFUL NESTED CLASSES
public class BoardCell
{
    public int x;
    public int y;
    public Vector2Int Position => new Vector2Int(x, y);
    public Vector2Int RailPosition => Position * 2;
    public CellType type = CellType.Empty;
    public string value = "";

    public static Vector2 GetWorldPosition(Vector2Int position, int cell_size, int inter_cell)
    {
        float xpos = position.x * (cell_size + inter_cell) * (1f / 24f);
        float ypos = position.y * (cell_size + inter_cell) * (1f / 24f);
        return new Vector2(xpos, ypos);
    }
    public Vector2 GetWorldPosition(int cell_size, int inter_cell)
    {
        float xpos = x * (cell_size + inter_cell) * (1f / 24f);
        float ypos = y * (cell_size + inter_cell) * (1f / 24f);
        return new Vector2(xpos, ypos);
    }

    public List<Vector2> GetMiddleRailsWorldPositions(int cell_size, int inter_cell, out List<Vector2Int> rail_positions)
    {
        // 4 corners of the cell
        Vector2 world_cell = GetWorldPosition(cell_size, inter_cell);
        float translation = (cell_size + inter_cell) / 2f * (1f / 24f);

        List<Vector2> corners = new List<Vector2>
        {
            new Vector2(world_cell.x + translation, world_cell.y + translation), // UP RIGHT
            new Vector2(world_cell.x + translation, world_cell.y - translation), // DOWN RIGHT
            new Vector2(world_cell.x - translation, world_cell.y - translation), // D L
            new Vector2(world_cell.x - translation, world_cell.y + translation), // U L
        };

        rail_positions = new List<Vector2Int>
        {
            RailPosition + Vector2Int.up + Vector2Int.right,
            RailPosition + Vector2Int.down + Vector2Int.right,
            RailPosition + Vector2Int.down + Vector2Int.left,
            RailPosition + Vector2Int.up + Vector2Int.left,
        };

        return corners;
    }
    public List<Vector2> GetVerticalRailsWorldPositions(int cell_size, int inter_cell, out List<Vector2Int> rail_positions)
    {
        // left & right vertical things
        Vector2 world_cell = GetWorldPosition(cell_size, inter_cell);
        float translation = (cell_size + inter_cell) / 2f * (1f / 24f);
        List<Vector2> verts = new List<Vector2>
        {
            new Vector2(world_cell.x + translation, world_cell.y), // RIGHT
            new Vector2(world_cell.x - translation, world_cell.y), // LEFT
        };
        rail_positions = new List<Vector2Int>
        {
            RailPosition + Vector2Int.right,
            RailPosition + Vector2Int.left,
        };
        return verts;
    }
    public List<Vector2> GetHorizontalRailsWorldPositions(int cell_size, int inter_cell, out List<Vector2Int> rail_positions)
    {
        // u & d horizontal rails
        Vector2 world_cell = GetWorldPosition(cell_size, inter_cell);
        float translation = (cell_size + inter_cell) / 2f * (1f / 24f);
        List<Vector2> hors = new List<Vector2>
        {
            new Vector2(world_cell.x, world_cell.y + translation), // UP
            new Vector2(world_cell.x, world_cell.y - translation), // D
        };
        rail_positions = new List<Vector2Int>
        {
            RailPosition + Vector2Int.up,
            RailPosition + Vector2Int.down,
        };
        return hors;
    }

}
public enum CellType
{
    Empty,
    Component,
    Desc,
    Title
}