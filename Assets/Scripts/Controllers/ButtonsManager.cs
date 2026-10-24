using System.Collections.Generic;
using UnityEngine;

public class ButtonsManager : MonoBehaviour
{

    [Header("Hover")]
    [SerializeField] private Button hovered_button = null;

    [Header("Click animations")]
    [SerializeField] private List<string> click_anim = new List<string>();

    [Header("Logs")]
    [SerializeField] private bool log_hovers = false;

    public void HoverButton(Button btn)
    {
        if (btn == null) { return; }
        if (btn == hovered_button) { return; }
        if (btn != null) { btn.ResetScale(); }
        btn.ScaleUp();
        hovered_button = btn;
        if (log_hovers) { Debug.Log($"(ButtonsManager) Hovered button {btn.name}"); }
    }
    public void UnhoverButton() => UnhoverButton(hovered_button);
    public void UnhoverButton(Button comp)
    {
        if (comp == null) { return; }
        if (comp != hovered_button) { return; }
        comp.ResetScale();
        hovered_button = null;
        if (log_hovers) { Debug.Log($"(ButtonsManager) Unhover button {comp.name}"); }
    }

    // GETTERS
    public bool HasHoveredButton(out Button btn)
    {
        btn = hovered_button;
        return btn != null;
    }
    public void ClickOnButton(Button btn)
    {
        string anim = click_anim[UnityEngine.Random.Range(0, click_anim.Count)];
        while (btn.Animator.GetCurrentAnimatorStateInfo(0).IsName(anim))
        {
            anim = click_anim[UnityEngine.Random.Range(0, click_anim.Count)];
        }
        btn.Animator.Play(anim);
        btn.OnClick();
    }
}