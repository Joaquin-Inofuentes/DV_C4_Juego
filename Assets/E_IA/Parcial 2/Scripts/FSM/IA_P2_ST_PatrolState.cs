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
        if (context.patrolWaypoints.Count != 1)
            context.agent.SetSpeed(2.0f);
        else
            context.agent.SetSpeed(5);
        // (Sacamos el DrawAllWaypoints de aquí para que no se
        // ejecute solo una vez)
    }
    private float _lastGoToTime = 0f;
    private float _cooldown = 0.05f;
    private Vector3 _ultimoDestino;
    private float _distanciaMinima = 0.05f;
    public void Execute(IA_P2_FSM context)
    {
        if (context.patrolWaypoints.Count == 1)
        {
            Vector3 nuevoDestino = context.patrolWaypoints[0].position;

            bool pasoTiempo = Time.time >= _lastGoToTime + _cooldown;
            bool cambioDistancia = Vector3.Distance(_ultimoDestino, nuevoDestino) > _distanciaMinima;

            if (pasoTiempo && cambioDistancia)
            {
                _lastGoToTime = Time.time;
                _ultimoDestino = nuevoDestino;

                context.agent.GoTo(nuevoDestino);
            }

            return;
        }







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