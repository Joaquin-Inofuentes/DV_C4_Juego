using UnityEngine;
using System.Linq;

public class PatrolState : FSMState
{
    private int _targetWaypointIndex = 0;

    public override void Enter(Agent agent)
    {
        Hunter hunter = (Hunter)agent;
        hunter.SetDebugInfo(Color.yellow, "WayPoints");
        hunter.currentHunterState = HunterState.Patrolling;
        // Al entrar, reseteamos la memoria y encontramos el punto de partida más cercano.
        hunter.lastWaypointVisitedIndex = -1;
        _targetWaypointIndex = FindClosestWaypointIndex(hunter);
    }

    public override void Execute(Agent agent)
    {
        Hunter hunter = (Hunter)agent;
        var waypoints = EntityManager.Instance.patrolWaypoints;

        if (FindClosestBoid(hunter, out Boid closestBoid))
        {
            hunter.ChangeState(new HuntingState(closestBoid));
            return;
        }

        if (waypoints.Count == 0) return;

        Vector3 currentTargetPosition = waypoints[_targetWaypointIndex].position;
        hunter.distanceToTarget = Vector3.Distance(hunter.transform.position, currentTargetPosition);

        // --- LÓGICA CORREGIDA ---

        // 1. Comportamiento Base (Secuencial): ¿Hemos llegado?
        if (hunter.distanceToTarget < hunter.waypointArrivalDistance)
        {
            Debug.Log($"<color=green>LLEGÓ:</color> Cruzó el waypoint #{_targetWaypointIndex}.");

            // Actualizamos la "memoria" con el índice que acabamos de visitar.
            hunter.lastWaypointVisitedIndex = _targetWaypointIndex;

            // Pasamos al siguiente waypoint en la secuencia.
            _targetWaypointIndex = (hunter.lastWaypointVisitedIndex + 1) % waypoints.Count;

            Debug.Log($"<color=cyan>SECUENCIA:</color> Nuevo objetivo es el waypoint #{_targetWaypointIndex}.");

            // Actualizamos la posición objetivo para el resto de la lógica.
            currentTargetPosition = waypoints[_targetWaypointIndex].position;
        }
        else // 2. Comportamiento de Anulación (Dinámico): Solo se ejecuta si NO hemos llegado.
        {
            int absoluteClosestIndex = FindClosestWaypointIndex(hunter);

            // La anulación solo ocurre si el punto más cercano NO es nuestro objetivo actual
            // Y, crucialmente, NO es el que acabamos de visitar.
            if (absoluteClosestIndex != _targetWaypointIndex && absoluteClosestIndex != hunter.lastWaypointVisitedIndex)
            {
                float distanceToClosest = Vector3.Distance(hunter.transform.position, waypoints[absoluteClosestIndex].position);

                if (distanceToClosest < hunter.distanceToTarget * hunter.dynamicRepathFactor)
                {
                    Debug.LogWarning($"<color=orange>ANULACIÓN DINÁMICA:</color> Waypoint #{absoluteClosestIndex} es mucho más cercano. Cambiando de objetivo.");
                    _targetWaypointIndex = absoluteClosestIndex;
                    currentTargetPosition = waypoints[_targetWaypointIndex].position;
                }
            }
        }

        // 3. Acción Final: Dibujar la línea hacia el objetivo final.
        Debug.DrawLine(hunter.transform.position, currentTargetPosition, Color.yellow);
        DebugHelper.DrawCircle(currentTargetPosition, hunter.waypointArrivalDistance, Color.cyan);

        hunter.energy -= 2 * Time.deltaTime;
        if (hunter.energy <= 0)
        {
            hunter.ChangeState(new IdleState());
        }
    }

    public override void Exit(Agent agent)
    {
        Hunter hunter = (Hunter)agent;
        hunter.distanceToTarget = 0;
        hunter.lastWaypointVisitedIndex = -1;
    }

    private int FindClosestWaypointIndex(Hunter hunter)
    {
        var waypoints = EntityManager.Instance.patrolWaypoints;
        if (waypoints.Count == 0) return 0;

        float closestDistSqr = float.MaxValue;
        int closestIndex = 0;
        for (int i = 0; i < waypoints.Count; i++)
        {
            float distSqr = (hunter.transform.position - waypoints[i].position).sqrMagnitude;
            if (distSqr < closestDistSqr)
            {
                closestDistSqr = distSqr;
                closestIndex = i;
            }
        }
        return closestIndex;
    }

    private bool FindClosestBoid(Hunter hunter, out Boid foundBoid)
    {
        foundBoid = EntityManager.Instance.boids
            .Where(b => b != null && b.gameObject.activeInHierarchy)
            .Where(b => Vector3.Distance(hunter.transform.position, b.transform.position) < hunter.sightRadius)
            .OrderBy(b => Vector3.Distance(hunter.transform.position, b.transform.position))
            .FirstOrDefault();
        return foundBoid != null;
    }
}