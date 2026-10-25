using UnityEngine;

public class Info : MonoBehaviour
{
    public void AnalyzePCB(PCB pcb)
    {
        update_info(pcb);
    }
    protected virtual void update_info(PCB pcb) { }
    public virtual string GetDetails()
    {
        return "awesome info";
    }
}