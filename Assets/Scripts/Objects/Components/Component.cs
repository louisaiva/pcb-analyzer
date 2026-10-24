using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.VisualScripting;
using UnityEngine;

public class Component : MonoBehaviour
{

    [Header("Data")]
    [SerializeField] private ComponentData data = null;


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
    ///  DATA MANAGEMENT
    //
    ///

    public void FeedData(ComponentData data)
    {
        this.data = data;
        this.name = data.name;
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

}