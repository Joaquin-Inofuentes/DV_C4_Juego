// --- Guarda este archivo como IA_P2_ST_SearchingState.cs ---

using UnityEngine;

public class IA_P2_ST_SearchingState : IA_P2_INT_gentState
{
    private float _searchTimer; // Tiempo para "mirar alrededor"

    // [NUEVO] Variable para la velocidad de parpadeo
    private float _blinkSpeed = 6f; // 6 veces por segundo (3 rojas, 3 amarillas)

    // [NUEVO] Variables para guardar la configuración original del agente
    private float _originalRotationSpeed;
    private float _searchRotationSpeed = 3f; // Una velocidad de "escaneo" más lenta y deliberada

    public void Enter(IA_P2_FSM context)
    {
        //Debug.Log("Searching: Entrando a 'Buscar' en: " + context.lastKnownPosition);
        context.agent.AsignarColor(Color.yellow);

        // Guardamos la velocidad de rotación original
        _originalRotationSpeed = context.agent.rotationSpeed;

        // Ir a la última posición conocida
        context.agent.GoTo(context.lastKnownPosition);

        _searchTimer = 3f; // Tiempo que se quedará "buscando" al llegar
    }

    public void Execute(IA_P2_FSM context)
    {
        // 1. ¿Vemos al jugador mientras buscamos? (Máxima prioridad)
        if (context.IsPlayerVisible())
        {
            context.TransitionTo(AgentState.Chasing);
            return;
        }

        // 2. ¿Llegamos al punto de búsqueda?
        if (!context.agent.isMoving)
        {
            // --- EL AGENTE LLEGÓ AL PUNTO ---

            // [NUEVO] Tarea 1: Parpadear colores
            if (Mathf.PingPong(Time.time * _blinkSpeed, 1f) < 0.5f)
            {
                context.agent.AsignarColor(Color.yellow);
            }
            else
            {
                context.agent.AsignarColor(Color.red);
            }

            // [NUEVO] Tarea 2: Mirar (rotar) hacia el objetivo
            // (Asumimos que el agente "sabe" dónde está el jugador pero no lo "ve")
            if (context.target != null)
            {
                // Ralentizamos la rotación para que parezca que "escanea"
                context.agent.rotationSpeed = _searchRotationSpeed;

                Vector3 dirToTarget = context.target.transform.position - context.agent.transform.position;
                dirToTarget.y = 0; // Rotación horizontal

                if (dirToTarget.sqrMagnitude > 0.001f)
                {
                    Quaternion targetRotation = Quaternion.LookRotation(dirToTarget.normalized);
                    context.agent.transform.rotation = Quaternion.Slerp(
                        context.agent.transform.rotation,
                        targetRotation,
                        Time.deltaTime * context.agent.rotationSpeed // Usamos la velocidad (lenta)
                    );
                }
            }

            // [EXISTENTE] Tarea 3: Contar tiempo para rendirse
            _searchTimer -= Time.deltaTime;
            if (_searchTimer <= 0f)
            {
                //Debug.Log("Searching: No se encontró al jugador. Volviendo a patrulla.");
                context.TransitionTo(AgentState.ReturningToPatrol);
            }
        }
        else
        {
            // --- EL AGENTE AÚN ESTÁ YENDO AL PUNTO ---

            // Mantenemos la velocidad de rotación normal mientras se mueve
            context.agent.rotationSpeed = _originalRotationSpeed;
            // Mantenemos el color amarillo fijo (sin parpadear)
            context.agent.AsignarColor(Color.yellow);
        }
    }

    public void Exit(IA_P2_FSM context)
    {
        // [NUEVO] Restaurar la velocidad de rotación original
        context.agent.rotationSpeed = _originalRotationSpeed;

        // El agente se detiene (o su próximo estado tomará el control)
        context.agent.StopAgent();
    }
}