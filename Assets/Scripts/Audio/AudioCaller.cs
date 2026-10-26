using UnityEngine;

public class AudioCaller : MonoBehaviour
{
    public void SetVolume(float vol)
    {
        AudioManager.LazyInstance.SetMixerVolume(vol);
    }
}