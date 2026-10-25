using UnityEngine;

public class Hoverable : MonoBehaviour
{

    [Header("References")]
    [SerializeField] protected SpriteRenderer bg_sr;
    [SerializeField] protected SpriteRenderer sr;
    public SpriteRenderer Renderer => sr;
    [SerializeField] protected Animator animator;
    public Animator Animator => animator;
    // [SerializeField] protected Collider2D hover_collider;
    [SerializeField] protected bool scaled_up = false;
    [SerializeField] protected bool logs = false;



    ///
    //
    ///  ENTRY POINTS
    //
    ///

    // ON HOVER
    public virtual void OnHover() { }
    public virtual void OnUnhover() { }


    // HOVER ANIMATION
    public virtual void ScaleUp()
    {
        if (logs) { Debug.Log($"(Component) Scaling up {name}"); }
        animator.Play("scale_up");
        scaled_up = true;
    }
    public virtual void ResetScale()
    {
        if (logs) { Debug.Log($"(Component) Resetting scale of {name}"); }
        // if (sr.transform.localScale.x <= 1f) { return; }
        if (scaled_up)
        {
            if (logs) { Debug.Log($"(Component) Depopping up {name}"); }
            animator.Play("depopup");
        }
        // else { animator.Play("") }
        // if scale > 1 we were hovered, we play a little animation depopup
        scaled_up = false;
    }

    // ON CLICK
    public virtual void OnClick()
    {
        
    }
}