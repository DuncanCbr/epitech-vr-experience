using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Sources")]
    [SerializeField] private AudioSource ambienceSource;
    [SerializeField] private AudioSource sfxSource;

    [Header("Ambiences par zone")]
    public AudioClip ambienceCouloir;
    public AudioClip ambienceSalle211;
    public AudioClip ambienceSalle212;
    public AudioClip ambienceSalle213;
    public AudioClip ambienceSalle214;
    public AudioClip ambienceSalle215;
    public AudioClip ambienceSalle216;

    [Header("SFX")]
    public AudioClip sfxEntreeSalle;
    public AudioClip sfxSortieSalle;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void PlayAmbience(AudioClip clip)
    {
        if (clip == null || ambienceSource.clip == clip) return;

        ambienceSource.Stop();
        ambienceSource.clip = clip;
        ambienceSource.loop = true;
        ambienceSource.Play();
    }

    public void PlaySFX(AudioClip clip)
    {
        if (clip == null) return;
        sfxSource.PlayOneShot(clip);
    }

    public void StopAmbience()
    {
        ambienceSource.Stop();
    }
}
