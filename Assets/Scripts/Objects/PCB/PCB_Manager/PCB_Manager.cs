using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PCB_Manager : MonoBehaviour
{
    public PCB pcb;
    public List<Problem> problems;
    public List<Info> infos;



    public void Start()
    {
        PCB next = BoardController.Instance.PCB_Generator.GenerateNext();
        NavigateToPCB(next);
    }


    public void NavigateToPCB(PCB pcb)
    {

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
        PCB next = BoardController.Instance.PCB_Generator.GenerateNext();
        NavigateToPCB(next);
    }

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