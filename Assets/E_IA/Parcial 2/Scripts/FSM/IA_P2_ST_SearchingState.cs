using UnityEngine;

public class IA_P2_ST_SearchingState : IA_P2_INT_gentState
{
    private float _searchTimer = 3f;
    private float _blinkSpeed = 6f;
    private float _originalRotationSpeed;
    private float _searchRotationSpeed = 3f;

    public void Enter(IA_P2_FSM context)
    {
        //Debug.Log("Se entro en modo SEARCHING");
        context.agent.AsignarColor(Color.yellow);

        _originalRotationSpeed = context.agent.rotationSpeed;

        // Ir a la última posición conocida
        context.agent.GoTo(context.lastKnownPosition);

        _searchTimer = 3f;
    }

    public void Execute(IA_P2_FSM context)
    {
        // Mira al objetivo
        context.agent.LookAtTarget(context.target.transform.position);

        if(Vector3.Distance(context.lastKnownPosition, context.agent.transform.position) > 3f)
        {
            // Va a la ultima a completar su mision de llegar
            context.TransitionTo(AgentState.Chasing);
        }

        // Si, lo puedo ver va hacia el
        if (IA_P2_LineOfSight3D.Check(context.agent.gameObject.transform.position, context.target.transform.position, context.NotificacionDeEnemigoVisible.visionObstacles))
        {
            context.TransitionTo(AgentState.Chasing);
            Debug.DrawLine(context.agent.transform.position, context.target.transform.position, Color.red, 2f);
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
