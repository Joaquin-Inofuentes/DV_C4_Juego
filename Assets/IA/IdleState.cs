using UnityEngine;

public class IdleState : FSMState
{
    public override void Enter(Agent agent)
    {
        Hunter hunter = (Hunter)agent;
        hunter.SetDebugInfo(new Color(0.2f, 0.2f, 0.4f), "Descansar");
        // --- LÍNEA QUE CAUSABA EL ERROR SI EL ENUM NO EXISTE ---
        hunter.currentHunterState = HunterState.Resting;
    }

    public override void Execute(Agent agent)
    {
        Hunter hunter = (Hunter)agent;
        hunter.energy += 20 * Time.deltaTime;
        if (hunter.energy >= hunter.maxEnergy)
        {
            hunter.energy = hunter.maxEnergy;
            hunter.ChangeState(new PatrolState());
        }
    }

    public override void Exit(Agent agent) { }
}