using UnityEngine;

public class IsScrewed : Problem
{
    protected override void update_problem(PCB pcb)
    {
        Quantity = pcb.IsScrewed ? 1 : 0;
    }

    public override string GetDetails()
    {
        if (Quantity == 1)
        {
            return " pcb is screwed";
        }
        return "";
    }
}