using UnityEngine;

public class HuntingState : FSMState
{
    public override void Enter(Agent agent)
    {
        Debug.Log("Cazador: Entrando en estado de CAZA.");
    }

    public override void Execute(Agent agent)
    {
        Debug.Log("Cazador: Ejecutando lógica de CAZA (persiguiendo un boid).");

        // Lógica de transición:
        // if (energy <= 0) { agent.ChangeState(new IdleState()); }
        // if (boidLost) { agent.ChangeState(new PatrolState()); }
    }

    public override void Exit(Agent agent)
    {
        Debug.Log("Cazador: Saliendo del estado de CAZA.");
    }
}