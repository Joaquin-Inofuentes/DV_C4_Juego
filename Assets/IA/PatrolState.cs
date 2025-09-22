using UnityEngine;
using System.Linq;

public class PatrolState : FSMState
{
    // Almacena el índice del waypoint que el cazador está persiguiendo actualmente.
    private int _targetWaypointIndex = 0;

    // Se ejecuta al entrar en el estado de patrulla.
    public override void Enter(Agent agent)
    {
        // Convierte el 'Agent' genérico a un 'Hunter' para acceder a sus variables específicas.
        Hunter hunter = (Hunter)agent;
        // Establece la información de depuración (color y texto).
        hunter.SetDebugInfo(Color.yellow, "WayPoints");
        // Actualiza el enum de estado en el Inspector.
        hunter.currentHunterState = HunterState.Patrolling;
        // Al entrar, resetea la memoria del último punto visitado.
        hunter.lastWaypointVisitedIndex = -1;
        // Y encuentra el waypoint más cercano para empezar la patrulla.
        _targetWaypointIndex = FindClosestWaypointIndex(hunter);
    }

    // Se ejecuta en cada frame mientras está patrullando.
    public override void Execute(Agent agent)
    {
        Hunter hunter = (Hunter)agent;
        var waypoints = EntityManager.Instance.patrolWaypoints;

        // --- PRIORIDAD 1: SUPERVIVENCIA Y OPORTUNIDAD ---
        // Busca boids cercanos. Si encuentra uno, cambia al estado de caza y termina.
        if (FindClosestBoid(hunter, out Boid closestBoid))
        {
            hunter.ChangeState(new HuntingState(closestBoid));
            return;
        }

        // Si no hay waypoints definidos, no hace nada más.
        if (waypoints.Count == 0) return;

        // --- LÓGICA DE MOVIMIENTO Y DECISIÓN DE PATRULLA ---
        Vector3 currentTargetPosition = waypoints[_targetWaypointIndex].position;
        hunter.distanceToTarget = Vector3.Distance(hunter.transform.position, currentTargetPosition);

        // 1. Comportamiento Base (Secuencial): ¿Hemos llegado a nuestro objetivo?
        if (hunter.distanceToTarget < hunter.waypointArrivalDistance)
        {
            Debug.Log($"<color=green>LLEGÓ:</color> Cruzó el waypoint #{_targetWaypointIndex}.");
            hunter.lastWaypointVisitedIndex = _targetWaypointIndex; // Actualiza la "memoria".
            _targetWaypointIndex = (hunter.lastWaypointVisitedIndex + 1) % waypoints.Count; // Pasa al siguiente.
            Debug.Log($"<color=cyan>SECUENCIA:</color> Nuevo objetivo es el waypoint #{_targetWaypointIndex}.");
            currentTargetPosition = waypoints[_targetWaypointIndex].position; // Actualiza el objetivo para este frame.
        }
        else // 2. Comportamiento de Anulación (Dinámico): Si no hemos llegado, ¿hay una mejor opción?
        {
            int absoluteClosestIndex = FindClosestWaypointIndex(hunter);
            // Si el más cercano no es nuestro objetivo Y no es el que acabamos de visitar...
            if (absoluteClosestIndex != _targetWaypointIndex && absoluteClosestIndex != hunter.lastWaypointVisitedIndex)
            {
                float distanceToClosest = Vector3.Distance(hunter.transform.position, waypoints[absoluteClosestIndex].position);
                // ...y es significativamente más cercano...
                if (distanceToClosest < hunter.distanceToTarget * hunter.dynamicRepathFactor)
                {
                    Debug.LogWarning($"<color=orange>ANULACIÓN DINÁMICA:</color> Waypoint #{absoluteClosestIndex} es mucho más cercano. Cambiando de objetivo.");
                    _targetWaypointIndex = absoluteClosestIndex; // ...cambia de objetivo.
                    currentTargetPosition = waypoints[_targetWaypointIndex].position;
                }
            }
        }

        // 3. Acción: Calcula la fuerza para llegar al objetivo y la aplica.
        hunter.ApplyForce(Arrive(currentTargetPosition, hunter));

        // Dibuja las líneas de depuración.
        Debug.DrawLine(hunter.transform.position, currentTargetPosition, Color.yellow);
        DebugHelper.DrawCircle(currentTargetPosition, hunter.waypointArrivalDistance, Color.cyan);

        // --- GESTIÓN DE ENERGÍA ---
        hunter.energy -= 2 * Time.deltaTime; // Gasta energía lentamente.
        if (hunter.energy <= 0)
        {
            hunter.ChangeState(new IdleState()); // Si se agota, descansa.
        }
    }

    // Se ejecuta al salir del estado de patrulla.
    public override void Exit(Agent agent)
    {
        Hunter hunter = (Hunter)agent;
        hunter.distanceToTarget = 0; // Resetea la distancia.
        hunter.lastWaypointVisitedIndex = -1; // Resetea la memoria.
    }

    // --- MÉTODOS DE AYUDA ---
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

    private Vector3 Arrive(Vector3 target, Agent agent)
    {
        Vector3 desired = target - agent.transform.position;
        float distance = desired.magnitude;
        float slowingRadius = 15f; // Un radio de frenado más grande para el cazador.
        if (distance < slowingRadius)
        {
            desired = desired.normalized * agent.maxSpeed * (distance / slowingRadius);
        }
        else
        {
            desired = desired.normalized * agent.maxSpeed;
        }
        return desired - agent.velocity;
    }
}