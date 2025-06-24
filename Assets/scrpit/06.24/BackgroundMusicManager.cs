using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class BackgroundMusicManager : MonoBehaviour
{
    public AudioClip bgmClip;
    [Range(0f, 1f)] public float volume = 0.5f;

    private static BackgroundMusicManager instance;
    private AudioSource audioSource;

    void Awake()
    {
        // �̱��� ó��: �̹� �����ϸ� �ߺ� �ı�
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);

        audioSource = GetComponent<AudioSource>();
        audioSource.clip = bgmClip;
        audioSource.volume = volume;
        audioSource.loop = true;
        audioSource.playOnAwake = false;
        audioSource.Play();
    }
}
