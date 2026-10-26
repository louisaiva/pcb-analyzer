using System.Threading.Tasks;
using UnityEngine;

public class Bug : Hoverable, Damageable
{

    [Header("References")]
    [SerializeField] protected Animator fx_animator;
    [SerializeField] protected HP_Bar bar;
    public string Tier = "bat"; // bat, fat
    public BoxCollider2D health;
    public BugBehaviour bb;

    public void InitBug(int life)
    {
        this.max_life = life % 2 == 0 ? life : life + 1;
        this.life = life;

        refresh_tier();

        bar.Target(this);
        bar.gameObject.SetActive(false);

        bb.Init();
    }


    ///
    //
    ///  HOVER
    //
    ///

    public override void OnHover()
    {
        bar.gameObject.SetActive(true);
        if (bb.CanDash()) { bb.SwitchState("dash"); }
    }
    public override void OnUnhover()
    {
        bar.gameObject.SetActive(false);
    }

    /* public override void ScaleUp() {}
    public override void ResetScale() {} */

    ///
    //
    ///  HEALING / DAMAGING
    //
    ///
    [Header("Life")]
    [SerializeField] protected int life;
    [SerializeField] protected int max_life;
    public int MaxLife { get => max_life; set => max_life = value; }
    public int MaxHP => MaxLife / 2;
    public int Life { get => life; set => life = value; }
    public float LifePercentage => Life / (float)MaxLife;
    public bool IsDamaged => life != max_life;
    public bool IsDead => life <= 0;


    // DAMAGE & DIE
    public virtual void Damage()
    {
        if (IsDead) { return; }

        animator.Play(Tier +"_damaged", 1);
        fx_animator.Play("fx_bleed");
        AudioManager.LazyInstance.PlaySFX("bug_dmg");

        life -= 1;
        refresh_tier();
        bar.RefreshLife();

        if (life == 0) { Die(); return; }
        Debug.Log($"(Bug) {name} took damage. Has now {life} / {max_life} HP.       ({LifePercentage})");

        bb.SwitchState("flee");
    }
    private void refresh_tier()
    {
        string new_tier = "";
        if (life > 2) { new_tier = "fat"; }
        else { new_tier = "bat"; }
        
        if (new_tier != Tier)
        {
            Vector2 size = Vector2.one;
            if (new_tier == "bat") { size /= 2f; }
            health.size = size;

            // also change current animation ?
        }

        Tier = new_tier;
    }
    public async void Die()
    {
        animator.Play(Tier + "_die", 1);
        AudioManager.LazyInstance.PlaySFX("bug_die");
        bar.gameObject.SetActive(false);
        bb.Stop();
        bb.enabled = false;
        health.enabled = false;
        await Task.Delay(1000);
        Destroy(this.gameObject);
    }

}