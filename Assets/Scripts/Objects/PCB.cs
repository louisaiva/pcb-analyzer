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


    [Header("Generation Parameters")]
    public PCB_GenerationParameter gen_para;


    // PCB GENERATION
    public async void Generate()
    {
        Componer.DestroyExistingComponents();

        // wait to ensure the components are really destroyed
        await Task.Yield();
        await Task.Yield();

        // choose a random number from the gen parameters
        int comp_to_generate = UnityEngine.Random.Range(gen_para.components_min_max.x, gen_para.components_min_max.y+1);
        GridGenerationHelper ggh = new GridGenerationHelper(comp_to_generate);

        for (int i=0; i<comp_to_generate; i++)
        {
            Component comp = BoardController.Instance.CompBank.CreateRandomComponent(Componer.transform);
            ggh.PositionInFreeCell(comp, gen_para.cell_size, gen_para.inter_size);
        }

        // generate the pcb board
        generate_pcb_board(ggh);

        Componer.GrabExistingComponents();
    }

    private async void generate_pcb_board(GridGenerationHelper ggh)
    {
        // we trim the grid so we have the smallest pcb
        // ggh.Trim();

        // we place wires & rocks

        // we place description & title in an empty cell

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
    public int cell_size = 18;
    public int inter_size = 7;
    public Vector2Int total_life_min_max = new Vector2Int();
    public Vector2Int shards_min_max = new Vector2Int();
    public Vector2Int virus_min_max = new Vector2Int();
}

public class GridGenerationHelper
{
    private static bool logs = false;
    public Vector2Int grid_size;
    public Dictionary<Vector2Int,string> cells;
    // public List<GridCell> cells ;
    public GridGenerationHelper(int component_count)
    {
        cells = new Dictionary<Vector2Int, string>();
        // cells = new List<GridCell>();
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
                Vector2Int cell = new Vector2Int(x,y) + center_offset;
                if (logs) { Debug.Log($"(CHGH) Creating cell ({x},{y}) offseted in {cell}"); }
                cells.Add(cell,"");
            }
        }
    }
    private string get_grid_details()
    {
        string debug = $"(CHGH) GRID DETAILS : of size {grid_size}\n";
        foreach (KeyValuePair<Vector2Int, string> kvp in cells)
        {
            debug += $"\n - {kvp.Key} - {kvp.Value}";
        }
        return debug;
    }

    public Vector2Int GetFreeCell()
    {
        List<Vector2Int> free_cells = new List<Vector2Int>(); // cells.Where(c => string.IsNullOrEmpty(c.Value));
        foreach (KeyValuePair<Vector2Int,string> kvp in cells)
        {
            if (!string.IsNullOrEmpty(kvp.Value)) { continue; }
            free_cells.Add(kvp.Key);
        }
        if (free_cells.Count == 0) { return new Vector2Int(-888,-888); }
        return free_cells[UnityEngine.Random.Range(0, free_cells.Count)];
    }
    public void OccupyCell(Vector2Int cell, string comp_name)
    {
        if (!cells.ContainsKey(cell)) { return; }
        if (!string.IsNullOrEmpty(cells[cell])) { Debug.LogError($"(PCB - ComponentHolderGenerationHelper) Tried occupying the cell in {cell} with {comp_name} but the component {cells[cell]} is already occupying this cell !"); return; }
        cells[cell] = comp_name;
        if (logs) { Debug.Log(get_grid_details()); }
    }
    public void PositionInFreeCell(Component comp, int cell_size, int inter_cell)
    {
        Vector2Int cell = GetFreeCell();

        float x = cell.x * cell_size * (1f / 24f) + Mathf.Sign(cell.x) * cell.x * inter_cell;
        float y = cell.x * cell_size * (1f / 24f) + Mathf.Sign(cell.y) * cell.y * inter_cell;

        comp.transform.localPosition = new Vector3 ( x, y, comp.transform.localPosition.z );
        if (logs) { Debug.Log($"(CHGH) Positionning {comp.name} in cell {cell} at world position {x},{y}"); }
        OccupyCell(cell, comp.name);
    }

    public class GridCell
    {
        public int x;
        public int y;
        public CellType type = CellType.Empty;
        public string value = "";
    }
    public enum CellType
    {
        Empty,
        Component,
        Desc,
        Title
    }
}