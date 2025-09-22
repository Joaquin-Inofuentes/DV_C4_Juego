using UnityEngine;

public class PatrolState : FSMState
{
    public override void Enter(Agent agent)
    {
        Debug.Log("Cazador: Entrando en estado de PATRULLA.");
    }

    public override void Execute(Agent agent)
    {
        Debug.Log("Cazador: Ejecutando lógica de PATRULLA (moviéndose entre waypoints, buscando boids).");

        // Lógica de transición:
        // if (boidInSight) { agent.ChangeState(new HuntingState()); }
        // if (energy <= 0) { agent.ChangeState(new IdleState()); }
    }

    public override void Exit(Agent agent)
    {
        Debug.Log("Cazador: Saliendo del estado de PATRULLA.");
    }
}