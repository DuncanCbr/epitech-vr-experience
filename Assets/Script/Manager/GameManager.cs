using UnityEngine;

public class GameManager : MonoBehaviour
{

    public static GameManager Instance { get; private set; }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    private void Awake()
    {
        KeepGameObjectAcrossScenes();

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void KeepGameObjectAcrossScenes()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
}
