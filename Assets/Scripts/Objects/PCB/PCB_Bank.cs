using System.Collections.Generic;
using UnityEngine;

public class PCB_Bank : MonoBehaviour
{
    public List<PCB_Skin> skins = new List<PCB_Skin>();

    public PCB_Skin GetSkin(string name)
    {
        foreach (PCB_Skin skin in skins)
        {
            if (skin.name == name) { return skin; }
        }
        return null;
    }

    public Sprite GetCellSprite(CellType type, string skin = "back")
    {
        return GetSkin(skin).full;
    }
    public Sprite GetLineMid(string skin = "back")
    {
        return GetSkin(skin).middle;
    }
    public Sprite GetLineVertical(string skin = "back") { return GetSkin(skin).vertical; }
    public Sprite GetLineHorizontal(string skin = "back") { return GetSkin(skin).horizontal; }

    internal Sprite GetEdgeMatchingWith(Vector2Int neighbour, string skin = "back")
    {
        if (neighbour == Vector2Int.down) { return GetSkin(skin).edge_U; }
        else if (neighbour == Vector2Int.left) { return GetSkin(skin).edge_R; }
        else if (neighbour == Vector2Int.up) { return GetSkin(skin).edge_D; }
        else if (neighbour == Vector2Int.right) { return GetSkin(skin).edge_L; }
        return GetSkin(skin).full;
    }
    internal Sprite GetBigCornerMatchingWith(List<Vector2Int> neighbours, string skin = "back")
    {
        if (neighbours.Contains(Vector2Int.down) && neighbours.Contains(Vector2Int.left)) { return GetSkin(skin).big_corner_UR; }
        else if (neighbours.Contains(Vector2Int.up) && neighbours.Contains(Vector2Int.left)) { return GetSkin(skin).big_corner_DR; }
        else if (neighbours.Contains(Vector2Int.up) && neighbours.Contains(Vector2Int.right)) { return GetSkin(skin).big_corner_DL; }
        else if (neighbours.Contains(Vector2Int.down) && neighbours.Contains(Vector2Int.right)) { return GetSkin(skin).big_corner_UL; }
        return GetSkin(skin).full;
    }
    internal Sprite GetOutlineRailEdge(Vector2Int neighbour, string skin = "back")
    {
        if (neighbour == Vector2Int.down) { return GetSkin(skin).line_edge_U; }
        else if (neighbour == Vector2Int.left) { return GetSkin(skin).line_edge_R; }
        else if (neighbour == Vector2Int.up) { return GetSkin(skin).line_edge_D; }
        else if (neighbour == Vector2Int.right) { return GetSkin(skin).line_edge_L; }
        return GetSkin(skin).full;
    }
    internal Sprite GetOutlineRailEdgeFromBigCorner(Sprite big_corner, Vector2Int translation, string skin = "back")
    {
        if (big_corner == GetSkin(skin).big_corner_UR)
        {
            if (translation == Vector2Int.up) { return GetSkin(skin).line_edge_R; }
            if (translation == Vector2Int.right) { return GetSkin(skin).line_edge_U; }
        }
        else if (big_corner == GetSkin(skin).big_corner_DR)
        {
            if (translation == Vector2Int.down) { return GetSkin(skin).line_edge_R; }
            if (translation == Vector2Int.right) { return GetSkin(skin).line_edge_D; }
        }
        else if (big_corner == GetSkin(skin).big_corner_DL)
        {
            if (translation == Vector2Int.down) { return GetSkin(skin).line_edge_L; }
            if (translation == Vector2Int.left) { return GetSkin(skin).line_edge_D; }
        }
        else if (big_corner == GetSkin(skin).big_corner_UL)
        {
            if (translation == Vector2Int.up) { return GetSkin(skin).line_edge_L; }
            if (translation == Vector2Int.left) { return GetSkin(skin).line_edge_U; }
        }
        return GetSkin(skin).full;
    }
    internal Sprite GetCornerMatchingWith(Vector2Int neighbour, string skin = "back")
    {
        if (neighbour == new Vector2Int(-1, -1)) { return GetSkin(skin).corner_UR; }
        if (neighbour == new Vector2Int(-1, 1)) { return GetSkin(skin).corner_DR; }
        if (neighbour == new Vector2Int(1, 1)) { return GetSkin(skin).corner_DL; }
        if (neighbour == new Vector2Int(1, -1)) { return GetSkin(skin).corner_UL; }
        return GetSkin(skin).full;
    }

}