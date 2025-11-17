using UnityEngine;

public class IA_P2_ST_ChaseState : IA_P2_INT_gentState
{

    public void Enter(IA_P2_FSM context)
    {
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
        IA_P2_BusEvent_Manager.NotificarEncontrado(context.target, context);
        context.agent.SetSpeed(5);
    }

    public void Execute(IA_P2_FSM context)
    {
        //Debug.Log("Ejecutando Chase State");

        // --- Lógica de Persecución (si SÍ vemos al jugador) ---

        if (context.target == null) return; // Doble chequeo por si acaso

        Vector3 targetPosition = context.target.transform.position;
        //Debug.DrawLine(context.agent.transform.position, targetPosition, Color.red);

        // Actualizamos la última posición conocida MIENTRAS perseguimos
        //context.lastKnownPosition = context.target.transform.position;
        context.lastKnownPosition = context.target.transform.position;
        context.agent.GoTo(targetPosition, context.agent.DistanceStop);
    }

    public void Exit(IA_P2_FSM context)
    {
        context.agent.StopAgent();
    }
}