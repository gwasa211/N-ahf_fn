using UnityEngine;

public class UIAudioManager : MonoBehaviour
{
    public static UIAudioManager Instance { get; private set; }

    [Header("UI Sfx")]
    public AudioClip clickClip;
    [Range(0f, 1f)] public float clickVolume = 1f;

    AudioSource audioSource;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.loop = false;
    }

    public void PlayClick()
    {
        if (clickClip != null)
            audioSource.PlayOneShot(clickClip, clickVolume);
    }
}
