using System.Collections.Generic;
using UnityEngine;

public class BugManager : HoverableManager
{
    public bool activated = false;
    public PCB pcb = null;

    private void Start() { pcb = GetComponentInParent<PCB>(includeInactive: true); }

    // ACTIVATE / DEACTIVATE
    public void ActivateBugs()
    {
        activated = true;

        foreach (Bug bug in hoverables)
        {
            bug.bb.ReEnterCurrentState();
        }
    }
    public void DeactivateBugs()
    {
        activated = false;
    }
    private void Update()
    {
        if (!activated) { return; }
        if (pcb == null) { return; }
        if (BoardController.Instance.PCB_Manager.pcb != pcb) { return; }

        foreach (Bug bug in hoverables)
        {
            if (bug.IsDead) { continue; }
            if (!bug.bb.enabled) { continue; }
            bug.bb.UpdateBehaviour();
        }
    }


    // MAIN METHODS
    public List<Bug> GetBugs()
    {
        return hoverables.ConvertAll(h => (Bug)h);
    }
    public override void OnMouseRelease()
    {
        if (HasHover(out Bug bug))
        {
            DamageBug(bug);
            return;
        }
    }
    public void AddBug(Bug bug)
    {
        hoverables.Add(bug);
    }
    public void DamageBug(Bug bug)
    {
        bug.Damage();

        if (bug.IsDead) { hoverables.Remove(bug); }
    }
}