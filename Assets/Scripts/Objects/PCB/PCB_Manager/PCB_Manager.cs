using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PCB_Manager : MonoBehaviour
{
    public PCB pcb;
    public List<Problem> problems;
    public List<Info> infos;

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