using UnityEngine;
using System.Collections.Generic;

public class HoverableManager : MonoBehaviour
{
    [SerializeField] protected List<Hoverable> hoverables = new List<Hoverable>();

    [Header("Hover")]
    [SerializeField] protected Hoverable current_hover = null;
    [SerializeField] protected float faded_alpha = .9f;

    [Header("Click animations")]
    [SerializeField] protected List<string> click_anim = new List<string>();

    [Header("Logs")]
    [SerializeField] protected bool log_hovers = false;

    // HOVERING
    public virtual void Hover(Hoverable hoverable)
    {
        if (hoverable == null) { return; }
        if (hoverable == current_hover) { return; }
        if (!hoverables.Contains(hoverable)) { return; }
        foreach (Hoverable other_hoverable in hoverables)
        {
            if (hoverable == other_hoverable)
            {
                set_alpha_of_hoverable(hoverable, 1f);
                hoverable.ScaleUp();
                continue;
            }
            set_alpha_of_hoverable(other_hoverable, faded_alpha);
            other_hoverable.ResetScale();
        }
        current_hover = hoverable;
        if (log_hovers) { Debug.Log($"(CompManager) Hovered compoenent {hoverable.name}"); }
    }
    public virtual void Unhover() => Unhover(current_hover);
    public virtual void Unhover(Hoverable hoverable)
    {
        if (hoverable == null) { return; }
        if (hoverable != current_hover) { return; }
        if (!hoverables.Contains(hoverable)) { return; }
        foreach (Hoverable other in hoverables)
        {
            set_alpha_of_hoverable(other, 1f);
            other.ResetScale();
        }
        current_hover = null;
        if (log_hovers) { Debug.Log($"({GetType().Name}) Unhover {hoverable.name}"); }
    }
    public virtual bool HasHover<T>(out T hover) where T : Hoverable
    {
        hover = (T)current_hover;
        return hover != null;
    }
    private static void set_alpha_of_hoverable(Hoverable hover, float alpha)
    {
        if (hover == null) { return; }
        hover.Renderer.color = new Color
        (
            hover.Renderer.color.r,
            hover.Renderer.color.g,
            hover.Renderer.color.b,
            alpha
        );
    }

    public virtual void OnMousePress()
    {
    }
    public virtual void OnMouseRelease()
    {
        if (!HasHover(out Hoverable hoverable)) { return; }

        string anim = click_anim[UnityEngine.Random.Range(0, click_anim.Count)];
        while (hoverable.Animator.GetCurrentAnimatorStateInfo(0).IsName(anim))
        {
            anim = click_anim[UnityEngine.Random.Range(0, click_anim.Count)];
        }
        hoverable.Animator.Play(anim);
        hoverable.OnClick();
    }
}