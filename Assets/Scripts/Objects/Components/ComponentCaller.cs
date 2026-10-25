using UnityEngine;

public class ComponentCaller : MonoBehaviour
{
    public Component component;
    public void HealMax()
    {
        if (component == null) { return; }
        component.Heal();
    }
}