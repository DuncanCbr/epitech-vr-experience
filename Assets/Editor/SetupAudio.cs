using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

public class SetupAudio
{
    [MenuItem("EPITECH VR/Setup Audio Scene")]
    static void Setup()
    {
        // --- AudioManager ---
        GameObject existing = GameObject.Find("AudioManager");
        if (existing != null)
        {
            Debug.Log("AudioManager already exists in scene.");
        }
        else
        {
            GameObject go = new GameObject("AudioManager");

            AudioSource ambienceSource = go.AddComponent<AudioSource>();
            ambienceSource.loop = true;
            ambienceSource.playOnAwake = false;
            ambienceSource.spatialBlend = 0f;
            ambienceSource.volume = 0.5f;

            AudioSource sfxSource = go.AddComponent<AudioSource>();
            sfxSource.loop = false;
            sfxSource.playOnAwake = false;
            sfxSource.spatialBlend = 0f;
            sfxSource.volume = 1f;

            AudioManager manager = go.AddComponent<AudioManager>();

            SerializedObject so = new SerializedObject(manager);
            so.FindProperty("ambienceSource").objectReferenceValue = ambienceSource;
            so.FindProperty("sfxSource").objectReferenceValue = sfxSource;

            so.FindProperty("ambienceCouloir").objectReferenceValue =
                AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/Ambience/ambience_couloir.wav");
            so.FindProperty("ambienceSalle211").objectReferenceValue =
                AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/Ambience/ambience_salle.wav");
            so.FindProperty("ambienceSalle212").objectReferenceValue =
                AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/Ambience/ambience_salle.wav");
            so.FindProperty("ambienceSalle213").objectReferenceValue =
                AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/Ambience/ambience_salle.wav");
            so.FindProperty("ambienceSalle214").objectReferenceValue =
                AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/Ambience/ambience_salle.wav");
            so.FindProperty("ambienceSalle215").objectReferenceValue =
                AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/Ambience/ambience_salle.wav");
            so.FindProperty("ambienceSalle216").objectReferenceValue =
                AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/Ambience/ambience_salle.wav");

            so.FindProperty("sfxEntreeSalle").objectReferenceValue =
                AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/SFX/door open.mp3");
            so.FindProperty("sfxSortieSalle").objectReferenceValue =
                AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/SFX/door close.mp3");

            so.ApplyModifiedProperties();

            Debug.Log("AudioManager created and configured.");
        }

        // --- RoomAudioTriggers ---
        string[] rooms = { "Couloir", "Salle211", "Salle212", "Salle213", "Salle214", "Salle215", "Salle216" };

        foreach (string room in rooms)
        {
            string triggerName = "Trigger_" + room;
            if (GameObject.Find(triggerName) != null) continue;

            GameObject trigger = new GameObject(triggerName);
            BoxCollider col = trigger.AddComponent<BoxCollider>();
            col.isTrigger = true;
            col.size = new Vector3(5f, 3f, 5f);

            RoomAudioTrigger rat = trigger.AddComponent<RoomAudioTrigger>();
            SerializedObject so = new SerializedObject(rat);
            so.FindProperty("room").enumValueIndex = System.Array.IndexOf(rooms, room);
            so.ApplyModifiedProperties();

            Debug.Log($"Trigger created: {triggerName}");
        }

        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        Debug.Log("Audio setup complete. Position the triggers on each room, then save the scene.");
    }
}
