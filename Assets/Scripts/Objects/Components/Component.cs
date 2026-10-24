using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class Component : MonoBehaviour
{
    private SpriteRenderer sr;
    public SpriteRenderer Renderer => sr;
    private Animator animator;
    public Animator Animator => animator;

    private void Start()
    {
        sr = GetComponentInChildren<SpriteRenderer>(includeInactive:true);
        animator = GetComponentInChildren<Animator>(includeInactive:true);
    }

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
}