using UnityEngine;

public class IdleState : FSMState
{
    public override void Enter(Agent agent)
    {
        Debug.Log("Cazador: Entrando en estado de DESCANSO (recuperando energía).");
    }

    public override void Execute(Agent agent)
    {
        Debug.Log("Cazador: Ejecutando lógica de DESCANSO (esperando).");

        // Lógica de transición:
        // if (energy >= maxEnergy) { agent.ChangeState(new PatrolState()); }
    }

    public override void Exit(Agent agent)
    {
        Debug.Log("Cazador: Saliendo del estado de DESCANSO.");
    }
}