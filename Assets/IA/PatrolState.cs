using UnityEngine;
using System.Linq;

public class PatrolState : FSMState
{
    // Almacena el índice del waypoint que el cazador está persiguiendo actualmente.
    private int _targetWaypointIndex = 0;

    // --- NUEVO: Temporizador para la cadencia de fuego oportunista ---
    // Cada estado que puede disparar necesita su propio temporizador.
    private float _shootTimer;

    // Se ejecuta al entrar en el estado de patrulla.
    public override void Enter(Agent agent)
    {
        Hunter hunter = (Hunter)agent;
        hunter.SetDebugInfo(Color.yellow, "WayPoints");
        hunter.currentHunterState = HunterState.Patrolling;
        hunter.lastWaypointVisitedIndex = -1;
        _targetWaypointIndex = FindClosestWaypointIndex(hunter);
        // Inicializa el temporizador de disparo para que pueda disparar inmediatamente.
        _shootTimer = 0f;
    }

    // Se ejecuta en cada frame mientras está patrullando.
    public override void Execute(Agent agent)
    {
        Hunter hunter = (Hunter)agent;
        var waypoints = EntityManager.Instance.patrolWaypoints;

        // El temporizador de disparo siempre cuenta hacia abajo.
        _shootTimer -= Time.deltaTime;

        // --- LÓGICA DE DECISIÓN REESTRUCTURADA ---

        // 1. Buscar el boid más cercano, sin importar la distancia.
        FindClosestBoid(hunter, out Boid closestBoid, out float distanceToBoid);

        // 2. Decidir qué hacer basándose en ese boid.
        if (closestBoid != null)
        {
            // --- CASO A: ¡OBJETIVO DE OPORTUNIDAD! ---
            // Si el boid está dentro del radio de ATAQUE...
            if (distanceToBoid < hunter.attackRadius)
            {
                // ...ataca inmediatamente SIN cambiar de estado.
                hunter.SetDebugInfo(Color.cyan, "Ataque Oportunista"); // Un color especial para este estado

                // Si el temporizador de disparo está listo...
                if (_shootTimer <= 0f)
                {
                    // ...dispara.
                    hunter.Shoot(closestBoid);
                    // ...y reinicia el temporizador.
                    _shootTimer = 1f / hunter.fireRate;
                }
                // NOTA: El cazador seguirá moviéndose hacia su waypoint mientras dispara.
            }
            // --- CASO B: OBJETIVO LEJANO ---
            // Si el boid está dentro del radio de VISIÓN (pero no de ataque)...
            else if (distanceToBoid < hunter.sightRadius)
            {
                // ...debe comprometerse a una caza. CAMBIA DE ESTADO.
                hunter.ChangeState(new HuntingState(closestBoid));
                return; // Termina la ejecución de este estado.
            }
        }

        // 3. Si no hay boids relevantes, continuar con la patrulla normal.
        if (waypoints.Count > 0)
        {
            Vector3 currentTargetPosition = waypoints[_targetWaypointIndex].position;
            hunter.distanceToTarget = Vector3.Distance(hunter.transform.position, currentTargetPosition);

            if (hunter.distanceToTarget < hunter.waypointArrivalDistance)
            {
                hunter.lastWaypointVisitedIndex = _targetWaypointIndex;
                _targetWaypointIndex = (hunter.lastWaypointVisitedIndex + 1) % waypoints.Count;
                currentTargetPosition = waypoints[_targetWaypointIndex].position;
            }
            else
            {
                int absoluteClosestIndex = FindClosestWaypointIndex(hunter);
                if (absoluteClosestIndex != _targetWaypointIndex && absoluteClosestIndex != hunter.lastWaypointVisitedIndex)
                {
                    float distanceToClosest = Vector3.Distance(hunter.transform.position, waypoints[absoluteClosestIndex].position);
                    if (distanceToClosest < hunter.distanceToTarget * hunter.dynamicRepathFactor)
                    {
                        _targetWaypointIndex = absoluteClosestIndex;
                        currentTargetPosition = waypoints[_targetWaypointIndex].position;
                    }
                }
            }

            hunter.ApplyForce(Arrive(currentTargetPosition, hunter));
            Debug.DrawLine(hunter.transform.position, currentTargetPosition, Color.yellow);
            DebugHelper.DrawCircle(currentTargetPosition, hunter.waypointArrivalDistance, Color.cyan);
        }

        // --- GESTIÓN DE ENERGÍA ---
        hunter.energy -= 2 * Time.deltaTime;
        if (hunter.energy <= 0)
        {
            hunter.ChangeState(new IdleState());
        }
    }

    // Se ejecuta al salir del estado de patrulla.
    public override void Exit(Agent agent)
    {
        Hunter hunter = (Hunter)agent;
        hunter.distanceToTarget = 0;
        hunter.lastWaypointVisitedIndex = -1;
    }

    // --- MÉTODOS DE AYUDA (FindClosestBoid modificado) ---
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

    // --- MÉTODO MODIFICADO para devolver también la distancia ---
    private bool FindClosestBoid(Hunter hunter, out Boid foundBoid, out float distance)
    {
        foundBoid = null;
        distance = float.MaxValue;
        float closestDistSqr = float.MaxValue;

        foreach (var boid in EntityManager.Instance.boids)
        {
            if (boid == null || !boid.gameObject.activeInHierarchy) continue;

            float distSqr = (hunter.transform.position - boid.transform.position).sqrMagnitude;
            if (distSqr < closestDistSqr)
            {
                closestDistSqr = distSqr;
                foundBoid = boid;
            }
        }

        if (foundBoid != null)
        {
            distance = Mathf.Sqrt(closestDistSqr);
            return true;
        }
        return false;
    }

    private Vector3 Arrive(Vector3 target, Agent agent)
    {
        Vector3 desired = target - agent.transform.position;
        float distance = desired.magnitude;
        float slowingRadius = 15f;
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