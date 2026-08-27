using UnityEngine;

public class BillboardUI : MonoBehaviour
{
    private Transform _cam;

    void Start()
    {
        _cam = Camera.main?.transform;
    }

    void LateUpdate()
    {
        if (_cam == null) return;

        Vector3 direction = transform.position - _cam.position;
        direction.y = 0f;

        if (direction != Vector3.zero)
            transform.rotation = Quaternion.LookRotation(direction);
    }
}
