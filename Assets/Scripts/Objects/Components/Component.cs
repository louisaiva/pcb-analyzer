using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.VisualScripting;
using UnityEngine;

public class Component : Hoverable, Damageable
{
    public static int Prefix = 0;

    [Header("Data")]
    [SerializeField] private ComponentData data = null;
    public Vector2Int CellPosition;
    public Vector2Int RailPosition => CellPosition*2;
    public Vector2 WorldPosition => animator.transform.position;


    [Header("Components References")]
    [SerializeField] protected Animator fx_animator;
    [SerializeField] protected HP_Bar bar;
    

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

        // set full life to it
        MaxHP = data.max_HP;
        max_life = data.max_HP * 2; // multiply by 2 because we have 2 steps
        life = max_life;
        bar.Target(this);
        bar.gameObject.SetActive(false);

        // this.animator.Play(data.name + "_full", 1);
        play_life_anim();

        // calculate the local offset based on the size vector
        float half_px = .5f / 24f;
        float px = 1f / 24f;
        bg_sr.transform.localScale = new Vector3
        (
            data.bg_size.x,
            data.bg_size.y,
            1f
        );
        bg_sr.transform.localPosition = new Vector3
        (
            data.bg_size.x % 2 == 0 ? 0f : half_px,
            data.bg_size.y % 2 == 0 ? 0f : half_px,
            0f
        );
        bar.transform.localPosition = new Vector3
        (
            data.bg_size.x % 2 == 0 ? 0f : half_px,
            data.bg_size.y / 2f / 24f + 4*px,
            0f
        );
        sr.transform.localPosition = new Vector3
        (
            data.size.x % 2 == 0 ? 0f : half_px,
            data.size.y % 2 == 0 ? 0f : half_px,
            0f
        );
        sr.transform.localPosition += new Vector3
        (
            data.offset.x * px,
            data.offset.y * px,
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
        // Debug.Log($"(Component) {name} has a new Weld with pixel offset {pixel} && world_pos {position}. Animator is located at {animator.transform.position}");
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
    ///  HOVER
    //
    ///

    public override void OnHover()
    {
        bar.gameObject.SetActive(true);
    }
    public override void OnUnhover()
    {
        bar.gameObject.SetActive(false);
    }




    ///
    //
    ///  HEALING / DAMAGING
    //
    ///
    [Header("Life")]
    [SerializeField] protected int life;
    [SerializeField] protected int max_life;
    public int MaxLife { get => max_life; set => max_life = value; }
    public int MaxHP { get; private set; }
    public int Life { get => life; set => life = value; }
    public float LifePercentage => Life / (float)MaxLife;
    public bool IsDamaged => life != max_life;
    public bool IsDead => life <= 0;

    // INPUT ENTRY
    public void OnPress()
    {
        fx_animator.Play("fx_heal");
        animator.Play("slow_fix", 0);
    }
    public void OnRelease()
    {
        fx_animator.Play("fx_idle");
        ScaleUp();
    }

    // HEALING
    public void Heal()
    {
        life = max_life;
        play_life_anim();
        bar.RefreshLife();
        Debug.Log($"(Component) {name} of type {data.type_of_component} was healed ! Has now {life} / {max_life} HP.       ({LifePercentage})");
        ScaleUp();
    }
    

    // DAMAGE & DIE
    public virtual void Damage()
    {
        if (IsDead) { return; }

        fx_animator.Play("fx_hurt");

        life -= 1;
        if (life == 0) { Die(); }
        Debug.Log($"(Component) {name} of type {data.type_of_component} took damage. Has now {life} / {max_life} HP.       ({LifePercentage})");

        play_life_anim();
        bar.RefreshLife();
    }
    private void play_life_anim()
    {
        if (!IsDamaged) { animator.Play(data.name + "_full", 1); }
        else if (LifePercentage > 0f) { animator.Play(data.name + "_mid", 1); }
        else { animator.Play(data.name + "_dead", 1); }
    }
    public void Die()
    {
        Debug.Log($"(Component) {name} of type {data.type_of_component} just died ://///    RIP");
    }
}