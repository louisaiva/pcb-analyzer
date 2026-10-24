using UnityEngine;

public class BoardController : MonoBehaviour
{
    public static BoardController Instance = null;
    private void Awake()
    {
        Instance = this;
    }


    public ButtonsManager BtnManager;
    public ComponentsManager CompManager;
    public ComponentBank CompBank;
    public PCB PCB;

    public void OnButtonHover(Button btn)
    {
        BtnManager.HoverButton(btn);
    }
    public void OnComponentHover(Component comp)
    {
        CompManager.HoverComponent(comp);
    }

    public void OnNothingHovered()
    {
        BtnManager.UnhoverButton();
        CompManager.UnhoverComponent();
    }

    public void OnMouseClick()
    {
        if (PCB.IsShowingFront) { PCB.Flip(); return; }
        if (BtnManager.HasHoveredButton(out Button btn)) { BtnManager.ClickOnButton(btn); return; }
        if (CompManager.HasHoveredComponent(out Component comp)) { CompManager.ClickOnComponent(comp); }
    }
}