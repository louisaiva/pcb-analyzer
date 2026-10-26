using UnityEngine;

public class ButtonLeftRight : Button
{
    public int movement = 1;
    public override void OnClick()
    {
        BoardController.Instance.PCB_Manager.Navigate(movement);
    }
}