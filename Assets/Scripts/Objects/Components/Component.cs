using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.VisualScripting;
using UnityEngine;

public class Component : Hoverable
{
    public static int Prefix = 0;

    [Header("Data")]
    [SerializeField] private ComponentData data = null;
    public Vector2Int CellPosition;
    public Vector2Int RailPosition => CellPosition*2;
    public Vector2 WorldPosition => animator.transform.position;

    [Header("Components References")]
    [SerializeField] protected Animator fx_animator;

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

    public Weld GetClosestFreeWeldFrom(Component b)
    {
        if (b == this) { return null; } // can't wire ourselves
        return GetClosestFreeWeldFrom(b.WorldPosition);
    }
    public Weld GetClosestFreeWeldFrom(Vector2 world_position)
    {
        List<Weld> free_welds = welds.Where(w => !w.used).ToList();
        if (free_welds.Count == 0) { return null; }
        Weld best_weld = null;
        float smallest_distance = float.MaxValue;
        for (int i=0; i<free_welds.Count;i++)
        {
            Vector2 direction = free_welds[i].world_position - world_position;
            if (direction.magnitude > smallest_distance) { continue; }
            smallest_distance = direction.magnitude;
            best_weld = free_welds[i];
        }
        return best_weld;
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


    ///
    //
    ///  HEALING
    //
    ///

    public void OnPress()
    {
        fx_animator.Play("fx_heal");
    }
    public void OnRelease()
    {
        fx_animator.Play("fx_idle");
    }

}