using UnityEngine;

public class Scraper : Problem
{
    protected override void update_problem(PCB pcb)
    {
        RocksManager rocker = pcb.Rocker;
        Quantity = rocker.GetRocks().Count;
    }

    public override string GetDetails()
    {
        return Quantity + " scrap" + (Quantity > 1 ? "s" : "");
    }
}