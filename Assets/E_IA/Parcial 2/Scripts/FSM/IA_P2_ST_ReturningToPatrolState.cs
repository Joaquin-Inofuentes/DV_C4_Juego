// --- Guarda este archivo como IA_P2_ST_ReturningToPatrolState.cs ---

using UnityEngine;

public class IA_P2_ST_ReturningToPatrolState : IA_P2_INT_gentState
{
    private Vector3 _patrolDestination;

    public void Enter(IA_P2_FSM context)
    {
        //Debug.Log("Returning: Volviendo a la patrulla.");
        context.agent.AsignarColor(Color.cyan); // Un color "tranquilo"

        // 1. Encontrar el waypoint de patrulla MÁS CERCANO
        var wps = context.patrolWaypoints;
        if (wps == null || wps.Count == 0)
        {
            // No hay patrulla a la que volver, pasar directo a Patrullar
            context.TransitionTo(AgentState.Patrolling);
            return;
        }

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

        // 2. Ir a ese punto
        _patrolDestination = wps[closestIndex].position;
        context.agent.GoTo(_patrolDestination);
        context.agent.SetSpeed(2.0f);
    }

    public void Execute(IA_P2_FSM context)
    {
        // 1. ¿Vemos al jugador mientras volvemos?
        if (context.IsPlayerVisible())
        {
            context.TransitionTo(AgentState.Chasing);
            return;
        }

        // 2. ¿Llegamos al punto de la patrulla?
        float arrivalDistance = 0.5f;

        // Comprobamos si no nos estamos moviendo Y estamos cerca del destino
        if (!context.agent.isMoving && Vector3.Distance(context.agent.transform.position, _patrolDestination) < arrivalDistance)
        {
            // Llegamos. Ahora podemos empezar a patrullar normalmente.
            Debug.Log("Returning: En posición. Reanudando patrulla.");
            context.TransitionTo(AgentState.Patrolling);
        }
    }

    public void Exit(IA_P2_FSM context)
    {
        // No necesita parar al agente, porque el estado Patrolling
        // tomará el control del movimiento.
        context.agent.SetSpeed(3.0f); // Sali de returnat patrullaje a velocidad normal
    }
}