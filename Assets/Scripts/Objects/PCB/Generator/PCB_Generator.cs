using System;
using System.Collections.Generic;
using UnityEngine;

public class PCB_Generator : MonoBehaviour
{
    public PCB pcb_prefab;
    public List<BoardPlan> plans = new List<BoardPlan>();
    public List<PCB> pcbs = new List<PCB>();
    public int Count => pcbs.Count;

    public PCB GenerateNext()
    {
        BoardPlan plan = null;
        if (plans.Count > 0) { plan = plans[0]; plans.RemoveAt(0); }
        else { Debug.LogError("NO MORE PLAN !!!"); return null; }
        plan.index = pcbs.Count + 1;
        Debug.Log($"Generating board for plan {plan} : PCB " + plan.index);

        return Generate(plan);
    }
    public PCB Generate(BoardPlan plan)
    {
        PCB pcb = Instantiate(pcb_prefab);
        // pcb.transform.localPosition = new Vector3(20f,0f,0f);
        pcb.Generate(plan);
        pcbs.Add(pcb);
        return pcb;
    }


    public PCB GetClosestUnlockedIndex(int index)
    {
        PCB best = null;
        int best_diff = int.MaxValue;
        foreach (PCB pcb in pcbs)
        {
            if (!pcb.Unlocked) { continue; }
            int diff = Mathf.Abs(index - pcb.Index);
            if (diff > best_diff) { continue; }
            best = pcb;
            best_diff = diff;
        }
        if (best == null) { Debug.LogError($"(PCB_Generator) Could not find closest unlocked index pcb of index {index}. PCBs are : \n{GetDetails()}"); }
        return best;
    }
    public string GetDetails()
    {
        string debug = Count + "PCBs";
        foreach (PCB pcb in pcbs)
        {
            debug += $"\n - {pcb.name} : Unlocked ? {pcb.Unlocked} / Fixed ? {pcb.Fixed} / Side ? {(pcb.IsShowingFront ? "FRONT" : "BACK")} / Screwed ? {pcb.IsScrewed} / Components nb : {pcb.Componer.GetComponents().Count}";
        }
        return debug;
    }

}

[Serializable] public class BoardPlan
{
    [NonSerialized] public int index;
    public List<string> components = new List<string>();
    public int wires_per_components = 2;
    public List<int> rocks = new List<int>(); // life of said rocks
    public List<int> bugs = new List<int>(); // life of said bugs
    public SpriteRenderer front_desc;
}