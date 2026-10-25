using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class ComponentsManager : HoverableManager
{
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
        hoverables.Clear();
    }
    public void GrabExistingComponents()
    {
        hoverables.Clear();
        foreach (Component comp in GetComponentsInChildren<Component>(includeInactive: false))
        {
            if (hoverables.Contains(comp)) { continue; }
            hoverables.Add(comp);
        }
    }

    // GETTERS
    public Component GetRandomComponent()
    {
        List<Component> components = GetComponents();
        if (components.Count == 0) { return null; }
        return components[UnityEngine.Random.Range(0, components.Count)];
    }
    public List<Component> GetComponents() { return hoverables.ConvertAll(h => (Component)h); }





    // CLICKING
    protected Coroutine holding_coroutine = null;
    [SerializeField] protected float hold_delay = 0.5f;
    [SerializeField] protected Component holding_comp = null;
    public virtual bool HasHolding<T>(out T holding) where T : Component
    {
        holding = (T)holding_comp;
        return holding != null;
    }

    // ENTRY POINTS    
    public override void OnMousePress()
    {
        if (holding_coroutine != null) { return; }
        if (!HasHover(out Component comp)) { return; }
        holding_coroutine = StartCoroutine(HoldCoroutine());
    }
    public override void OnMouseRelease()
    {
        if (holding_coroutine != null)
        {
            StopCoroutine(holding_coroutine);
            holding_coroutine = null;
        }

        if (HasHolding(out Component comp))
        {
            comp.OnRelease();
            holding_comp = null;
            return;
        }

        if (HasHover(out comp))
        {
            DamageComponent(comp);
            return;
        }

    }
    
    // MAIN METHODS
    public void DamageComponent(Component comp)
    {
        string anim = click_anim[UnityEngine.Random.Range(0, click_anim.Count)];
        while (comp.Animator.GetCurrentAnimatorStateInfo(0).IsName(anim))
        {
            anim = click_anim[UnityEngine.Random.Range(0, click_anim.Count)];
        }
        comp.Animator.Play(anim);
        comp.OnClick();
    }
    public IEnumerator HoldCoroutine()
    {
        Component comp = (Component)current_hover;

        yield return new WaitForSeconds(hold_delay);

        holding_comp = comp;
        holding_comp.OnPress();
        holding_coroutine = null;
    }
}