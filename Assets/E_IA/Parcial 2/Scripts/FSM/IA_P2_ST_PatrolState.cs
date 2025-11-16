using UnityEngine;
using System.Collections.Generic;

public class IA_P2_ST_PatrolState : IA_P2_INT_gentState
{
    private int _currentWaypoint = 0;

    public IA_P2_ST_PatrolState() { }

    public void Enter(IA_P2_AgentIA agent)
    {
        _currentWaypoint = 0;

        if (agent.patrolWaypoints != null && agent.patrolWaypoints.Count > 0)
            agent.GoTo(agent.patrolWaypoints[_currentWaypoint].position);

        // Dibuja todos los waypoints conectados al inicio
        DrawAllWaypoints(agent);
    }
    private int IndiceActual;
    public void Execute(IA_P2_AgentIA agent)
    {
        if (agent.patrolWaypoints == null || agent.patrolWaypoints.Count == 0) return;
        _currentWaypoint = agent.currentIndex;
        Vector3 target = agent.patrolWaypoints[_currentWaypoint].position;
        Debug.Log("Se intento pedir q vaya");
        if (IndiceActual == _currentWaypoint && agent.isMoving) return;
        agent.GoTo(target);

        if (!agent.IsMoving())
            _currentWaypoint = (_currentWaypoint + 1) % agent.patrolWaypoints.Count;

        // Dibuja línea hacia waypoint actual
        Debug.DrawLine(agent.transform.position, target, Color.yellow);

        // Opcional: siempre dibujar todos los waypoints conectados cada frame
        DrawAllWaypoints(agent);
        IndiceActual = _currentWaypoint;
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
