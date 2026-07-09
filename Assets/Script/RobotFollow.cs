using UnityEngine;
using UnityEngine.AI;

public class RobotFollow : MonoBehaviour
{
    [Header("Cible")]
    [SerializeField] private Transform target;

    [Header("Paramètres")]
    [SerializeField] private float followDistance = 1.5f;
    [SerializeField] private float moveSpeed = 1.2f;
    [SerializeField] private float rotationSpeed = 5f;

    private NavMeshAgent _agent;

    void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        if (_agent != null)
        {
            _agent.speed = moveSpeed;
            _agent.stoppingDistance = followDistance;
        }

        if (target == null)
        {
            GameObject rig = GameObject.Find("[BuildingBlock] Camera Rig (1)");
            if (rig != null) target = rig.transform;
        }
    }

    void Update()
    {
        if (target == null || _agent == null) return;

        float distance = Vector3.Distance(transform.position, target.position);

        if (distance > followDistance)
            _agent.SetDestination(target.position);
        else
            _agent.ResetPath();

        // Tourne vers le joueur quand proche
        if (distance < followDistance + 1f)
        {
            Vector3 dir = (target.position - transform.position);
            dir.y = 0f;
            if (dir != Vector3.zero)
                transform.rotation = Quaternion.Slerp(transform.rotation,
                    Quaternion.LookRotation(dir), rotationSpeed * Time.deltaTime);
        }
    }
}
