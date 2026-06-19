using UnityEngine;

public class RoomAudioTrigger : MonoBehaviour
{
    public enum RoomType
    {
        Couloir,
        Salle211,
        Salle212,
        Salle213,
        Salle214,
        Salle215,
        Salle216
    }

    [SerializeField] private RoomType room;

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        AudioManager am = AudioManager.Instance;
        if (am == null) return;

        am.PlaySFX(am.sfxEntreeSalle);

        AudioClip ambience = room switch
        {
            RoomType.Couloir   => am.ambienceCouloir,
            RoomType.Salle211  => am.ambienceSalle211,
            RoomType.Salle212  => am.ambienceSalle212,
            RoomType.Salle213  => am.ambienceSalle213,
            RoomType.Salle214  => am.ambienceSalle214,
            RoomType.Salle215  => am.ambienceSalle215,
            RoomType.Salle216  => am.ambienceSalle216,
            _                  => null
        };

        am.PlayAmbience(ambience);
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        AudioManager.Instance?.PlaySFX(AudioManager.Instance.sfxSortieSalle);
    }
}
