using UnityEngine;

public class IA_P2_ST_ChaseState : IA_P2_INT_gentState
{
    private float _repathTimer;
    private const float REPATH_INTERVAL = 0.5f;
    private Vector3 _lastKnownPosition;

    public void Enter(IA_P2_FSM context)
    {
        context.agent.AsignarColor(Color.red);
        _repathTimer = 0f;

        if (context.target != null)
        {
            _lastKnownPosition = context.target.transform.position;
            context.agent.GoTo(_lastKnownPosition);
        }
    }

    public void Execute(IA_P2_FSM context)
    {
        // [NUEVO] Comprobación de transición
        // ¿Perdimos al jugador?
        if (!context.IsPlayerVisible())
        {
            // Guardamos la última posición donde SÍ lo vimos
            context.lastKnownPosition = _lastKnownPosition;
            context.TransitionTo(AgentState.Searching);
            return;
        }

        // --- Lógica de Persecución (si SÍ vemos al jugador) ---

        if (context.target == null) return; // Doble chequeo por si acaso

        Vector3 targetPosition = context.target.transform.position;
        Debug.DrawLine(context.agent.transform.position, targetPosition, Color.red);

        _repathTimer -= Time.deltaTime;

        if (_repathTimer <= 0f)
        {
            _repathTimer = REPATH_INTERVAL;

            // Actualizamos la última posición conocida MIENTRAS perseguimos
            _lastKnownPosition = targetPosition;
            context.agent.GoTo(targetPosition);
        }
    }

    public void Exit(IA_P2_FSM context)
    {
        context.agent.StopAgent();
    }
}