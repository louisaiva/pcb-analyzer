using UnityEngine;
using TMPro;

public class PCB_Description : MonoBehaviour
{
    public TMPro.TextMeshPro qty;
    public TMPro.TextMeshPro title;
    public TMPro.TextMeshPro details;
    public Animator pcb_fixed_animator;
    private PCB_Manager manager => BoardController.Instance.PCB_Manager;

    private bool was_fixed_last_frame = false;
    private void Update()
    {
        if (manager.TotalProblems > 0)
        {
            qty.text = manager.TotalProblems.ToString();
            title.text = "ISSUES ON BOARD";
            details.text = manager.GetPCB_Details();
            was_fixed_last_frame = false;
            pcb_fixed_animator.Play("idle");
            return;
        }

        qty.text = "";
        title.text = "";
        details.text = "";
        if (!was_fixed_last_frame)
        {
            pcb_fixed_animator.Play("fixed");
        }
        was_fixed_last_frame = true;
    }
}