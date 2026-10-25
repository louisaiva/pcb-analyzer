using UnityEngine;

public class BoardController : MonoBehaviour
{
    public static BoardController Instance = null;
    private void Awake()
    {
        Instance = this;
    }


    public PCB_Manager PCB_Manager;
    public ButtonsManager Btner;
    public ComponentsManager Comper;
    public RocksManager Rocker;
    public ComponentBank CompBank;
    public PCB PCB;
    public PCB_Bank PCBank;
    public StickerBank StickerBank;

    public void OnHover(Hoverable hoverable)
    {
        if (hoverable is Button button)
        {
            Btner.Hover(button);
            Comper.Unhover();
            Rocker.Unhover();
        }
        else if (hoverable is Component comp)
        {
            Comper.Hover(comp);
            Btner.Unhover();
            Rocker.Unhover();
        }
        else if (hoverable is Rock rock)
        {
            Rocker.Hover(rock);
            Btner.Unhover();
            Comper.Unhover();
        }
    }
    public void OnNothingHovered()
    {
        Btner.Unhover();
        Comper.Unhover();
        Rocker.Unhover();
    }

    public void OnMousePress()
    {
        // if (CompManager.HasHoveredComponent(out Component comp)) { CompManager.StartHolding(comp); }
        Btner.OnMousePress();
        Comper.OnMousePress();
        Rocker.OnMousePress();
    }

    public void OnMouseRelease()
    {
        if (PCB.IsShowingFront && !PCB.Timer.IsRunning) { PCB.Flip(); return; }
        Btner.OnMouseRelease();
        Comper.OnMouseRelease();
        Rocker.OnMouseRelease();
    }
}