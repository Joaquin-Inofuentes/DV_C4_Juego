using System.Net.Sockets;
using UnityEngine;

public class IA_P2_ST_ChaseState : IA_P2_INT_gentState
{

    public void Enter(IA_P2_FSM context)
    {
        // Si entramos aquí por error sin target, abortamos y volvemos a patrulla
        if (context.target == null)
        {
            Debug.LogWarning("Se intentó entrar en Chase sin un Target. Volviendo a patrulla.");
            context.TransitionTo(AgentState.Patrolling);
            return;
        }
        Debug.Log("Se llamo a perseguir");
        context.agent.AsignarColor(Color.red);

        if (context.target != null)
        {
            context.lastKnownPosition = context.target.transform.position;
            context.agent.GoTo(context.target.transform.position, context.agent.DistanceStop);
        }
        else
        {
            Debug.Log("Target es null. Revisar");
        }
        IA_P2_BusEvent_Manager.NotificarEncontrado(context.target);
        context.agent.SetSpeed(5);
    }

    public void Execute(IA_P2_FSM context)
    {
        //Debug.Log("Ejecutando Chase State");

        // --- Lógica de Persecución (si SÍ vemos al jugador) ---

        if (context.target == null) return; // Doble chequeo por si acaso

        Vector3 targetPosition = context.target.transform.position;
        Debug.DrawLine(context.agent.transform.position, targetPosition, Color.red);

        // Si estamos en el ultimo tramo hacia la posición del jugador
        if (context.agent.IsOnFinalPathSegment())
        {
            // Miramos hacia el objetivo
            context.agent.LookAtTarget(context.target.transform.position);
            // Si es visible actualizamos el destino
            if (IA_P2_LineOfSight3D.Check(
                context.agent.transform.position, 
                targetPosition, 
                context.NotificacionDeEnemigoVisible.visionObstacles))
            {
                context.lastKnownPosition = context.target.transform.position;
                context.agent.GoTo(targetPosition, context.agent.DistanceStop);
                Debug.DrawLine(targetPosition, context.agent.transform.position, Color.blue);
                return;
            }
            else
            {
                //Debug.DrawLine(targetPosition, context.agent.transform.position, Color.black,2);
                // Perdimos de vista al jugador
                context.LoPerdiDeVision(context.target);
            }
        }
    }

    public void Exit(IA_P2_FSM context)
    {
        context.agent.StopAgent();
    }
}