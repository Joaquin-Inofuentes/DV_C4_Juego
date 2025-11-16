using UnityEngine;
using System.Collections.Generic;

public class IA_P2_ST_PatrolState : IA_P2_INT_gentState
{
    private int _currentWaypoint = 0;

    public IA_P2_ST_PatrolState() { }

    public void Enter(IA_P2_AgentIA agent)
    {
        agent.AsignarColor(Color.blue);
        _currentWaypoint = 0;

        if (agent.patrolWaypoints != null && agent.patrolWaypoints.Count > 0)
            agent.GoTo(agent.patrolWaypoints[_currentWaypoint].position);

        // Dibuja todos los waypoints conectados al inicio
        DrawAllWaypoints(agent);
    }
    private Vector3 Registrada;
    public void Execute(IA_P2_AgentIA agent)
    {
        if (agent.patrolWaypoints == null || agent.patrolWaypoints.Count == 0) return;

        Vector3 target = agent.patrolWaypoints[_currentWaypoint].position;

        // Solo pedir ir si cambió el objetivo
        if (Registrada != target && !agent.isMoving)
        {
            agent.GoTo(target);
            Registrada = target;
        }

        // Avanza al siguiente waypoint si está suficientemente cerca
        float distanciaParaCambiar = 0.1f; // <-- X distancia
        if (Vector3.Distance(agent.transform.position, target) <= distanciaParaCambiar)
        {
            _currentWaypoint = (_currentWaypoint + 1) % agent.patrolWaypoints.Count; // vuelve a 0 si se acabaron
        }

        // Dibuja línea hacia waypoint actual
        Debug.DrawLine(agent.transform.position, target, Color.yellow);

        // Dibuja todos los waypoints conectados
        DrawAllWaypoints(agent);
    }


    public void Exit(IA_P2_AgentIA agent)
    {
        agent.StopAgent();
    }

    private void DrawAllWaypoints(IA_P2_AgentIA agent)
    {
        var wps = agent.patrolWaypoints;
        if (wps == null || wps.Count < 2) return;

        for (int i = 0; i < wps.Count; i++)
        {
            Vector3 start = wps[i].position;
            Vector3 end = wps[(i + 1) % wps.Count].position; // conecta último con primero
            Debug.DrawLine(start, end, Color.cyan);
        }
    }
}
