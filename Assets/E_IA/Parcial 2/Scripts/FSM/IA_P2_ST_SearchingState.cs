using UnityEngine;

public class IA_P2_ST_SearchingState : IA_P2_INT_gentState
{
    private float _searchTimer = 3f;
    private float _blinkSpeed = 6f;
    private float _originalRotationSpeed;
    private float _searchRotationSpeed = 3f;

    public void Enter(IA_P2_FSM context)
    {
        Debug.Log("Se entro en modo SEARCHING");
        context.agent.AsignarColor(Color.yellow);

        _originalRotationSpeed = context.agent.rotationSpeed;

        // Ir a la última posición conocida
        context.agent.GoTo(context.lastKnownPosition);

        _searchTimer = 3f;
    }

    public void Execute(IA_P2_FSM context)
    {
        // 1. Si vemos al jugador → CHASING inmediatamente
        if (context.IsPlayerVisible())
        {
            context.agent.GoTo(context.target.transform.position);
            context.TransitionTo(AgentState.Chasing);
            return;
        }

        // 2. En Searching SIEMPRE avanzar hacia lastKnownPosition
        context.agent.GoTo(context.lastKnownPosition);

        // 3. LERP de color (alerta)
        float t = Mathf.PingPong(Time.time * _blinkSpeed, 1f);
        context.agent.AsignarColor(Color.Lerp(Color.yellow, Color.red, t));

        // 4. Siempre mirar hacia el objetivo (aunque no lo vea)
        if (context.target != null)
        {
            Vector3 dirToTarget = context.target.transform.position - context.agent.transform.position;
            dirToTarget.y = 0;

            if (dirToTarget.sqrMagnitude > 0.001f)
            {
                Quaternion targetRot = Quaternion.LookRotation(dirToTarget.normalized);
                context.agent.rotationSpeed = _searchRotationSpeed;

                context.agent.transform.rotation = Quaternion.Slerp(
                    context.agent.transform.rotation,
                    targetRot,
                    Time.deltaTime * context.agent.rotationSpeed
                );
            }
        }

        // 5. Timer → si pasan 3 segundos sin verlo, volver a patrulla
        _searchTimer -= Time.deltaTime;
        if (_searchTimer <= 0f)
        {
            context.TransitionTo(AgentState.ReturningToPatrol);
        }
    }


    public void Exit(IA_P2_FSM context)
    {
        context.agent.rotationSpeed = _originalRotationSpeed;
        context.agent.StopAgent();
    }
}
