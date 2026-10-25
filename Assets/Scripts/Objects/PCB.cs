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
    public Welder Welder;

    public static int cell_size = 20;
    public static int inter_cell = 7;


    [Header("Generation Parameters")]
    public PCB_GenerationParameter gen_para;
    public SpriteRenderer pcb_part_prefab;
    public SpriteRenderer rail_prefab;
    public SpriteRenderer rail_node_prefab;
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
        Board board = new Board(comp_to_generate, log_generation);

        for (int i=0; i<comp_to_generate; i++)
        {
            Component comp = BoardController.Instance.CompBank.CreateRandomComponent(Componer.transform);
            board.PositionComponentInFreeCell(comp, PCB.cell_size, PCB.inter_cell);
        }

        Componer.GrabExistingComponents();

        // generate the pcb board
        generate_pcb_board(board);

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
        foreach (BoardCell cell in board.cells)
        {
            // if (cell)
            SpriteRenderer sr = create_pcb_part(BoardController.Instance.PCB_PartsBank.GetCellSprite(cell.type), cell);
            pcb_parts.Add(sr);
            build_rails_around(cell, ref rails_placed, board);
        }

        // we build the outline
        InstantiaterParameter instantiator = new InstantiaterParameter
        {
            prefab = pcb_part_prefab,
            parts_parent = back_parts,
            rails_parent = back_rails
        };
        board.BuildPCB_Outline(ref pcb_parts, instantiator);


        // we wire the board
        Welder.WireBoard(board, Componer);
    }
    private void build_rails_around(BoardCell cell, ref List<Vector2> placed_rails, Board board)
    {
        // we build the rails of this
        List<Vector2> positions = cell.GetMiddleRailsWorldPositions(PCB.cell_size, PCB.inter_cell, out List<Vector2Int> rails_pos);
        for (int i=0; i< positions.Count; i++)
        {
            Vector2 pos = positions[i];
            if (placed_rails.Contains(pos)) { continue; }
            SpriteRenderer sr = create_pcb_line(rail_node_prefab, BoardController.Instance.PCB_PartsBank.GetLineMid(), pos);
            pcb_parts.Add(sr);
            placed_rails.Add(pos);
            board.AddRailCross(sr.GetComponent<RailCross>(), rails_pos[i]);
        }
        positions = cell.GetVerticalRailsWorldPositions(PCB.cell_size, PCB.inter_cell, out rails_pos);
        for (int i = 0; i < positions.Count; i++)
        {
            Vector2 pos = positions[i];
            if (placed_rails.Contains(pos)) { continue; }
            SpriteRenderer sr = create_pcb_line(rail_prefab, BoardController.Instance.PCB_PartsBank.GetLineVertical(), pos);
            pcb_parts.Add(sr);
            placed_rails.Add(pos);
            board.AddRail(sr.GetComponent<Rail>(), rails_pos[i], Axis.Vertical);
        }
        positions = cell.GetHorizontalRailsWorldPositions(PCB.cell_size, PCB.inter_cell, out rails_pos);
        for (int i = 0; i < positions.Count; i++)
        {
            Vector2 pos = positions[i];
            if (placed_rails.Contains(pos)) { continue; }
            SpriteRenderer sr = create_pcb_line(rail_prefab, BoardController.Instance.PCB_PartsBank.GetLineHorizontal(), pos);
            pcb_parts.Add(sr);
            placed_rails.Add(pos);
            board.AddRail(sr.GetComponent<Rail>(), rails_pos[i], Axis.Horizontal);
        }
    }
    private SpriteRenderer create_pcb_line(SpriteRenderer prefab, Sprite sprite, Vector3 position, bool is_back = true)
    {
        SpriteRenderer sr = Instantiate(prefab);
        sr.transform.SetParent(is_back ? back_rails : front_pcb.transform);
        sr.sprite = sprite;
        sr.transform.localPosition = position;
        return sr;
    }
    private SpriteRenderer create_pcb_part(Sprite sprite, BoardCell cell, bool is_back=true)
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
    [SerializeField] private Animator wires_pcb;
    [SerializeField] private bool showing_front = true;
    public bool IsShowingFront => showing_front;
    private Coroutine current_coroutine = null;
    
    
    [Header("Logs")]
    [SerializeField] private bool log_generation;


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
        wires_pcb.Play("hide");

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
        wires_pcb.Play("show");

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


public class InstantiaterParameter
{
    public SpriteRenderer prefab;
    public Transform parts_parent;
    public Transform rails_parent;
}