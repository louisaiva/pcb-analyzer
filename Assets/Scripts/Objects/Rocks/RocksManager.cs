using System.Collections.Generic;
using UnityEngine;

public class RocksManager : HoverableManager
{

    public List<Rock> GetRocks()
    {
        return hoverables.ConvertAll(h => (Rock)h);
    }

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
        // if we are 1 we will play the die animation either way so it is ok :)
        
        if (rock.Life != 1)
        {
            string anim = click_anim[UnityEngine.Random.Range(0, click_anim.Count)];
            while (rock.Animator.GetCurrentAnimatorStateInfo(0).IsName(anim))
            {
                anim = click_anim[UnityEngine.Random.Range(0, click_anim.Count)];
            }
            rock.Animator.Play(anim);
        }



        rock.Damage();

        if (rock.IsDead) { hoverables.Remove(rock); }
    }
}