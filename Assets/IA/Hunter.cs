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

    [Tooltip("Si el cazador está más lejos que esto de su waypoint objetivo, buscará uno nuevo más cercano.")]
    public float repathThresholdDistance = 30.0f;

    [Header("Debug Info (Read-Only)")]
    public HunterState currentHunterState;
    public float distanceToTarget;
    public float distanceToAttackRange;

    private FSMState currentState;

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