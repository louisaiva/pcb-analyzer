using System;
using System.Collections.Generic;
using UnityEngine;

public class PCB_Generator : MonoBehaviour
{
    public PCB pcb_prefab;
    public List<BoardPlan> plans = new List<BoardPlan>();
    public List<PCB> pcbs = new List<PCB>();


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