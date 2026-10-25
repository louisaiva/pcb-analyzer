using UnityEngine;

public class ElapsedTime : Info
{
    private PCB_Timer timer = null;

    protected override void update_info(PCB pcb)
    {
        if (timer != pcb.Timer) { timer = pcb.Timer; }
    }
    public override string GetDetails()
    {
        if (timer == null) { return "no timer"; }
        return timer.elapsed_time.ToString("F2") + " s";
    }
}