using UnityEngine;

public class ButtonFlip : Button
{
    public override void OnClick()
    {
        BoardController.Instance.PCB.Flip();
    }
}