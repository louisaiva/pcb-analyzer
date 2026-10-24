using UnityEngine;

public class Button : MonoBehaviour
{

    private Animator animator;
    public Animator Animator => animator;

    private void Start()
    {
        animator = GetComponentInChildren<Animator>(includeInactive: true);
    }
    public void ScaleUp()
    {
        // slowly rise the scale
        animator.Play("scale_up");
    }
    public void ResetScale()
    {
        if (animator.transform.localScale.x <= 1f) { return; }
        // if scale > 1 we were hovered, we play a little animation depopup
        animator.Play("depopup");
    }

    public virtual void OnClick()
    {
        
    }

}