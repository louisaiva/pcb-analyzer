using UnityEngine;

public class Problem : MonoBehaviour
{
    public bool Fixed = false;
    public int Quantity = 0;
    public bool ShowEvenIfFixed = false;

    public void AnalyzePCB(PCB pcb)
    {
        update_problem(pcb);
        Fixed = Quantity == 0;
    }
    protected virtual void update_problem(PCB pcb) { }
    public virtual string GetDetails()
    {
        return Quantity + " problems";
    }
}