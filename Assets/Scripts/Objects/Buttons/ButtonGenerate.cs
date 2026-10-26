using UnityEngine;

public class ButtonGenerate : Button
{
    public override void OnClick()
    {
        BoardController.Instance.PCB_Generator.GenerateNext();
    }
}