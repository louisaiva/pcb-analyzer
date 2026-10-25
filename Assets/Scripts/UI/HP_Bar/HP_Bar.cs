using System.Collections.Generic;
using UnityEngine;

public class HP_Bar : MonoBehaviour
{
    public List<HP> hps = new List<HP>();
    public Damageable target;
    public HP prefab;
    public float inter_hp = 4f / 24f;


    // MAIN METHODS
    public void Clear()
    {
        foreach (HP hp in hps) { Destroy(hp.gameObject); }
        hps.Clear();
    }
    public void Untarget()
    {
        Clear();
        target = null;
    }
    public void Target(Damageable dmg, bool centered = true)
    {
        Clear();

        int hp_count = dmg.MaxHP;
        float total_width = inter_hp * (hp_count - 1);
        float left = centered ? - total_width / 2f : 0f;
        List<float> hp_positions = new List<float>();
        for (int i=0; i<hp_count;i++)
        {
            hp_positions.Add( left + inter_hp*i );
        }

        foreach (float x in hp_positions)
        {
            HP hp = create_hp(new Vector2(x,0f));
            hps.Add(hp);
        }

        target = dmg;
        RefreshLife();
    }
    private HP create_hp(Vector2 pos)
    {
        HP hp = Instantiate(prefab, this.transform);
        hp.transform.localPosition = pos;
        hp.transform.localScale = Vector3.one;
        return hp;
    }

    public void RefreshLife()
    {
        if (target == null || hps.Count == 0) return;

        int life = Mathf.Max(0, target.Life);
        int baseLife = life / hps.Count;
        int remainder = life % hps.Count;

        for (int i = 0; i < hps.Count; i++)
        {
            int distributedLife = baseLife + (i < remainder ? 1 : 0);
            hps[i].SetLife(distributedLife);
        }
    }
}