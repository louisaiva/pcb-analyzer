using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class PCB : MonoBehaviour
{

    [Header("References")]
    public ComponentsManager Componer;

    public static int cell_size = 20;
    public static int inter_cell = 7;


    [Header("Generation Parameters")]
    public PCB_GenerationParameter gen_para;
    public SpriteRenderer pcb_part_prefab;
    [SerializeField] private Transform back_parts;
    [SerializeField] private Transform back_rails;
    [SerializeField] private List<SpriteRenderer> pcb_parts = new List<SpriteRenderer>();


    // PCB GENERATION
    public async void Generate()
    {
        Componer.DestroyExistingComponents();
        while (pcb_parts.Count > 0)
        {
            Destroy(pcb_parts[0].gameObject);
            pcb_parts.RemoveAt(0);
        }

        // wait to ensure the components are really destroyed
        await Task.Yield();
        await Task.Yield();

        // choose a random number from the gen parameters
        int comp_to_generate = UnityEngine.Random.Range(gen_para.components_min_max.x, gen_para.components_min_max.y+1);
        Board board = new Board(comp_to_generate);

        for (int i=0; i<comp_to_generate; i++)
        {
            Component comp = BoardController.Instance.CompBank.CreateRandomComponent(Componer.transform);
            board.PositionComponentInFreeCell(comp, PCB.cell_size, PCB.inter_cell);
        }

        // generate the pcb board
        generate_pcb_board(board);

        Componer.GrabExistingComponents();
    }

    private void generate_pcb_board(Board board)
    {
        // we trim the grid so we have the smallest pcb
        board.TrimRecursively(1);

        // we place wires & rocks

        // we place description & title in an empty cell
        // we place screws ?

        // we build the grid + rails
        List<Vector2> rails_placed = new List<Vector2>();
        foreach (GridCell cell in board.cells)
        {
            // if (cell)
            SpriteRenderer sr = create_pcb_part(BoardController.Instance.PCB_PartsBank.GetCellSprite(cell.type), cell);
            pcb_parts.Add(sr);
            build_rails_around(cell, ref rails_placed);
        }

        // we build the outline
        InstantiaterParameter instantiator = new InstantiaterParameter
        {
            prefab = pcb_part_prefab,
            parts_parent = back_parts,
            rails_parent = back_rails
        };
        board.BuildPCB_Outline(ref pcb_parts, instantiator);

    }
    private void build_rails_around(GridCell cell, ref List<Vector2> placed_rails)
    {
        // we build the rails of this
        foreach (Vector2 pos in cell.GetMiddleRailsWorldPositions(PCB.cell_size, PCB.inter_cell))
        {
            if (placed_rails.Contains(pos)) { continue; }
            SpriteRenderer sr = create_pcb_line(BoardController.Instance.PCB_PartsBank.GetLineMid(), pos);
            pcb_parts.Add(sr);
            placed_rails.Add(pos);
        }
        foreach (Vector2 pos in cell.GetVerticalRailsWorldPositions(PCB.cell_size, PCB.inter_cell))
        {
            if (placed_rails.Contains(pos)) { continue; }
            SpriteRenderer sr = create_pcb_line(BoardController.Instance.PCB_PartsBank.GetLineVertical(), pos);
            pcb_parts.Add(sr);
            placed_rails.Add(pos);
        }
        foreach (Vector2 pos in cell.GetHorizontalRailsWorldPositions(PCB.cell_size, PCB.inter_cell))
        {
            if (placed_rails.Contains(pos)) { continue; }
            SpriteRenderer sr = create_pcb_line(BoardController.Instance.PCB_PartsBank.GetLineHorizontal(), pos);
            pcb_parts.Add(sr);
            placed_rails.Add(pos);
        }
    }
    private SpriteRenderer create_pcb_line(Sprite sprite, Vector3 position, bool is_back = true)
    {
        SpriteRenderer sr = Instantiate(pcb_part_prefab);
        sr.transform.SetParent(is_back ? back_rails : front_pcb.transform);
        sr.sprite = sprite;
        sr.transform.localPosition = position;
        return sr;
    }
    private SpriteRenderer create_pcb_part(Sprite sprite, GridCell cell, bool is_back=true)
    {
        SpriteRenderer sr = Instantiate(pcb_part_prefab);
        sr.transform.SetParent( is_back ? back_parts : front_pcb.transform );
        sr.sprite = sprite;
        sr.transform.localPosition = cell.GetWorldPosition(PCB.cell_size, PCB.inter_cell);
        return sr;
    }



    [Header("Animations")]
    [SerializeField] private Animator front_pcb;
    [SerializeField] private Animator back_pcb;
    [SerializeField] private Animator components_pcb;
    [SerializeField] private Animator buttons_pcb;
    [SerializeField] private bool showing_front = true;
    public bool IsShowingFront => showing_front;
    private Coroutine current_coroutine = null;


    // PCB FLIPPING
    public void Flip()
    {
        if (current_coroutine != null) { return; }

        // ensure we don't have any screws
        if (GetComponentInChildren<Screw>(includeInactive:false))
        {
            current_coroutine = StartCoroutine("UnscrewThenShowFront");
            return;
        }

        if (showing_front)
        {
            current_coroutine = StartCoroutine("ShowBackCoroutine");
            return;
        }
        current_coroutine = StartCoroutine("ShowFrontCoroutine");
    }
    public IEnumerator ShowFrontCoroutine()
    {
        Debug.Log($"Showing FRONT of {name}");

        front_pcb.Play("show");
        back_pcb.Play("hide");
        components_pcb.Play("hide");
        buttons_pcb.Play("hide");

        while (front_pcb.GetCurrentAnimatorStateInfo(0).IsName("show"))
        {
            yield break;
        }

        showing_front = true;
        current_coroutine = null;
        Debug.Log($"PCB {name} is now FRONT");
    }
    public IEnumerator ShowBackCoroutine()
    {
        Debug.Log($"Showing BACK of {name}");

        front_pcb.Play("hide");
        back_pcb.Play("show");
        components_pcb.Play("show");
        buttons_pcb.Play("show");

        while (front_pcb.GetCurrentAnimatorStateInfo(0).IsName("hide"))
        {
            yield break;
        }

        showing_front = false;
        current_coroutine = null;
        Debug.Log($"PCB {name} is now BACK");
    }
    public IEnumerator UnscrewThenShowFront()
    {
        Debug.Log($"UNSCREWING {name}");
        List<Screw> screws = transform.GetComponentsInChildren<Screw>().ToList();
        // screws.RemoveAll(s => s == null); // useful ?

        foreach (Screw screw in screws)
        {
            screw.Animator.Play("screw_unscrew");
            yield return new WaitForSeconds(0.25f);
        }

        yield return new WaitForSeconds(1f);

        yield return ShowBackCoroutine();
    }


}

/// <summary>
/// max inclusive each time
/// </summary>
[Serializable] public class PCB_GenerationParameter
{
    public Vector2Int components_min_max = new Vector2Int();
    public Vector2Int total_life_min_max = new Vector2Int();
    public Vector2Int shards_min_max = new Vector2Int();
    public Vector2Int virus_min_max = new Vector2Int();
}

public class Board
{
    private static bool logs = false;
    public Vector2Int grid_size;
    public List<GridCell> cells;
    

    // START
    public Board(int component_count)
    {
        // cells = new Dictionary<Vector2Int, string>();
        cells = new List<GridCell>();
        if (component_count <= 1) { grid_size = new Vector2Int(1,1); }
        else if (component_count <= 8) { grid_size = new Vector2Int(3, 3); }
        else if (component_count <= 13) { grid_size = new Vector2Int(5, 3); }
        else if (component_count <= 22) { grid_size = new Vector2Int(5, 5); }
        else if (component_count <= 31) { grid_size = new Vector2Int(7, 5); }
        else { grid_size = new Vector2Int(7,7); }

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
        for (int y=0; y<grid_size.y; y++)
        {
            for (int x=0; x < grid_size.x; x++)
            {
                Vector2Int pos = new Vector2Int(x,y) + center_offset;
                if (logs) { Debug.Log($"(CHGH) Creating cell ({x},{y}) offseted in {pos}"); }
                // cells.Add(cell,"");
                GridCell cell = new GridCell()
                {
                    x = pos.x,
                    y = pos.y,                
                };
                cells.Add(cell);
            }
        }
    }


    // MAIN METHODS
    public void OccupyCell(GridCell cell, string comp_name)
    {
        // if (!TryGetCell(pos, out GridCell cell)) { return; }
        if (cell.type != CellType.Empty) { Debug.LogError($"(Board) Tried occupying the cell in {cell.Position} with {comp_name} but the it is a {cell.type} cell with value {cell.value} already !"); return; }
        cell.value = comp_name;
        cell.type = CellType.Component;
        if (logs) { Debug.Log(get_grid_details()); }
    }
    public void PositionComponentInFreeCell(Component comp, int cell_size, int inter_cell)
    {
        GridCell cell = GetEmptyCell();
        if (cell == null) { return; }
        Vector2 pos = GetWorldPositionOfCell(cell, cell_size, inter_cell);

        comp.transform.localPosition = new Vector3 ( pos.x, pos.y, comp.transform.localPosition.z );
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
        if (logs) { Debug.Log($"(Board) Recursive trimming removed {count_before_touching_anything-cells.Count} cells in {iterations} iterations. We now have only {cells.Count}"); }
    }
    public int TrimOnce()
    {
        List<GridCell> to_delete = new List<GridCell>();
        int count_before_triming = cells.Count;
        foreach (GridCell cell in cells)
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
        if (logs) { Debug.Log($"(Board) Trimmed grid. Removed {count_before_triming-cells.Count} cells"); }
        return count_before_triming - cells.Count;
    }



    // GETTERS
    public Vector2 GetWorldPositionOfCell(GridCell cell, int cell_size, int inter_cell)
    {
        return cell.GetWorldPosition(cell_size, inter_cell);
    }
    public bool TryGetCell(Vector2Int pos, out GridCell cell)
    {
        cell = null;
        foreach (GridCell other_cell in cells)
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
        foreach (GridCell cell in cells)
        {
            if (cell.type != CellType.Empty) { continue; }
            free_cells.Add(cell.Position);
        }
        if (free_cells.Count == 0) { return new Vector2Int(-888, -888); }
        return free_cells[UnityEngine.Random.Range(0, free_cells.Count)];
    }
    public List<GridCell> GetEmptyCells()
    {
        List<GridCell> free_cells = new List<GridCell>(); // cells.Where(c => string.IsNullOrEmpty(c.Value));
        foreach (GridCell cell in cells)
        {
            if (cell.type != CellType.Empty) { continue; }
            free_cells.Add(cell);
        }
        return free_cells;
    }
    public GridCell GetEmptyCell()
    {
        List<GridCell> free_cells = GetEmptyCells();
        if (free_cells.Count == 0) { return null; }
        return free_cells[UnityEngine.Random.Range(0, free_cells.Count)];
    }
    private string get_grid_details()
    {
        string debug = $"(CHGH) GRID DETAILS : of size {grid_size}\n";
        foreach (GridCell cell in cells)
        {
            debug += $"\n - {cell.Position} - {cell.type.ToString()}";
            if (!string.IsNullOrEmpty(cell.value)) { debug += " - " + cell.value; }
        }
        return debug;
    }
    
    List<Vector2Int> neighbour_translations = new List<Vector2Int>()
    {
        Vector2Int.down,
        Vector2Int.left,
        Vector2Int.up,
        Vector2Int.right
    };
    public int GetNeighboursCount(GridCell cell) => GetNeighbours(cell.Position, out List<Vector2Int> translations);
    public int GetNeighbours(Vector2Int position, out List<Vector2Int> translations)
    {
        translations = new List<Vector2Int>();
        int count = 0;
        foreach (Vector2Int translation in neighbour_translations)
        {
            if (!TryGetCell(position + translation, out GridCell neighbour)) { continue; }
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
            if (!TryGetCell(position + translation, out GridCell neighbour)) { continue; }
            translations.Add(translation);
            count++;
        }
        return count;
    }


    // BUILD THE OUTLINE
    private List<Vector2> outlines_rails_pos = new List<Vector2>();
    internal void BuildPCB_Outline(ref List<SpriteRenderer> pcb_parts, InstantiaterParameter instantior)
    {
        outlines_rails_pos.Clear();

        int min_x = cells.Min(c => c.x);
        int max_x = cells.Max(c => c.x);
        int min_y = cells.Min(c => c.y);
        int max_y = cells.Max(c => c.y);

        Dictionary<Vector2Int,string> outline = new Dictionary<Vector2Int, string>();

        for (int i=min_x-1; i<max_x+2; i++)
        {
            for (int j = min_y - 1; j < max_y + 2; j++)
            {
                Vector2Int position = new Vector2Int(i,j);
                if (TryGetCell(position, out GridCell cell)) { continue; }
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
        Vector2 worldpos = GridCell.GetWorldPosition(position, PCB.cell_size, PCB.inter_cell);
        InstantiatePCB_Part(worldpos, corner, ref pcb_parts, instantior);
    }
    private void build_outline_edge(Vector2Int position, Vector2Int translation, ref List<SpriteRenderer> pcb_parts, InstantiaterParameter instantior)
    {
        Sprite edge = BoardController.Instance.PCB_PartsBank.GetEdgeMatchingWith(translation);
        Vector2 worldpos = GridCell.GetWorldPosition(position, PCB.cell_size, PCB.inter_cell);
        InstantiatePCB_Part(worldpos, edge, ref pcb_parts, instantior);

        // we place 2 rails if we can
        float rail_translation = (PCB.cell_size + PCB.inter_cell) / 2f * (1f / 24f);
        if (translation.x == 0f)
        {
            Vector2 left_rail = worldpos - new Vector2(rail_translation, 0f);
            if (!outlines_rails_pos.Contains(left_rail))
            {
                Sprite rail = BoardController.Instance.PCB_PartsBank.GetOutlineRailEdge(translation);
                InstantiatePCB_Part(left_rail, rail, ref pcb_parts, instantior, is_rail:true);
                outlines_rails_pos.Add(left_rail);
            }
            Vector2 right_rail = worldpos + new Vector2(rail_translation, 0f);
            if (!outlines_rails_pos.Contains(right_rail))
            {
                Sprite rail = BoardController.Instance.PCB_PartsBank.GetOutlineRailEdge(translation);
                InstantiatePCB_Part(right_rail, rail, ref pcb_parts, instantior, is_rail:true);
                outlines_rails_pos.Add(right_rail);
            }
        }
        else if (translation.y == 0f)
        {
            Vector2 down_rail = worldpos - new Vector2(0f, rail_translation);
            if (!outlines_rails_pos.Contains(down_rail))
            {
                Sprite rail = BoardController.Instance.PCB_PartsBank.GetOutlineRailEdge(translation);
                InstantiatePCB_Part(down_rail, rail, ref pcb_parts, instantior, is_rail:true);
                outlines_rails_pos.Add(down_rail);
            }
            Vector2 up_rail = worldpos + new Vector2(0f, rail_translation);
            if (!outlines_rails_pos.Contains(up_rail))
            {
                Sprite rail = BoardController.Instance.PCB_PartsBank.GetOutlineRailEdge(translation);
                InstantiatePCB_Part(up_rail, rail, ref pcb_parts, instantior, is_rail:true);
                outlines_rails_pos.Add(up_rail);
            }
        }
    }
    private void build_outline_big_corner(Vector2Int position, List<Vector2Int> translations, ref List<SpriteRenderer> pcb_parts, InstantiaterParameter instantior)
    {
        Sprite big_corner = BoardController.Instance.PCB_PartsBank.GetBigCornerMatchingWith(translations);
        Vector2 worldpos = GridCell.GetWorldPosition(position, PCB.cell_size, PCB.inter_cell);
        InstantiatePCB_Part(worldpos, big_corner, ref pcb_parts, instantior);


        // we place 2 rails if we can
        float rail_translation = (PCB.cell_size + PCB.inter_cell) / 2f * (1f / 24f);
        foreach (Vector2Int opposite_translation in translations)
        {
            Vector2Int translation = opposite_translation * new Vector2Int(-1,-1);
            Vector2 rail = worldpos + new Vector2( rail_translation*translation.x, rail_translation*translation.y );
            if (outlines_rails_pos.Contains(rail)) { continue; }
            Sprite sprite = BoardController.Instance.PCB_PartsBank.GetOutlineRailEdgeFromBigCorner(big_corner, translation);
            InstantiatePCB_Part(rail, sprite, ref pcb_parts, instantior, is_rail: true);
            outlines_rails_pos.Add(rail);
        }
    }

    private void InstantiatePCB_Part(Vector3 position, Sprite sprite, ref List<SpriteRenderer> pcb_parts, InstantiaterParameter instantior, bool is_rail=false)
    {
        SpriteRenderer sr = GameObject.Instantiate(instantior.prefab);
        sr.transform.SetParent(is_rail ? instantior.rails_parent : instantior.parts_parent);
        sr.sprite = sprite;
        sr.transform.localPosition = position;
        pcb_parts.Add(sr);
    }

}

public class InstantiaterParameter
{
    public SpriteRenderer prefab;
    public Transform parts_parent;
    public Transform rails_parent;
}





// USEFUL NESTED CLASSES
public class GridCell
{
    public int x;
    public int y;
    public Vector2Int Position => new Vector2Int(x, y);
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

    public List<Vector2> GetMiddleRailsWorldPositions(int cell_size, int inter_cell)
    {
        // 4 corners of the cell
        Vector2 world_cell = GetWorldPosition(cell_size, inter_cell);
        float translation = (cell_size + inter_cell) / 2f * (1f/24f) ;

        List<Vector2> corners = new List<Vector2>
        {
            new Vector2(world_cell.x + translation, world_cell.y + translation), // UP RIGHT
            new Vector2(world_cell.x + translation, world_cell.y - translation), // DOWN RIGHT
            new Vector2(world_cell.x - translation, world_cell.y - translation), // D L
            new Vector2(world_cell.x - translation, world_cell.y + translation), // U L
        };
        return corners;
    }
    public List<Vector2> GetVerticalRailsWorldPositions(int cell_size, int inter_cell)
    {
        // left & right vertical things
        Vector2 world_cell = GetWorldPosition(cell_size, inter_cell);
        float translation = (cell_size + inter_cell) / 2f * (1f/24f) ;
        List<Vector2> verts = new List<Vector2>
        {
            new Vector2(world_cell.x + translation, world_cell.y), // RIGHT
            new Vector2(world_cell.x - translation, world_cell.y), // LEFT
        };
        return verts;
    }
    public List<Vector2> GetHorizontalRailsWorldPositions(int cell_size, int inter_cell)
    {
        // u & d horizontal rails
        Vector2 world_cell = GetWorldPosition(cell_size, inter_cell);
        float translation = (cell_size + inter_cell) / 2f * (1f/24f) ;
        List<Vector2> hors = new List<Vector2>
        {
            new Vector2(world_cell.x, world_cell.y + translation), // UP
            new Vector2(world_cell.x, world_cell.y - translation), // D
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