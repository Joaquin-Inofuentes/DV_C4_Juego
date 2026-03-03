using UnityEngine;

public class IA_P2_ST_ChaseState : IA_P2_INT_gentState
{
    private bool _goingToLastKnown;

    public void Enter(IA_P2_FSM context)
    {
        context.agent.AsignarColor(Color.red); // ROJO = Te estoy viendo o te estoy buscando donde te vi
        context.agent.SetSpeed(5f);
        _goingToLastKnown = false;
    }

    public void Execute(IA_P2_FSM context)
    {
        if (context.target == null)
        {
            context.TransitionTo(AgentState.ReturningToPatrol);
            return;
        }

        Vector3 targetPos = context.target.transform.position;
        // Check de visión real
        bool loVeo = IA_P2_LineOfSight3D.Check(context.agent.transform.position, targetPos, context.NotificacionDeEnemigoVisible.visionObstacles);

        if (loVeo)
        {
            // SI LO VEO: Persecución activa
            context.lastKnownPosition = targetPos;
            context.agent.GoTo(targetPos, context.agent.DistanceStop);
            _goingToLastKnown = false;
        }
        else
        {
            // SI NO LO VEO:
            if (!_goingToLastKnown)
            {
                // Solo enviamos la orden de ir al último punto UNA VEZ
                context.agent.GoTo(context.lastKnownPosition, 0f);
                _goingToLastKnown = true;
                Debug.Log("<color=red>Chase:</color> Te perdí. Yendo a tu última posición...");
            }

            // Chequeamos si hemos llegado físicamente a ese último punto
            float distancia = Vector3.Distance(context.agent.transform.position, context.lastKnownPosition);

            // Si estamos a menos de 0.8 metros o el agente se detuvo
            if (distancia < 0.8f || !context.agent.isMoving)
            {
                Debug.Log("<color=yellow>Chase:</color> ¡Llegué al último punto! Entrando en ALERTA.");
                context.TransitionTo(AgentState.Searching);
            }
        }
    }

    public void Exit(IA_P2_FSM context) { }
}