using UnityEngine;

public class Hunter : Agent
{
    [Header("Parámetros de la FSM")]
    public float energy = 100f;
    public float maxEnergy = 100f;

    [Header("Parámetros de Detección y Patrulla")]
    public float sightRadius = 25f;
    public float attackRadius = 8f;

    [Tooltip("Distancia a la que se considera que el cazador ha llegado a un waypoint.")]
    public float waypointArrivalDistance = 2.0f;

    [Range(0.5f, 1f)]
    [Tooltip("Factor de anulación. Si otro waypoint es (distancia_actual * este_factor) más cercano, cambiará de objetivo. 1.0 = nunca cambia. 0.8 = cambia si hay uno un 20% más cerca.")]
    public float dynamicRepathFactor = 0.8f;


    [Header("Debug Info (Read-Only)")]
    public HunterState currentHunterState;

    [Tooltip("Distancia actual al objetivo (waypoint o boid).")]
    public float distanceToTarget;

    // --- NUEVA VARIABLE PÚBLICA DE "MEMORIA" ---
    [Tooltip("El índice del último waypoint que el cazador cruzó.")]
    public int lastWaypointVisitedIndex = -1;

    [Tooltip("Cuánto falta para entrar en rango de ataque. Negativo si ya está dentro.")]
    public float distanceToAttackRange;

    private FSMState currentState;

    // ... (el resto del script se mantiene exactamente igual) ...
    private void OnEnable()
    {
        if (EntityManager.Instance != null)
        {
            EntityManager.Instance.RegisterHunter(this);
        }
        ChangeState(new PatrolState());
    }

    private void OnDisable()
    {
        if (EntityManager.Instance != null)
        {
            EntityManager.Instance.UnregisterHunter(this);
        }
    }

    protected override void Update()
    {
        if (currentState != null)
        {
            currentState.Execute(this);
        }
        base.Update();
    }

    public void ChangeState(FSMState newState)
    {
        if (currentState != null)
        {
            currentState.Exit(this);
        }
        currentState = newState;
        if (currentState != null)
        {
            currentState.Enter(this);
        }
    }

    public void SetDebugInfo(Color color, string status)
    {
        SetDebugColor(color);
        debugStatusText = status;
    }

    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();
        DebugHelper.DrawCircle(transform.position, sightRadius, Color.yellow);
        DebugHelper.DrawCircle(transform.position, attackRadius, Color.red);
    }
}

public enum HunterState
{
    Patrolling,
    Hunting,
    Attacking,
    Resting
}