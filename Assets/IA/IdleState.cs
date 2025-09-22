using UnityEngine;

public class IdleState : FSMState
{
    // Se ejecuta al entrar en el estado de descanso.
    public override void Enter(Agent agent)
    {
        Hunter hunter = (Hunter)agent;
        hunter.SetDebugInfo(new Color(0.2f, 0.2f, 0.4f), "Descansar");
        hunter.currentHunterState = HunterState.Resting;
        // Detiene cualquier movimiento residual.
        hunter.velocity = Vector3.zero;
    }

    // Se ejecuta en cada frame mientras está descansando.
    public override void Execute(Agent agent)
    {
        Hunter hunter = (Hunter)agent;
        // Recupera energía con el tiempo.
        hunter.energy += 20 * Time.deltaTime;

        // Si la energía está al máximo...
        if (hunter.energy >= hunter.maxEnergy)
        {
            hunter.energy = hunter.maxEnergy; // Asegura que no exceda el máximo.
            hunter.ChangeState(new PatrolState()); // ...vuelve a patrullar.
        }
    }

    // Se ejecuta al salir del estado de descanso.
    public override void Exit(Agent agent)
    {
        // No necesita hacer nada especial al salir.
    }
}