using UnityEngine;

public class BoardController : MonoBehaviour
{
    public static BoardController Instance = null;
    private void Awake()
    {
        Instance = this;
    }



    public Vector3 WorldMouse = Vector3.zero;
    public PCB_Generator PCB_Generator;
    public PCB_Manager PCB_Manager;
    public PCB PCB => PCB_Manager.pcb;
    public ButtonsManager Btner;
    public ComponentsManager Comper;
    public RocksManager Rocker;
    public BugManager Bugger;
    public ComponentBank CompBank;
    public PCB_Bank PCBank;
    public StickerBank StickerBank;

    // PCB CONNECTION
    public void ConnectPCB(PCB pcb)
    {
        Comper = pcb.Componer;
        Rocker = pcb.Rocker;
        Bugger = pcb.Bugger;
    }

    // MOUSE EVENTS TRANSMISSIONS
    public void OnHover(Hoverable hoverable)
    {
        if (hoverable is Button button)
        {
            Btner.Hover(button);
            Comper.Unhover();
            Rocker.Unhover();
            Bugger.Unhover();
        }
        else if (hoverable is Component comp)
        {
            Comper.Hover(comp);
            Btner.Unhover();
            Rocker.Unhover();
            Bugger.Unhover();
        }
        else if (hoverable is Rock rock)
        {
            Rocker.Hover(rock);
            Btner.Unhover();
            Comper.Unhover();
            Bugger.Unhover();
        }
        else if (hoverable is Bug bug)
        {
            Bugger.Hover(bug);
            Rocker.Unhover();
            Btner.Unhover();
            Comper.Unhover();
        }
    }
    public void OnNothingHovered()
    {
        Btner.Unhover();
        Comper.Unhover();
        Rocker.Unhover();
        Bugger.Unhover();
    }
    public void OnMousePress()
    {
        // if (CompManager.HasHoveredComponent(out Component comp)) { CompManager.StartHolding(comp); }
        Btner.OnMousePress();
        Comper.OnMousePress();
        Rocker.OnMousePress();
        Bugger.OnMousePress();
    }
    public void OnMouseRelease()
    {
        if (PCB.IsShowingFront && !PCB.Timer.IsRunning) { PCB.Flip(); return; }
        Btner.OnMouseRelease();
        Comper.OnMouseRelease();
        Rocker.OnMouseRelease();
        Bugger.OnMouseRelease();
    }
}