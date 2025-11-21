using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CascarudoMOvement : MonoBehaviour
{
    [Header("Waypoints")]
    public Transform[] waypoints;
    public float speed = 2f;
    public float arriveDistance = 0.2f;

    [Header("Animación")]
    public Animator animator;
    public string walkAnimationName = "Armature-cascarudo|Action_Walk-fast"; // el nombre del estado en el Animator

    private int currentWaypoint = 0;

    void Start()
    {
        animator.Play("Armature-cascarudo|Action_Walk-Normal");

        if (animator != null)
        {
            animator.Play(walkAnimationName);
        }
    }

    void Update()
    {
        if (waypoints.Length == 0) return;

        Transform target = waypoints[currentWaypoint];

        // Mover hacia el waypoint
        Vector3 direction = (target.position - transform.position).normalized;
        transform.position += direction * speed * Time.deltaTime;

        // Rotar hacia el objetivo si querés
        if (direction != Vector3.zero)
        {
            transform.rotation = Quaternion.Lerp(
                transform.rotation,
                Quaternion.LookRotation(direction),
                Time.deltaTime * 5f
            );
        }

        // ¿Llegó al waypoint?
        if (Vector3.Distance(transform.position, target.position) < arriveDistance)
        {
            currentWaypoint++;

            // Volver al inicio si ya recorrio todos
            if (currentWaypoint >= waypoints.Length)
                currentWaypoint = 0;
        }
    }
}