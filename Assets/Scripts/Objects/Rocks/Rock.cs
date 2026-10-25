using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

public class Rock : Hoverable, Damageable
{

    [Header("Components References")]
    [SerializeField] protected Animator fx_animator;
    [SerializeField] protected HP_Bar bar;

    private void Start()
    {
        InitRock(6);
    }

    public void InitRock(int max_life)
    {
        this.max_life = max_life;
        this.life = max_life;

        play_life_anim();

        bar.Target(this);
    }


    ///
    //
    ///  HOVER
    //
    ///

    public override void OnHover()
    {
        bar.gameObject.SetActive(true);
    }
    public override void OnUnhover()
    {
        bar.gameObject.SetActive(false);
    }

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

        fx_animator.Play("fx_hurt");

        life -= 1;
        if (life == 0) { Die(); }
        Debug.Log($"(Rock) {name} took damage. Has now {life} / {max_life} HP.       ({LifePercentage})");

        play_life_anim();
        bar.RefreshLife();
    }
    private void play_life_anim()
    {
        if (life >= 6) { animator.Play("colony2", 1); }
        else if (life >= 5) { animator.Play("colony", 1); }
        else if (life >= 4) { animator.Play("nest2", 1); }
        else if (life >= 3) { animator.Play("nest", 1); }
        else if (life >= 2) { animator.Play("rock2", 1); }
        else if (life >= 1) { animator.Play("rock", 1); }
        else { animator.Play("die", 1); }
    }
    public async void Die()
    {
        animator.Play("die", 1);
        bar.gameObject.SetActive(false);
        bg_sr.GetComponent<Collider2D>().enabled = false;
        while (animator.GetCurrentAnimatorStateInfo(1).IsName("die"))
        {
            await Task.Yield();
        }

        await Task.Delay(3000);

        Destroy(this.gameObject);
    }

}