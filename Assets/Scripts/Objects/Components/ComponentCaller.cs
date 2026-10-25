using UnityEngine;

public class ComponentCaller : MonoBehaviour
{
    public Component component;
    public void HealMax()
    {
        component.Heal();
    }
}