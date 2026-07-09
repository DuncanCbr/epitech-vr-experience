using UnityEngine;

public class CollectibleItem : MonoBehaviour
{
    bool collected;

    public void Collect()
    {
        if (collected)
            return;

        if (GameManagerVR.Instance.CompleteStep(gameObject))
        {
            collected = true;
            Destroy(gameObject);
        }
    }
}