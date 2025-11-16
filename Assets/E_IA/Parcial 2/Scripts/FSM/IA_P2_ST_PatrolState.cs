using UnityEngine;
using System.Collections.Generic;

public class IA_P2_ST_PatrolState : IA_P2_INT_gentState
{
    private int _currentWaypoint = 0;
    private Vector3 _registrada;

    public void Enter(IA_P2_FSM context)
    {
        context.agent.AsignarColor(Color.blue);
        var wps = context.patrolWaypoints;

        if (wps == null || wps.Count == 0) return;

        // 1. Encontrar el índice del waypoint más cercano
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
        _currentWaypoint = closestIndex;

        Vector3 targetPos = wps[_currentWaypoint].position;
        context.agent.GoTo(targetPos);
        _registrada = targetPos;

        context.agent.SetSpeed(3.0f);

        // (Sacamos el DrawAllWaypoints de aquí para que no se
        // ejecute solo una vez)
    }

    public void Execute(IA_P2_FSM context)
    {
        // [NUEVO] Comprobación de transición
        // ¿Vemos al jugador? Si es así, cambiamos a Chase.
        if (context.IsPlayerVisible())
        {
            context.TransitionTo(AgentState.Chasing);
            return; // Salimos del Execute
        }

        // --- Lógica de Patrulla (si no vemos al jugador) ---

        List<Transform> wps = context.patrolWaypoints;
        if (wps == null || wps.Count == 0) return;

        // Dibujos de Debug
        DrawAllWaypoints(context);
        Debug.DrawLine(context.agent.transform.position, _registrada, Color.yellow);


        if (!context.agent.isMoving)
        {
            float arrivalDistance = 0.5f;
            if (Vector3.Distance(context.agent.transform.position, _registrada) < arrivalDistance)
            {
                // Llegó. Calcular siguiente.
                _currentWaypoint = (_currentWaypoint + 1) % wps.Count;
                Vector3 newTarget = wps[_currentWaypoint].position;
                context.agent.GoTo(newTarget);
                _registrada = newTarget;
            }
            else
            {
                // Está parado, pero no donde debería. Reintentar.
                context.agent.GoTo(_registrada);
            }
        }
    }

    public void Exit(IA_P2_FSM context)
    {
        context.agent.StopAgent();
        context.agent.SetSpeed(5.0f);
    }

    // ... (El método DrawAllWaypoints no cambia)
    private void DrawAllWaypoints(IA_P2_FSM context)
    {
        var wps = context.patrolWaypoints;
        if (wps == null || wps.Count < 2) return;
        for (int i = 0; i < wps.Count; i++)
        {
            Vector3 start = wps[i].position;
            Vector3 end = wps[(i + 1) % wps.Count].position;
            Debug.DrawLine(start, end, Color.cyan);
        }
    }
}