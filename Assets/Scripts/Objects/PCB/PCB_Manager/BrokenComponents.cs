using UnityEngine;

public class BrokenComponents : Problem
{
    protected override void update_problem(PCB pcb)
    {
        // we count how many broken components are on the pcb
        // that we need to fix
        ComponentsManager componer = pcb.Componer;
        int broken_comp = 0;
        foreach (Component comp in componer.GetComponents())
        {
            if (comp.IsDamaged) { broken_comp++; }
        }
        Quantity = broken_comp;
    }

    public override string GetDetails()
    {
        return Quantity + " broken components";
    }
}