using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PCB_Manager : MonoBehaviour
{
    public PCB pcb;
    public List<Problem> problems;
    public List<Info> infos;
    private PCB_Generator generator => BoardController.Instance.PCB_Generator;


    // START
    public void Start()
    {
        PCB next = BoardController.Instance.PCB_Generator.GenerateNext();
        next.Unlocked = true;
        NavigateToPCB(next);
    }


    // NAVIGATION
    public void Navigate(int delta_index)
    {
        int next_index = delta_index + (this.pcb ? this.pcb.Index : 0);
        PCB next_pcb = generator.GetClosestUnlockedIndex(next_index);
        // Debug.Log("Navigating to" + next_index + " gave " + next_pcb.name);
        NavigateToPCB(next_pcb);
    }
    public void NavigateToPCB(PCB pcb)
    {
        if (this.pcb != null && pcb == this.pcb)
        {
            pcb.Animator.Play("end_reached");
            return;
        }

        int index_dif = pcb.Index - (this.pcb == null ? 0 : this.pcb.Index);
        
        if (Mathf.Sign(index_dif) > 0)
        {
            // we move both pcb to the left
            if (this.pcb != null) { this.pcb.Animator.Play("stack_to_left"); }
            pcb.Animator.Play("unstack_from_right");
        }
        else
        {
            // we move both pcb to the right
            if (this.pcb != null) { this.pcb.Animator.Play("stack_to_right"); }
            pcb.Animator.Play("unstack_from_left");
        }

        this.pcb = pcb;
        BoardController.Instance.ConnectPCB(pcb);
        // here we may want to reset some things ?
    }
    
    
    // UPDATE PCB
    private void Update()
    {
        bool resolved = true;
        foreach (Problem pb in problems)
        {
            pb.AnalyzePCB(pcb);
            if (!pb.Fixed) { resolved = false; }
        }
        foreach (Info info in infos) { info.AnalyzePCB(pcb); }

        if (!resolved) { return; }

        // we fixed this pcb ! we can now go to the next one
        Resolve();
    }
    public void Resolve()
    {
        if (this.pcb.Fixed) { return; }
        this.pcb.Fixed = true;
        Debug.Log($"FIXED "+pcb.name);
        PCB next = generator.GenerateNext();
        next.Unlocked = true;
        NavigateToPCB(next);
        Debug.Log($"PCB Status :" + generator.GetDetails());
    }


    // PROBLEMS
    public int TotalProblems => problems.Sum(p => p.Quantity);
    public string GetPCB_Details()
    {
        string debug = "";
        foreach (Problem pb in problems)
        {
            if (pb.Fixed && !pb.ShowEvenIfFixed) { continue; }
            debug += "□ " + pb.GetDetails() + "\n\n";
        }
        debug += "\n\n\n";
        foreach (Info info in infos)
        {
            debug += "□ " + info.GetDetails() + "\n\n";
        }
        return debug;
    }
}