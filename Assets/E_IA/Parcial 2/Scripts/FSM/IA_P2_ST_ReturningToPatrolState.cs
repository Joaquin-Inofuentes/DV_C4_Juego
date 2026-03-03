// --- Guarda este archivo como IA_P2_ST_ReturningToPatrolState.cs ---

using UnityEngine;

public class IA_P2_ST_ReturningToPatrolState : IA_P2_INT_gentState
{
    private Vector3 _patrolDestination;

    // Retry
    private float _nextRetryTime;
    private const float RETRY_EVERY = 0.2f;
    private int _retryCount;
    private const int MAX_RETRIES = 1; // ajustá si querés infinito

    public void Enter(IA_P2_FSM context)
    {
        context.agent.AsignarColor(Color.cyan);

        var wps = context.patrolWaypoints;
        if (wps == null || wps.Count == 0)
        {
            context.TransitionTo(AgentState.Patrolling);
            return;
        }

        // 1) waypoint más cercano
        int closestIndex = 0;
        float minDistance = float.MaxValue;
        Vector3 agentPos = context.agent.transform.position;

        for (int i = 0; i < wps.Count; i++)
        {
            float distance = Vector3.Distance(agentPos, wps[i].position);
            if (distance < minDistance)
            {
                minDistance = distance;
                closestIndex = i;
            }
        }

        // 2) destino
        _patrolDestination = wps[closestIndex].position;

        // reset retry
        _retryCount = 0;
        _nextRetryTime = Time.time; // intento inmediato

        // primer intento
        TryGoToPatrol(context);
        context.agent.SetSpeed(2.0f);
    }

    public void Execute(IA_P2_FSM context)
    {
        if (context.IsPlayerVisible())
        {
            context.TransitionTo(AgentState.Chasing);
            return;
        }

        // Reintento cada 1 segundo si NO estamos avanzando hacia el destino
        if (Time.time >= _nextRetryTime)
        {
            // criterio simple: si no se mueve y todavía está lejos => reintentar
            float arrivalDistance = 0.5f;
            bool lejos = Vector3.Distance(context.agent.transform.position, _patrolDestination) > arrivalDistance;

            if (!context.agent.isMoving && lejos)
            {
                _retryCount++;
                if (_retryCount <= MAX_RETRIES)
                {
                    TryGoToPatrol(context);
                }
                else
                {
                    // si querés: fallback duro
                    Debug.LogWarning("ReturningToPatrol: demasiados retries, vuelvo a Patrolling igual.");
                    context.TransitionTo(AgentState.Patrolling);
                    return;
                }
            }

            _nextRetryTime = Time.time + RETRY_EVERY;
        }

        // Llegada normal
        float arrivalDist = 0.5f;
        if (!context.agent.isMoving && Vector3.Distance(context.agent.transform.position, _patrolDestination) < arrivalDist)
        {
            context.TransitionTo(AgentState.Patrolling);
        }
    }

    public void Exit(IA_P2_FSM context)
    {
        context.agent.SetSpeed(3.0f);
    }

    private void TryGoToPatrol(IA_P2_FSM context)
    {
        //Debug.Log($"ReturningToPatrol: intento #{_retryCount} -> {_patrolDestination}");
        context.agent.GoTo(_patrolDestination);
    }
}