using UnityEngine;

public class HP : MonoBehaviour
{
    [SerializeField] protected SpriteRenderer sr;
    [SerializeField] protected Sprite full;
    [SerializeField] protected Sprite mid;
    [SerializeField] protected Sprite dead;
    public void SetLife(int life)
    {
        sr.sprite
            = life == 2 ? full
            : life == 1 ? mid
            : dead;
    }
}