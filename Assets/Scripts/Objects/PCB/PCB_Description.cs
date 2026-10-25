using UnityEngine;
using TMPro;

public class PCB_Description : MonoBehaviour
{
    public TMPro.TextMeshPro qty;
    public TMPro.TextMeshPro title;
    public TMPro.TextMeshPro details;
    private PCB_Manager manager => BoardController.Instance.PCB_Manager;
    private void Update()
    {
        if (manager.TotalProblems > 0)
        {
            qty.text = manager.TotalProblems.ToString();
            title.text = "ISSUES ON BOARD";
            details.text = manager.GetPCB_Details();
            return;
        }

        qty.text = "";
        title.text = "BOARD FIXED";
        details.text = "";
    }
}