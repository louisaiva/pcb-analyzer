using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.VisualScripting;
using UnityEngine;

public class Component : MonoBehaviour
{
    public static int Prefix = 0;

    [Header("Data")]
    [SerializeField] private ComponentData data = null;
    public Vector2Int CellPosition;
    public Vector2Int RailPosition => CellPosition*2;


    [Header("Components")]
    [SerializeField] private SpriteRenderer bg_sr;
    [SerializeField] private SpriteRenderer sr;
    public SpriteRenderer Renderer => sr;
    [SerializeField] private Animator animator;
    public Animator Animator => animator;
    // [SerializeField] private Collider2D hover_collider;
    [SerializeField] private bool scaled_up = false;
    [SerializeField] private bool logs = false;



    ///
    //
    ///  ENTRY POINTS
    //
    ///


    // HOVER ANIMATION
    public void ScaleUp()
    {
        if (logs) { Debug.Log($"(Component) Scaling up {name}"); }
        animator.Play("scale_up");
        scaled_up = true;
    }
    public void ResetScale()
    {
        if (logs) { Debug.Log($"(Component) Resetting scale of {name}"); }
        // if (sr.transform.localScale.x <= 1f) { return; }
        if (scaled_up)
        {
            if (logs) { Debug.Log($"(Component) Depopping up {name}"); }
            animator.Play("depopup");
        }
        // else { animator.Play("") }
        // if scale > 1 we were hovered, we play a little animation depopup
        scaled_up = false;
    }

    // ON CLICK
    public void OnClick()
    {
    }




    ///
    //
    ///  WELD HANDLING
    //
    ///
    private List<Weld> welds = new List<Weld>();
    public Weld GetRandomFreeWeld()
    {
        List<Weld> free_welds = welds.Where(w => !w.used).ToList();
        if (free_welds.Count == 0) { return null; }
        return free_welds[UnityEngine.Random.Range(0,free_welds.Count)];
    }




    ///
    //
    ///  DATA MANAGEMENT
    //
    ///

    public void FeedData(ComponentData data)
    {
        this.data = data;
        this.name = Prefix.ToString() + "_" + data.name;
        Prefix++;
        this.animator.Play(data.name + "_idle", 1);

        // calculate the local offset based on the size vector
        float half_offset = .5f / 24f;
        float full_offset = 1f / 24f;
        bg_sr.transform.localScale = new Vector3
        (
            data.bg_size.x,
            data.bg_size.y,
            1f
        );
        bg_sr.transform.localPosition = new Vector3
        (
            data.bg_size.x % 2 == 0 ? 0f : half_offset,
            data.bg_size.y % 2 == 0 ? 0f : half_offset,
            0f
        );
        sr.transform.localPosition = new Vector3
        (
            data.size.x % 2 == 0 ? 0f : half_offset,
            data.size.y % 2 == 0 ? 0f : half_offset,
            0f
        );
        sr.transform.localPosition += new Vector3
        (
            data.offset.x * full_offset,
            data.offset.y * full_offset,
            0f
        );

        // Debug.Log($"(Component) Feeding data into {name} :\n {data.GetDetails()}");
        // Debug.Log($"(Component) local positions are now : {bg_sr.transform.localPosition}     /    {sr.transform.localPosition}");

    }

    public void RefreshWelds()
    {
        // creates welds
        welds.Clear();
        foreach (WeldData wdata in data.welds)
        {
            Weld weld = new Weld
            {
                component = this,
                world_position = calculate_weld_position(wdata.pixel),
                exit_direction = wdata.exit,
            };
            welds.Add(weld);
        }
    }

    private Vector2 calculate_weld_position(Vector2 pixel)
    {
        Vector2 position = new Vector2(animator.transform.position.x, animator.transform.position.y);
        position.x += pixel.x * (1f / 24f);
        position.y += pixel.y * (1f / 24f);
        Debug.Log($"(Component) {name} has a new Weld with pixel offset {pixel} && world_pos {position}. Animator is located at {animator.transform.position}");
        return position;
    }

    private void OnDrawGizmos()
    {
        if (welds.Count == 0) { return; }
        
        Gizmos.color = Color.red;
        float radius = 0.03f;
        foreach (Weld weld in welds)
        {
            Gizmos.DrawSphere(weld.world_position, radius);
        }
    }

}