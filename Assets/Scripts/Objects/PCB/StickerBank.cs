using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StickerBank : MonoBehaviour
{
    [Header("Stickers")]
    public List<Sticker> stickers;

    public Sprite GetRandomSprite()
    {
        stickers.Sort((a,b) => (int)(a.apparition_freq*1000 - b.apparition_freq*1000));

        float total_freq = stickers.Sum(s => s.apparition_freq);
        float random_nb = UnityEngine.Random.value * total_freq;
        float ponderation = 0f;
        foreach (Sticker stick in stickers)
        {
            ponderation += stick.apparition_freq;
            if (ponderation < random_nb) { continue; }
            return stick.sprite;
        }
        return stickers[0].sprite;
    }
}

[Serializable] public class Sticker
{
    public float apparition_freq = 1f;
    public Sprite sprite;
}