using UnityEngine;

public class HuntingState : FSMState
{
    private Boid _targetBoid;
    private float _huntingTimer;
    private const float MAX_HUNTING_TIME = 5.0f;

    public HuntingState(Boid target) { _targetBoid = target; }

    public override void Enter(Agent agent) { _huntingTimer = 0f; }

    public override void Execute(Agent agent)
    {
        Hunter hunter = (Hunter)agent;

        if (_targetBoid == null || !_targetBoid.gameObject.activeInHierarchy)
        {
            hunter.ChangeState(new PatrolState());
            return;
        }

        _huntingTimer += Time.deltaTime;
        float distanceToTarget = Vector3.Distance(hunter.transform.position, _targetBoid.transform.position);

        hunter.distanceToTarget = distanceToTarget;
        hunter.distanceToAttackRange = distanceToTarget - hunter.attackRadius;

        if (distanceToTarget < hunter.attackRadius)
        {
            hunter.SetDebugInfo(Color.red, "Atacando");
            // --- LÍNEA QUE CAUSABA EL ERROR SI EL ENUM NO EXISTE ---
            hunter.currentHunterState = HunterState.Attacking;
            Debug.DrawLine(hunter.transform.position, _targetBoid.transform.position, Color.red);
        }
        else
        {
            hunter.SetDebugInfo(Color.magenta, "Cazando");
            // --- LÍNEA QUE CAUSABA EL ERROR SI EL ENUM NO EXISTE ---
            hunter.currentHunterState = HunterState.Hunting;
            Debug.DrawLine(hunter.transform.position, _targetBoid.transform.position, Color.magenta);
        }

        if (_huntingTimer > MAX_HUNTING_TIME || distanceToTarget > hunter.sightRadius)
        {
            hunter.ChangeState(new PatrolState());
            return;
        }

        hunter.energy -= 10 * Time.deltaTime;
        if (hunter.energy <= 0)
        {
            hunter.ChangeState(new IdleState());
        }
    }

    public override void Exit(Agent agent)
    {
        Hunter hunter = (Hunter)agent;
        hunter.distanceToTarget = 0;
        hunter.distanceToAttackRange = 0;
    }
}