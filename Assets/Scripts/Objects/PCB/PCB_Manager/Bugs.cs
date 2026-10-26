using UnityEngine;

public class Bugs : Problem
{
    protected override void update_problem(PCB pcb)
    {
        Quantity = pcb.Bugger.Count;
    }

    public override string GetDetails()
    {
        return Quantity + " bugs";
    }
}