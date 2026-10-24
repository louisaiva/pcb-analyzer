using UnityEngine;

public class Button : MonoBehaviour
{

    private Animator animator;
    public Animator Animator => animator;

    [SerializeField] private bool scaled_up = false;
    [SerializeField] private bool logs = false;


    private void Start()
    {
        animator = GetComponentInChildren<Animator>(includeInactive: true);
    }


    ///
    //
    ///  ENTRY POINTS
    //
    ///


    // HOVER ANIMATION
    public void ScaleUp()
    {
        if (logs) { Debug.Log($"(Button) Scaling up {name}"); }
        animator.Play("scale_up");
        scaled_up = true;
    }
    public void ResetScale()
    {
        if (logs) { Debug.Log($"(Button) Resetting scale of {name}"); }
        if (scaled_up)
        {
            if (logs) { Debug.Log($"(Button) Depopping up {name}"); }
            animator.Play("depopup");
        }
        scaled_up = false;
    }


    public virtual void OnClick()
    {
        
    }

}