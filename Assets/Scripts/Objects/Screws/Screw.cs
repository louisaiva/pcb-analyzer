using UnityEngine;

public class Screw : MonoBehaviour
{

    private SpriteRenderer sr;
    public SpriteRenderer Renderer => sr;
    private Animator animator;
    public Animator Animator => animator;

    private void Start()
    {
        sr = GetComponentInChildren<SpriteRenderer>(includeInactive: true);
        animator = GetComponentInChildren<Animator>(includeInactive: true);
    }

    public void DisableGameObject()
    {
        gameObject.SetActive(false);
    }
}