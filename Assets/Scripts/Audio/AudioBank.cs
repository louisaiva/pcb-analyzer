using System.Collections.Generic;
using UnityEngine;

public class AudioBank : MonoBehaviour
{
    [SerializeField] public List<SFX> fx;

    public AudioSource GetSFX(string name)
    {
        foreach (SFX sfx in fx)
        {
            if (sfx.name == name) { return sfx.RandomClip(); }
        }
        // Debug.LogError("No clip audio found with name : " + name);
        return null;
    }

}