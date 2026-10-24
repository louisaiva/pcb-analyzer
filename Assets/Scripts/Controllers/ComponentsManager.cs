using System.Collections.Generic;
using UnityEngine;

public class ComponentsManager : MonoBehaviour
{
    [SerializeField] private List<Component> components = new List<Component>();

    [Header("Hover")]
    [SerializeField] private Component hovered_component = null;
    [SerializeField] private Material base_material;
    [SerializeField] private Material hover_material;
    [SerializeField] private float faded_alpha = .9f;

    [Header("Click animations")]
    [SerializeField] private List<string> click_anim = new List<string>();

    [Header("Logs")]
    [SerializeField] private bool log_hovers = false;


    // COMPONENTs GRABBING
    private void Start()
    {
        GrabExistingComponents();
    }
    public void DestroyExistingComponents()
    {
        foreach (Component comp in GetComponentsInChildren<Component>(includeInactive: false))
        {
            Destroy(comp.gameObject);
        }
        components.Clear();
    }
    public void GrabExistingComponents()
    {
        components.Clear();
        foreach (Component comp in GetComponentsInChildren<Component>(includeInactive: false))
        {
            if (components.Contains(comp)) { continue; }
            components.Add(comp);
        }
    }

    // HOVERING
    public void HoverComponent(Component comp)
    {
        if (comp == null) { return; }
        if (comp == hovered_component) { return; }
        if (!components.Contains(comp)) { return; }
        foreach (Component other_comp in components)
        {
            if (comp == other_comp)
            {
                SetAlphaOfComponent(comp, 1f);
                comp.ScaleUp();
                // other_comp.Renderer.material = hover_material;
                continue;
            }
            SetAlphaOfComponent(other_comp, faded_alpha);
            comp.ResetScale();
            // other_comp.Renderer.material = base_material;
        }
        hovered_component = comp;
        if (log_hovers) { Debug.Log($"(CompManager) Hovered compoenent {comp.name}"); }
    }
    public void UnhoverComponent() => UnhoverComponent(hovered_component);
    public void UnhoverComponent(Component comp)
    {
        if (comp == null) { return; }
        if (comp != hovered_component) { return; }
        if (!components.Contains(comp)) { return; }
        foreach (Component other_comp in components)
        {
            SetAlphaOfComponent(other_comp, 1f);
            other_comp.ResetScale();
            // other_comp.Renderer.material = base_material;
        }
        hovered_component = null;
        if (log_hovers) { Debug.Log($"(CompManager) Unhover compoenent {comp.name}"); }
    }
    private static void SetAlphaOfComponent(Component comp, float alpha)
    {
        comp.Renderer.color = new Color
        (
            comp.Renderer.color.r,
            comp.Renderer.color.g,
            comp.Renderer.color.b,
            alpha
        );
    }
    public bool HasHoveredComponent(out Component comp)
    {
        comp = hovered_component;
        return comp != null;
    }

    // CLICKING
    public void ClickOnComponent(Component comp)
    {

        string anim = click_anim[UnityEngine.Random.Range(0, click_anim.Count)];
        while (comp.Animator.GetCurrentAnimatorStateInfo(0).IsName(anim))
        {
            anim = click_anim[UnityEngine.Random.Range(0, click_anim.Count)];
        }
        comp.Animator.Play(anim);
        comp.OnClick();
    }
}