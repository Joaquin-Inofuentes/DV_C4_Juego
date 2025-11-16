using UnityEngine;

public class IA_P2_ST_ChaseState : IA_P2_INT_gentState
{
    // --- Optimización ---
    // No queremos recalcular el pathfinding (GoTo) en cada frame.
    // Lo hacemos solo cada cierto intervalo de tiempo.
    private float _repathTimer;

    // Puedes ajustar este valor. 0.5s es un buen punto de partida.
    private const float REPATH_INTERVAL = 0.5f;

    // Almacenamos la última posición conocida del target
    private Vector3 _lastKnownPosition;

    /// <summary>
    /// Se llama al entrar en el estado de persecución.
    /// </summary>
    public void Enter(IA_P2_MoveAgent context)
    {
        context.agent.AsignarColor(Color.red);

        // Al entrar, fuerza un cálculo de ruta inmediato
        _repathTimer = 0f;

        if (context.target != null)
        {
            _lastKnownPosition = context.target.transform.position;
            context.agent.GoTo(_lastKnownPosition);
        }
        else
        {
            // Si no hay target, usa una posición inválida
            _lastKnownPosition = Vector3.positiveInfinity;
        }
    }

    /// <summary>
    /// Se llama en cada frame mientras el estado está activo.
    /// </summary>
    public void Execute(IA_P2_MoveAgent context)
    {
        // Si el objetivo desaparece, no hagas nada.
        // (El MoveAgent debería cambiar de estado pronto)
        if (context.target == null)
        {
            context.agent.StopAgent(); // Detiene al agente si el target desaparece
            return;
        }

        // Dibuja una línea hacia el objetivo
        Vector3 targetPosition = context.target.transform.position;
        Debug.DrawLine(context.agent.transform.position, targetPosition, Color.red);

        // --- Lógica de Optimización ---
        _repathTimer -= Time.deltaTime;

        // Si el timer se agotó O si el objetivo se movió mucho (opcional)
        // volvemos a calcular la ruta.
        if (_repathTimer <= 0f)
        {
            _repathTimer = REPATH_INTERVAL; // Reinicia el timer

            // Solo recalcula si la posición cambió
            if (_lastKnownPosition != targetPosition)
            {
                context.agent.GoTo(targetPosition);
                _lastKnownPosition = targetPosition;
            }
        }

        // Nota: El agente (IA_P2_AgentIA) se sigue moviendo en su propio Update()
        // hacia el último destino que le dimos con GoTo(). 
        // Esta función (Execute) solo se encarga de *actualizar* ese destino periódicamente.
    }

    /// <summary>
    /// Se llama al salir del estado.
    /// </summary>
    public void Exit(IA_P2_MoveAgent context)
    {
        // Detiene al agente cuando sale de la persecución
        context.agent.StopAgent();
    }
}