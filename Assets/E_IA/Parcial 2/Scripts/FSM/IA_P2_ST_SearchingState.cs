// --- Guarda este archivo como IA_P2_ST_SearchingState.cs ---

using UnityEngine;

public class IA_P2_ST_SearchingState : IA_P2_INT_gentState
{
    private float _searchTimer; // Tiempo para "mirar alrededor"

    public void Enter(IA_P2_FSM context)
    {
        Debug.Log("Searching: Entrando a 'Buscar' en: " + context.lastKnownPosition);
        context.agent.AsignarColor(Color.yellow);

        // Ir a la última posición conocida
        context.agent.GoTo(context.lastKnownPosition);

        _searchTimer = 3f; // Tiempo que se quedará "buscando" al llegar
    }

    public void Execute(IA_P2_FSM context)
    {
        // 1. ¿Vemos al jugador mientras buscamos?
        if (context.IsPlayerVisible())
        {
            context.TransitionTo(AgentState.Chasing);
            return;
        }

        // 2. ¿Llegamos al punto de búsqueda?
        if (!context.agent.isMoving)
        {
            // El agente llegó al destino (lastKnownPosition)
            _searchTimer -= Time.deltaTime;

            // "Mirar alrededor" (puedes añadir una animación de espera aquí)

            if (_searchTimer <= 0f)
            {
                // Se acabó el tiempo de búsqueda, nos rendimos
                Debug.Log("Searching: No se encontró al jugador. Volviendo a patrulla.");
                context.TransitionTo(AgentState.ReturningToPatrol);
            }
        }
    }

    public void Exit(IA_P2_FSM context)
    {
        context.agent.StopAgent();
    }
}