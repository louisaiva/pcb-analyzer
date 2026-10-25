using UnityEngine;

public class DeadWires : Problem
{
    protected override void update_problem(PCB pcb)
    {
        // we count how many broken components are on the pcb
        // that we need to fix
        Welder welder = pcb.Welder;
        int dead_wires = 0;
        foreach (Wire wire in welder.Wires)
        {
            if (!wire.Connected) { dead_wires++; }
        }
        Quantity = dead_wires;
    }

    public override string GetDetails()
    {
        return Quantity + " dead wires";
    }
}