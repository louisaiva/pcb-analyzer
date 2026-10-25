using System.Collections.Generic;
using UnityEngine;

public class RocksManager : HoverableManager
{

    public override void OnMouseRelease()
    {
        if (HasHover(out Rock rock))
        {
            DamageRock(rock);
            return;
        }

    }

    // MAIN METHODS
    public void DamageRock(Rock rock)
    {
        string anim = "";
        if (rock.Life == 1) { anim = "break"; }
        else
        {
            anim = click_anim[UnityEngine.Random.Range(0, click_anim.Count)];
            while (rock.Animator.GetCurrentAnimatorStateInfo(0).IsName(anim))
            {
                anim = click_anim[UnityEngine.Random.Range(0, click_anim.Count)];
            }
        }
        rock.Animator.Play(anim);
        rock.Damage();
    }
}