using UnityEngine;

public class StepTrigger : MonoBehaviour
{
    bool done;

    private void OnTriggerEnter(Collider other)
    {
        if (done)
            return;

        if (!other.CompareTag("Player"))
            return;

        if (GameManagerVR.Instance.CompleteStep(gameObject))
        {
            done = true;
        }
    }
}