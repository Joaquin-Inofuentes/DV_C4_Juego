// --- IA_P2_ST_PatrolState.cs ---
// (Modificado para usar el Contexto)

using UnityEngine;
using System.Collections.Generic;

public class IA_P2_ST_PatrolState : IA_P2_INT_gentState
{
    private int _currentWaypoint = 0;
    private Vector3 _registrada; // Almacena el último destino solicitado

    public IA_P2_ST_PatrolState() { }

    // Recibe el contexto (MoveAgent)
    public void Enter(IA_P2_MoveAgent context)
    {
        context.agent.AsignarColor(Color.blue);
        _currentWaypoint = 0;
        _registrada = Vector3.positiveInfinity; // Resetea el destino

        // Accede a los waypoints desde el contexto
        if (context.patrolWaypoints != null && context.patrolWaypoints.Count > 0)
        {
            // Le dice al agente (la herramienta) que vaya al punto
            Vector3 targetPos = context.patrolWaypoints[_currentWaypoint].position;
            context.agent.GoTo(targetPos);
            _registrada = targetPos; // Registra el primer destino
        }

        DrawAllWaypoints(context);
    }

    // Recibe el contexto (MoveAgent)
    public void Execute(IA_P2_MoveAgent context)
    {
        // Saca las variables del contexto para legibilidad
        IA_P2_AgentIA agent = context.agent;
        List<Transform> wps = context.patrolWaypoints;

        if (wps == null || wps.Count == 0) return;

        Vector3 target = wps[_currentWaypoint].position;

        // Dibuja línea hacia waypoint actual
        Debug.DrawLine(agent.transform.position, target, Color.yellow);

        // Dibuja todos los waypoints conectados
        DrawAllWaypoints(context);

        // Si el agente NO se está moviendo (porque ya llegó), d
        // dale el siguiente punto.
        float distanciaParaCambiar = 0.1f; // <-- Se puede ajustar

        // Comprueba si ha llegado al destino actual
        if (!agent.isMoving || Vector3.Distance(agent.transform.position, target) <= distanciaParaCambiar)
        {
            // Avanza al siguiente waypoint
            _currentWaypoint = (_currentWaypoint + 1) % wps.Count; // vuelve a 0 si se acabaron
            Vector3 newTarget = wps[_currentWaypoint].position;

            // Solo pide ir si cambió el objetivo
            if (_registrada != newTarget)
            {
                agent.GoTo(newTarget);
                _registrada = newTarget;
            }
        }
    }

    // Recibe el contexto (MoveAgent)
    public void Exit(IA_P2_MoveAgent context)
    {
        // Usa el agente del contexto
        context.agent.StopAgent();
    }

    private void DrawAllWaypoints(IA_P2_MoveAgent context)
    {
        // Usa los waypoints del contexto
        var wps = context.patrolWaypoints;
        if (wps == null || wps.Count < 2) return;

        for (int i = 0; i < wps.Count; i++)
        {
            Vector3 start = wps[i].position;
            Vector3 end = wps[(i + 1) % wps.Count].position; // conecta último con primero
            Debug.DrawLine(start, end, Color.cyan);
        }
    }
}