using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

public class Rock : Hoverable, Damageable
{

    [Header("Components References")]
    [SerializeField] protected Animator fx_animator;
    [SerializeField] protected HP_Bar bar;
    public RailCross cross;

    public void InitRock(int life, RailCross cross)
    {
        this.max_life = life % 2 == 0 ? life : life + 1;
        this.life = life;
        this.cross = cross;
        cross.Rocked = true;

        play_life_anim();

        bar.Target(this);
        bar.gameObject.SetActive(false);

        DisableSpawning = true;
        spawn_cooldown = Random.Range(8f,15f);
    }

    public bool DisableSpawning = false;
    public float spawn_cooldown = 0f;
    private void Update()
    {
        if (DisableSpawning) { return; }
        if (spawn_cooldown > 0f)
        {
            spawn_cooldown -= Time.deltaTime;
            return;
        }

        spawn_bug();
        spawn_cooldown = Random.Range(12f, 30f);
    }

    private void spawn_bug()
    {
        if (life < 3) { return; }

        int buglife = 1;
        if (life == 4) { buglife = Random.Range(1, 3); }
        if (life == 5) { buglife = Random.Range(1, 4); }
        if (life == 6) { buglife = Random.Range(2, 5); }

        Vector2 spawn_position = (Vector2)transform.position + Random.onUnitCircle * 0.08f;
        BoardController.Instance.PCB.CreateBug(buglife,spawn_position);
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
        cross.Rocked = false;

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