using UnityEngine;

public class BoardController : MonoBehaviour
{
    public static BoardController Instance = null;
    private void Awake()
    {
        Instance = this;
    }


    public PCB_Manager PCB_Manager;
    public ButtonsManager BtnManager;
    public ComponentsManager CompManager;
    public ComponentBank CompBank;
    public PCB PCB;
    public PCB_Bank PCBank;
    public StickerBank StickerBank;

    public void OnHover(Hoverable hoverable)
    {
        if (hoverable is Button button)
        {
            BtnManager.Hover(button);
            CompManager.Unhover();
        }
        else if (hoverable is Component comp)
        {
            CompManager.Hover(comp);
            BtnManager.Unhover();
        }
    }
    public void OnNothingHovered()
    {
        BtnManager.Unhover();
        CompManager.Unhover();
    }

    public void OnMousePress()
    {
        // if (CompManager.HasHoveredComponent(out Component comp)) { CompManager.StartHolding(comp); }
        BtnManager.OnMousePress();
        CompManager.OnMousePress();
    }

    public void OnMouseRelease()
    {
        // if (PCB.IsShowingFront) { PCB.Flip(); return; }
        // if (BtnManager.HasHover(out Button btn)) { BtnManager.ClickOnButton(btn); return; }
        BtnManager.OnMouseRelease();
        CompManager.OnMouseRelease();
        // if (CompManager.HasHoveredComponent(out Component comp)) { CompManager.MouseUpOn(comp); }
    }
}