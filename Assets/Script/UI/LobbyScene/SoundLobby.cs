using UnityEngine;

public class SoundLobby : MonoBehaviour
{

    public static SoundLobby Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    [SerializeField]
    private AudioSource audioSource;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void StartAmbianceLobbySound()
    {
        audioSource.Play();
    }      
}
