using UnityEngine;

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
}