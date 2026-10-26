using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    private static AudioManager _instance;
    public static AudioManager LazyInstance
    {
        get
        {
            if (_instance != null) { return _instance; }
            _instance = FindFirstObjectByType<AudioManager>();
            return _instance;
        }
        set
        {
            _instance = value;
        }
    }

    [SerializeField] public List<AudioBank> banks;
    public AudioMixer mixer;
    public bool disabled = false;
    public void SetMixerVolume(float vol)
    {
        disabled = vol == 0f;
        mixer.SetFloat("sfx", Mathf.Log10(vol)*25);
    }

    public void PlaySFX(string name)
    {
        if (disabled) { return; }
        if (name == "none") { return; }

        AudioSource clip = get_sfx(name);
        if (clip == null) { return; }
        Debug.Log("Playing AUDIO : " + clip.name);
        clip.Play();
    }

    private AudioSource get_sfx(string name)
    {
        foreach (AudioBank bank in banks)
        {
            AudioSource source = bank.GetSFX(name);
            if (source != null) { return source; }
        }
        Debug.LogError("No clip audio found with name : " + name);
        return null;
    }
}

[Serializable] public class SFX
{
    public string name;
    public List<AudioSource> clips;
    public AudioSource RandomClip()
    {
        if (clips == null || clips.Count == 0) { return null;}
        return clips[UnityEngine.Random.Range(0,clips.Count)];
    }
}