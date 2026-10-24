using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class Component : MonoBehaviour
{



    [Header("Components")]
    [SerializeField] private SpriteRenderer bg_sr;
    [SerializeField] private SpriteRenderer sr;
    public SpriteRenderer Renderer => sr;
    [SerializeField] private Animator animator;
    public Animator Animator => animator;



    ///
    //
    ///  ENTRY POINTS
    //
    ///


    // HOVER ANIMATION
    public void ScaleUp()
    {
        // slowly rise the scale
        animator.Play("scale_up");
    }
    public void ResetScale()
    {
        if (sr.transform.localScale.x <= 1f) { return; }
        // if scale > 1 we were hovered, we play a little animation depopup
        animator.Play("depopup");
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
        this.name = data.name;
        this.animator.Play(data.name + "_idle");

        // calculate the local offset based on the size vector
        // apply this offset to both bg + sr
        // setup the size into the bg
    }

}