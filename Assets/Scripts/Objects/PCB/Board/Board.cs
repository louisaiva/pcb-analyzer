
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Board
{
    private bool logs = false;
    public Vector2Int grid_size;
    public List<BoardCell> cells;


    // START
    public Board(int component_count, bool logs)
    {
        this.logs = logs;

        // cells = new Dictionary<Vector2Int, string>();
        cells = new List<BoardCell>();
        if (component_count <= 1) { grid_size = new Vector2Int(1, 1); }
        else if (component_count <= 8) { grid_size = new Vector2Int(3, 3); }
        else if (component_count <= 13) { grid_size = new Vector2Int(5, 3); }
        else if (component_count <= 22) { grid_size = new Vector2Int(5, 5); }
        else if (component_count <= 31) { grid_size = new Vector2Int(7, 5); }
        else { grid_size = new Vector2Int(7, 7); }

        generate_empty_grid();
        if (logs) { Debug.Log(get_grid_details()); }

    }
    private void generate_empty_grid()
    {
        cells.Clear();

        // calculate the center offset
        Vector2Int center_offset = Vector2Int.zero;
        if (grid_size.x == 3) { center_offset.x = -1; }
        else if (grid_size.x == 5) { center_offset.x = -2; }
        else if (grid_size.x == 7) { center_offset.x = -3; }
        if (grid_size.y == 3) { center_offset.y = -1; }
        else if (grid_size.y == 5) { center_offset.y = -2; }
        else if (grid_size.y == 7) { center_offset.y = -3; }

        if (logs) { Debug.Log($"(CHGH) Generating grid of size {grid_size} : center_offset is now {center_offset}"); }

        // creating the cells
        for (int y = 0; y < grid_size.y; y++)
        {
            for (int x = 0; x < grid_size.x; x++)
            {
                Vector2Int pos = new Vector2Int(x, y) + center_offset;
                if (logs) { Debug.Log($"(CHGH) Creating cell ({x},{y}) offseted in {pos}"); }
                // cells.Add(cell,"");
                BoardCell cell = new BoardCell()
                {
                    x = pos.x,
                    y = pos.y,
                };
                cells.Add(cell);
            }
        }
    }


    ///
    //
    ///  CELLS GENERATION
    //
    ///


    // MAIN METHODS
    public void OccupyCell(BoardCell cell, string comp_name)
    {
        // if (!TryGetCell(pos, out GridCell cell)) { return; }
        if (cell.type != CellType.Empty) { Debug.LogError($"(Board) Tried occupying the cell in {cell.Position} with {comp_name} but the it is a {cell.type} cell with value {cell.value} already !"); return; }
        cell.value = comp_name;
        cell.type = CellType.Component;
        if (logs) { Debug.Log(get_grid_details()); }
    }
    public void PositionComponentInFreeCell(Component comp, int cell_size, int inter_cell)
    {
        BoardCell cell = GetEmptyCell();
        if (cell == null) { return; }
        Vector2 pos = GetWorldPositionOfCell(cell, cell_size, inter_cell);

        comp.transform.localPosition = new Vector3(pos.x, pos.y, comp.transform.localPosition.z);
        comp.CellPosition = cell.Position;
        comp.RefreshWelds();
        if (logs) { Debug.Log($"(CHGH) Positionning {comp.name} in cell {cell.Position} at world position {pos}"); }
        OccupyCell(cell, comp.name);
    }
    public void TrimRecursively(int max_iterations = 100)
    {
        int count_before_touching_anything = cells.Count;
        bool continue_trimming = true;
        int iterations = 0;
        while (continue_trimming && iterations < max_iterations)
        {
            iterations++;
            int this_trimming = TrimOnce();
            if (this_trimming == 0) { continue_trimming = false; }
        }
        if (logs) { Debug.Log($"(Board) Recursive trimming removed {count_before_touching_anything - cells.Count} cells in {iterations} iterations. We now have only {cells.Count}"); }
    }
    public int TrimOnce()
    {
        List<BoardCell> to_delete = new List<BoardCell>();
        int count_before_triming = cells.Count;
        foreach (BoardCell cell in cells)
        {
            if (cell.type != CellType.Empty) { continue; }
            if (cell.Position == Vector2Int.zero) { continue; }

            int neighbours = GetNeighboursCount(cell);
            if (neighbours <= 2) { to_delete.Add(cell); }
        }
        while (to_delete.Count > 0)
        {
            cells.Remove(to_delete[0]);
            to_delete.RemoveAt(0);
        }
        if (logs) { Debug.Log($"(Board) Trimmed grid. Removed {count_before_triming - cells.Count} cells"); }
        return count_before_triming - cells.Count;
    }



    // GETTERS
    public Vector2 GetWorldPositionOfCell(BoardCell cell, int cell_size, int inter_cell)
    {
        return cell.GetWorldPosition(cell_size, inter_cell);
    }
    public bool TryGetCell(Vector2Int pos, out BoardCell cell)
    {
        cell = null;
        foreach (BoardCell other_cell in cells)
        {
            if (other_cell.Position == pos)
            {
                cell = other_cell;
                return true;
            }
        }
        return false;
    }
    public Vector2Int GetFreeCellPosition()
    {
        List<Vector2Int> free_cells = new List<Vector2Int>(); // cells.Where(c => string.IsNullOrEmpty(c.Value));
        foreach (BoardCell cell in cells)
        {
            if (cell.type != CellType.Empty) { continue; }
            free_cells.Add(cell.Position);
        }
        if (free_cells.Count == 0) { return new Vector2Int(-888, -888); }
        return free_cells[UnityEngine.Random.Range(0, free_cells.Count)];
    }
    public List<BoardCell> GetEmptyCells()
    {
        List<BoardCell> free_cells = new List<BoardCell>(); // cells.Where(c => string.IsNullOrEmpty(c.Value));
        foreach (BoardCell cell in cells)
        {
            if (cell.type != CellType.Empty) { continue; }
            free_cells.Add(cell);
        }
        return free_cells;
    }
    public BoardCell GetEmptyCell()
    {
        List<BoardCell> free_cells = GetEmptyCells();
        if (free_cells.Count == 0) { return null; }
        return free_cells[UnityEngine.Random.Range(0, free_cells.Count)];
    }
    private string get_grid_details()
    {
        string debug = $"(CHGH) GRID DETAILS : of size {grid_size}\n";
        foreach (BoardCell cell in cells)
        {
            debug += $"\n - {cell.Position} - {cell.type.ToString()}";
            if (!string.IsNullOrEmpty(cell.value)) { debug += " - " + cell.value; }
        }
        return debug;
    }


    // NEIGHBOURS GETTERS
    List<Vector2Int> neighbour_translations = new List<Vector2Int>()
    {
        Vector2Int.down,
        Vector2Int.left,
        Vector2Int.up,
        Vector2Int.right
    };
    public int GetNeighboursCount(BoardCell cell) => GetNeighbours(cell.Position, out List<Vector2Int> translations);
    public int GetNeighbours(Vector2Int position, out List<Vector2Int> translations)
    {
        translations = new List<Vector2Int>();
        int count = 0;
        foreach (Vector2Int translation in neighbour_translations)
        {
            if (!TryGetCell(position + translation, out BoardCell neighbour)) { continue; }
            // if (neighbour.type == CellType.Empty) { continue; }
            translations.Add(translation);
            count++;
        }
        return count;
    }

    List<Vector2Int> exterior_neighbour_translations = new List<Vector2Int>()
    {
        Vector2Int.up + Vector2Int.right,
        Vector2Int.down + Vector2Int.right,
        Vector2Int.down + Vector2Int.left,
        Vector2Int.up + Vector2Int.left,
    };
    public int GetExteriorNeighbours(Vector2Int position, out List<Vector2Int> translations)
    {
        translations = new List<Vector2Int>();
        int count = 0;
        foreach (Vector2Int translation in exterior_neighbour_translations)
        {
            if (!TryGetCell(position + translation, out BoardCell neighbour)) { continue; }
            translations.Add(translation);
            count++;
        }
        return count;
    }





    ///
    //
    ///  PCB BUILDING
    //
    ///


    // BUILD THE OUTLINE
    private List<Vector2> outlines_rails_pos = new List<Vector2>();
    internal void BuildPCB_Outline(ref List<SpriteRenderer> pcb_parts, InstantiaterParameter instantior)
    {
        outlines_rails_pos.Clear();

        int min_x = cells.Min(c => c.x);
        int max_x = cells.Max(c => c.x);
        int min_y = cells.Min(c => c.y);
        int max_y = cells.Max(c => c.y);

        Dictionary<Vector2Int, string> outline = new Dictionary<Vector2Int, string>();

        for (int i = min_x - 1; i < max_x + 2; i++)
        {
            for (int j = min_y - 1; j < max_y + 2; j++)
            {
                Vector2Int position = new Vector2Int(i, j);
                if (TryGetCell(position, out BoardCell cell)) { continue; }
                int neighbours = GetNeighbours(position, out List<Vector2Int> translations);

                if (neighbours == 0) { build_outline_corner_or_nothing(position, ref pcb_parts, instantior); }
                else if (neighbours == 1) { build_outline_edge(position, translations[0], ref pcb_parts, instantior); }
                else if (neighbours == 2) { build_outline_big_corner(position, translations, ref pcb_parts, instantior); }
                else
                {
                    Debug.LogWarning("(Board) Well we have to draw outlines for " + neighbours + " neighbours now : translations are " + string.Join("  //  ", translations));
                }
            }
        }

    }
    private void build_outline_corner_or_nothing(Vector2Int position, ref List<SpriteRenderer> pcb_parts, InstantiaterParameter instantior)
    {
        // for now we do nothing
        // later we want to check if we have a neighbour in the 8 neighbours
        int neighbours = GetExteriorNeighbours(position, out List<Vector2Int> translations);
        if (neighbours == 0) { return; } // we touch nothing, we do nothing
        if (neighbours != 1) // bad news we need to draw more things ?
        {
            Debug.LogWarning("(Board) Well we have to draw multiples corners for " + neighbours + " exterior neighbours now : translations are " + string.Join("  //  ", translations));
            return;
        }

        Sprite corner = BoardController.Instance.PCB_PartsBank.GetCornerMatchingWith(translations[0]);
        Vector2 worldpos = BoardCell.GetWorldPosition(position, PCB.cell_size, PCB.inter_cell);
        InstantiatePCB_Part(worldpos, corner, ref pcb_parts, instantior);
    }
    private void build_outline_edge(Vector2Int position, Vector2Int translation, ref List<SpriteRenderer> pcb_parts, InstantiaterParameter instantior)
    {
        Sprite edge = BoardController.Instance.PCB_PartsBank.GetEdgeMatchingWith(translation);
        Vector2 worldpos = BoardCell.GetWorldPosition(position, PCB.cell_size, PCB.inter_cell);
        InstantiatePCB_Part(worldpos, edge, ref pcb_parts, instantior);

        // we place 2 rails if we can
        float rail_translation = (PCB.cell_size + PCB.inter_cell) / 2f * (1f / 24f);
        if (translation.x == 0f)
        {
            Vector2 left_rail = worldpos - new Vector2(rail_translation, 0f);
            if (!outlines_rails_pos.Contains(left_rail))
            {
                Sprite rail = BoardController.Instance.PCB_PartsBank.GetOutlineRailEdge(translation);
                InstantiatePCB_Part(left_rail, rail, ref pcb_parts, instantior, is_rail: true);
                outlines_rails_pos.Add(left_rail);
            }
            Vector2 right_rail = worldpos + new Vector2(rail_translation, 0f);
            if (!outlines_rails_pos.Contains(right_rail))
            {
                Sprite rail = BoardController.Instance.PCB_PartsBank.GetOutlineRailEdge(translation);
                InstantiatePCB_Part(right_rail, rail, ref pcb_parts, instantior, is_rail: true);
                outlines_rails_pos.Add(right_rail);
            }
        }
        else if (translation.y == 0f)
        {
            Vector2 down_rail = worldpos - new Vector2(0f, rail_translation);
            if (!outlines_rails_pos.Contains(down_rail))
            {
                Sprite rail = BoardController.Instance.PCB_PartsBank.GetOutlineRailEdge(translation);
                InstantiatePCB_Part(down_rail, rail, ref pcb_parts, instantior, is_rail: true);
                outlines_rails_pos.Add(down_rail);
            }
            Vector2 up_rail = worldpos + new Vector2(0f, rail_translation);
            if (!outlines_rails_pos.Contains(up_rail))
            {
                Sprite rail = BoardController.Instance.PCB_PartsBank.GetOutlineRailEdge(translation);
                InstantiatePCB_Part(up_rail, rail, ref pcb_parts, instantior, is_rail: true);
                outlines_rails_pos.Add(up_rail);
            }
        }
    }
    private void build_outline_big_corner(Vector2Int position, List<Vector2Int> translations, ref List<SpriteRenderer> pcb_parts, InstantiaterParameter instantior)
    {
        Sprite big_corner = BoardController.Instance.PCB_PartsBank.GetBigCornerMatchingWith(translations);
        Vector2 worldpos = BoardCell.GetWorldPosition(position, PCB.cell_size, PCB.inter_cell);
        InstantiatePCB_Part(worldpos, big_corner, ref pcb_parts, instantior);


        // we place 2 rails if we can
        float rail_translation = (PCB.cell_size + PCB.inter_cell) / 2f * (1f / 24f);
        foreach (Vector2Int opposite_translation in translations)
        {
            Vector2Int translation = opposite_translation * new Vector2Int(-1, -1);
            Vector2 rail = worldpos + new Vector2(rail_translation * translation.x, rail_translation * translation.y);
            if (outlines_rails_pos.Contains(rail)) { continue; }
            Sprite sprite = BoardController.Instance.PCB_PartsBank.GetOutlineRailEdgeFromBigCorner(big_corner, translation);
            InstantiatePCB_Part(rail, sprite, ref pcb_parts, instantior, is_rail: true);
            outlines_rails_pos.Add(rail);
        }
    }
    private void InstantiatePCB_Part(Vector3 position, Sprite sprite, ref List<SpriteRenderer> pcb_parts, InstantiaterParameter instantior, bool is_rail = false)
    {
        SpriteRenderer sr = GameObject.Instantiate(instantior.prefab);
        sr.transform.SetParent(is_rail ? instantior.rails_parent : instantior.parts_parent);
        sr.sprite = sprite;
        sr.transform.localPosition = position;
        pcb_parts.Add(sr);
    }




    ///
    //
    ///  RAILS
    //
    ///

    private List<Rail> rails = new List<Rail>();
    private List<RailCross> rail_crosses = new List<RailCross>();

    public void AddRail(Rail rail, Vector2Int rail_pos, Axis axis)
    {
        rail.Init(rail_pos,axis);
        rails.Add(rail);
    }
    public void AddRailCross(RailCross rail_node, Vector2Int rail_pos)
    {
        rail_node.Init(rail_pos);
        rail_crosses.Add(rail_node);
    }
    public Rail GetWeldRail(Weld weld)
    {
        Vector2Int rail_pos = weld.RailPosition + weld.exit_direction;
        foreach (Rail rail in rails)
        {
            if (rail.RailPosition == rail_pos) { return rail; }
        }
        return null;
    }

    public List<RailNode> GetBoardRailNodes()
    {
        List<RailNode> nodes = new List<RailNode>();
        nodes.AddRange(rails);
        nodes.AddRange(rail_crosses);
        return nodes;
    }
}