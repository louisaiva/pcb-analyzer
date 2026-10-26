using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;

public class PCB : MonoBehaviour
{

    public bool Unlocked = false;
    public bool Fixed = false;

    [Header("References")]
    public ComponentsManager Componer;
    public RocksManager Rocker;
    public Welder Welder;
    public PCB_Timer Timer;
    public Animator Animator;

    public static int cell_size = 20;
    public static int inter_cell = 7;


    [Header("Generation Parameters")]
    public BoardPlan plan;
    public int Index => plan.index;
    public PCB_GenerationParameter gen_para;
    public SpriteRenderer pcb_part_prefab;
    public SpriteRenderer rail_prefab;
    public SpriteRenderer rail_node_prefab;
    public SpriteRenderer sticker_prefab;
    public SpriteRenderer desc_back_prefab;
    public SpriteRenderer desc_pcb_index;
    public Rock rock_prefab;
    
    // public SpriteRenderer desc_front_prefab;
    [SerializeField] private Transform back_parts;
    [SerializeField] private Transform back_rails;
    [SerializeField] private Transform front_parts;
    [SerializeField] private Transform front_rails;
    [SerializeField] private List<SpriteRenderer> pcb_parts = new List<SpriteRenderer>();


    // PCB GENERATION main methods
    public async void Generate(BoardPlan plan)
    {
        this.plan = plan;
        this.name = "PCB n°"+plan.index;
        Fixed = false;


        Componer.DestroyExistingComponents();
        while (pcb_parts.Count > 0)
        {
            Destroy(pcb_parts[0].gameObject);
            pcb_parts.RemoveAt(0);
        }

        // wait to ensure the components are really destroyed & everything is started correctly
        await Task.Yield();
        await Task.Yield();
        await Task.Yield();
        await Task.Yield();
        await Task.Yield();
        await Task.Yield();

        // we want to make sure pcb is flipped BACK before generating components
        if (showing_front) { FlipInstantly(); }

        // choose a random number from the gen parameters
        // int comp_to_generate = UnityEngine.Random.Range(gen_para.components_min_max.x, gen_para.components_min_max.y+1);
        Board board = new Board(plan.components.Count, log_generation);
        for (int i=0; i<plan.components.Count; i++)
        {
            string comp_name = plan.components[i].component;
            Component comp = BoardController.Instance.CompBank.CreateComponentFromTemplate(comp_name,Componer.transform);
            if (plan.components[i].life != -1) { comp.SetLife(plan.components[i].life); }
            board.PositionComponentInFreeCell(comp, PCB.cell_size, PCB.inter_cell);
        }

        Componer.GrabExistingComponents();


        // generate the pcb board
        build_back_board(board, plan);

        await Task.Yield();
        await Task.Yield();

        // we want to make sure pcb is flipped FRONT before generating the front lol
        if (!showing_front) { FlipInstantly(); }
        build_front_board(board);
    }
    private void build_back_board(Board board, BoardPlan plan)
    {
        // we trim the grid so we have the smallest pcb
        board.TrimRecursively(gen_para.trim_iterations);

        // we place description & title in an empty cell
        board.AddBackDescriptionCells();

        // get 2 random empty cells
        BoardCell stick1 = board.GetEmptyCell();
        if (stick1 != null) { stick1.type = CellType.Sticker; }
        BoardCell stick2 = board.GetEmptyCell();
        if (stick2 != null) { stick2.type = CellType.Sticker; }


        // we place screws ?

        // we build the outline
        InstantiaterParameter instantiator = new InstantiaterParameter
        {
            skin = "back",
            prefab = pcb_part_prefab,
            parts_parent = back_parts,
            rails_parent = back_rails
        };
        build_board(board, instantiator, is_back: true);

        // generate rocks
        for (int i = 0; i < plan.rocks.Count; i++)
        {
            RailCross cross = board.GetRandomCross();
            Rock rock = create_rock(plan.rocks[i], cross);
        }


        // we wire the board
        Welder.WireBoard(board, Componer);
    }
    private void build_front_board(Board board)
    {
        board.Flip();
        board.FreeAllCells();

        // we place description & title in an empty cell
        board.AddFrontDescriptionCells();

        // get a random empty cell to show the index
        BoardCell cell = board.GetEmptyCell();
        if (cell != null) { cell.SetTypeAndValue(CellType.Desc,"index"); }

        // get 2 random empty cells
        cell = board.GetEmptyCell();
        if (cell != null) { cell.type = CellType.Sticker; }
        cell = board.GetEmptyCell();
        if (cell != null) { cell.type = CellType.Sticker; }

        // we build the outline
        InstantiaterParameter instantiator = new InstantiaterParameter
        {
            skin = "front",
            prefab = pcb_part_prefab,
            parts_parent = front_parts,
            rails_parent = front_rails
        };
        build_board(board, instantiator, is_back: false);
    }


    private Rock create_rock(int life, RailCross cross)
    {
        Rock rock = Instantiate(rock_prefab);
        rock.transform.SetParent(Rocker.transform);
        rock.transform.localPosition = cross.WorldPosition - (Vector2)transform.position;
        rock.transform.localScale = Vector3.one;
        Rocker.AddRock(rock);
        rock.InitRock(life,cross);
        return rock;
    }

    // PCB generation mid level methods
    private void build_board(Board board, InstantiaterParameter instantiator, bool is_back=true)
    {
        // we build the grid + rails
        List<Vector2> rails_placed = new List<Vector2>();
        foreach (BoardCell cell in board.cells)
        {
            // if (cell)
            SpriteRenderer sr = create_pcb_part(BoardController.Instance.PCBank.GetCellSprite(cell.type, is_back ? "back" : "front"), cell, is_back);
            pcb_parts.Add(sr);

            // check if cell.type == sticker
            if (cell.type == CellType.Sticker) { create_sticker(sr.transform); }
            if (cell.type == CellType.Desc && cell.value == "back up") { create_description(sr.transform, desc_back_prefab); }
            if (cell.type == CellType.Desc && cell.value == "front left") { create_description(sr.transform, plan.front_desc); }
            if (cell.type == CellType.Desc && cell.value == "index")
            {
                SpriteRenderer desc_index = create_description(sr.transform, desc_pcb_index);
                desc_index.GetComponentInChildren<TMPro.TextMeshPro>(includeInactive:true).text = "PCB " + plan.index;
            }

            build_rails_around(cell, ref rails_placed, board, is_back);
        }

        // place rocks here ?

        board.BuildPCB_Outline(ref pcb_parts, instantiator);
    }
    private void build_rails_around(BoardCell cell, ref List<Vector2> placed_rails, Board board, bool is_back=true)
    {
        string skin = is_back ? "back" : "front";

        // we build the rails of this
        List<Vector2> positions = cell.GetMiddleRailsWorldPositions(PCB.cell_size, PCB.inter_cell, out List<Vector2Int> rails_pos);
        for (int i=0; i< positions.Count; i++)
        {
            Vector2 pos = positions[i];
            if (placed_rails.Contains(pos)) { continue; }
            SpriteRenderer sr = create_pcb_line(rail_node_prefab, BoardController.Instance.PCBank.GetLineMid(skin), pos, is_back);
            pcb_parts.Add(sr);
            placed_rails.Add(pos);
            board.AddRailCross(sr.GetComponent<RailCross>(), rails_pos[i]);
        }
        positions = cell.GetVerticalRailsWorldPositions(PCB.cell_size, PCB.inter_cell, out rails_pos);
        for (int i = 0; i < positions.Count; i++)
        {
            Vector2 pos = positions[i];
            if (placed_rails.Contains(pos)) { continue; }
            SpriteRenderer sr = create_pcb_line(rail_prefab, BoardController.Instance.PCBank.GetLineVertical(skin), pos, is_back);
            pcb_parts.Add(sr);
            placed_rails.Add(pos);
            board.AddRail(sr.GetComponent<Rail>(), rails_pos[i], Axis.Vertical);
        }
        positions = cell.GetHorizontalRailsWorldPositions(PCB.cell_size, PCB.inter_cell, out rails_pos);
        for (int i = 0; i < positions.Count; i++)
        {
            Vector2 pos = positions[i];
            if (placed_rails.Contains(pos)) { continue; }
            SpriteRenderer sr = create_pcb_line(rail_prefab, BoardController.Instance.PCBank.GetLineHorizontal(skin), pos, is_back);
            pcb_parts.Add(sr);
            placed_rails.Add(pos);
            Rail rail = sr.GetComponent<Rail>();
            board.AddRail(rail, rails_pos[i], Axis.Horizontal);

            // check if the rail is going over the description so we disable it
            if (cell.type != CellType.Desc) { continue; }
            if (   (i == 0 /* U */ && cell.value == "back down")
                || (i == 1 /* D */ && cell.value == "back up"))
            {
                rail.disabled = true;
            }
        }
    }
    private SpriteRenderer create_pcb_line(SpriteRenderer prefab, Sprite sprite, Vector3 position, bool is_back = true)
    {
        SpriteRenderer sr = Instantiate(prefab);
        sr.transform.SetParent(is_back ? back_rails : front_rails);
        sr.sprite = sprite;
        sr.transform.localPosition = position;
        sr.transform.localScale = Vector3.one;
        return sr;
    }
    private SpriteRenderer create_pcb_part(Sprite sprite, BoardCell cell, bool is_back = true)
    {
        SpriteRenderer sr = Instantiate(pcb_part_prefab);
        sr.transform.SetParent(is_back ? back_parts : front_parts);
        sr.sprite = sprite;
        sr.transform.localPosition = cell.GetWorldPosition(PCB.cell_size, PCB.inter_cell);
        sr.transform.localScale = Vector3.one;
        return sr;
    }
    private void create_sticker(Transform parent)
    {
        SpriteRenderer sr = Instantiate(sticker_prefab);
        sr.transform.SetParent(parent);
        sr.sprite = BoardController.Instance.StickerBank.GetRandomSprite();
        sr.transform.localPosition = Vector3.zero;
        sr.transform.localScale = Vector3.one;
    }
    private SpriteRenderer create_description(Transform parent, SpriteRenderer prefab)
    {
        SpriteRenderer sr = Instantiate(prefab/*  ? desc_back_prefab : plan.front_desc */);
        sr.transform.SetParent(parent);
        sr.transform.localPosition = Vector3.zero;
        sr.transform.localScale = Vector3.one;
        return sr;
    }


    [Header("Animations")]
    [SerializeField] private Animator front_pcb;
    [SerializeField] private List<Animator> back_pcbs;
    [SerializeField] private bool showing_front = true;
    public bool IsShowingFront => showing_front;
    private Coroutine current_coroutine = null;
    
    
    [Header("Logs")]
    [SerializeField] private bool log_generation;


    // PCB FLIPPING
    public bool IsScrewed => GetComponentInChildren<Screw>(includeInactive: false) != null;
    public void FlipInstantly()
    {
        if (showing_front)
        {
            front_pcb.Play("hidden");
            foreach (Animator animator in back_pcbs)
            {
                animator.Play("shown");
            }
            showing_front = false;
            Debug.Log($"{name} is now BACK (instant)");
            return;
        }

        front_pcb.Play("shown");
        foreach (Animator animator in back_pcbs)
        {
            animator.Play("hidden");
        }
        showing_front = true;
        Debug.Log($"{name} is now FRONT (instant)");
    }
    public void Flip()
    {
        if (current_coroutine != null) { return; }

        // ensure we don't have any screws
        if (IsScrewed)
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
        foreach (Animator animator in back_pcbs)
        {
            animator.Play("hide");
        }

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
        foreach (Animator animator in back_pcbs)
        {
            animator.Play("show");
        }

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

        Timer.StartTimer();

        foreach (Screw screw in screws)
        {
            screw.Animator.Play("screw_unscrew");
            yield return new WaitForSeconds(0.25f);
        }

        yield return new WaitForSeconds(.15f);

        yield return ShowBackCoroutine();
    }


}

/// <summary>
/// max inclusive each time
/// </summary>
[Serializable] public class PCB_GenerationParameter
{
    public Vector2Int components_min_max = new Vector2Int();
    [Range(0,3)] public int trim_iterations = 1;
    public Vector2Int total_life_min_max = new Vector2Int();
    public Vector2Int shards_min_max = new Vector2Int();
    public Vector2Int virus_min_max = new Vector2Int();
}


public class InstantiaterParameter
{
    public string skin;
    public SpriteRenderer prefab;
    public Transform parts_parent;
    public Transform rails_parent;
}