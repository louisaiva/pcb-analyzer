using UnityEngine;

public interface Damageable
{
    public GameObject gameObject { get; }
    public int MaxLife { get; set; }
    public int Life { get; set; }

    public void Damage();
    public void Die();
}