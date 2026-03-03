using UnityEngine;

public class IA_P2_ST_SearchingState : IA_P2_INT_gentState
{
    private float _alertTimer;
    private const float ALERT_TIME = 4f; // 4 segundos de alerta

    public void Enter(IA_P2_FSM context)
    {
        Debug.Log("<color=yellow>ALERTA:</color> Buscando en la zona...");
        context.agent.StopAgent(); // Se detiene por completo
        context.agent.AsignarColor(Color.yellow); // AMARILLO = Alerta
        _alertTimer = ALERT_TIME;
    }

    public void Execute(IA_P2_FSM context)
    {
        // 1. Si el enemigo aparece de nuevo, volvemos a Chase inmediatamente
        bool loVeo = IA_P2_LineOfSight3D.Check(context.agent.transform.position, context.target.transform.position, context.NotificacionDeEnemigoVisible.visionObstacles);
        if (loVeo)
        {
            context.TransitionTo(AgentState.Chasing);
            return;
        }

        // 2. Comportamiento de búsqueda: Girar en el sitio
        // Esto hace que el FOV escanee el área
        context.agent.transform.Rotate(Vector3.up, 120f * Time.deltaTime);

        // 3. Temporizador de la alerta
        _alertTimer -= Time.deltaTime;
        if (_alertTimer <= 0)
        {
            Debug.Log("<color=cyan>Alerta terminada:</color> No hay rastro. Volviendo a patrulla.");
            context.TransitionTo(AgentState.ReturningToPatrol);
        }
    }

    public void Exit(IA_P2_FSM context) { }
}