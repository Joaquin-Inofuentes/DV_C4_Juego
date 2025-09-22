using UnityEngine;

public class HuntingState : FSMState
{
    // Almacena una referencia al boid que se está persiguiendo.
    private Boid _targetBoid;
    // Temporizador para que el cazador "se aburra" y no persiga indefinidamente.
    private float _huntingTimer;
    private const float MAX_HUNTING_TIME = 8.0f; // Puede perseguir durante 8 segundos.

    // Constructor: se le debe pasar el boid objetivo al crear el estado.
    public HuntingState(Boid target)
    {
        _targetBoid = target;
    }

    // Se ejecuta al entrar en el estado de caza.
    public override void Enter(Agent agent)
    {
        _huntingTimer = 0f; // Reinicia el temporizador.
    }

    // Se ejecuta en cada frame mientras está cazando.
    public override void Execute(Agent agent)
    {
        Hunter hunter = (Hunter)agent;

        // --- PRIORIDAD 1: VALIDACIÓN DEL OBJETIVO ---
        // Si el objetivo ya no existe o está inactivo, vuelve a patrullar.
        if (_targetBoid == null || !_targetBoid.gameObject.activeInHierarchy)
        {
            hunter.ChangeState(new PatrolState());
            return;
        }

        // Incrementa el temporizador de caza.
        _huntingTimer += Time.deltaTime;
        float distanceToTarget = Vector3.Distance(hunter.transform.position, _targetBoid.transform.position);

        // Actualiza las variables de depuración en el Inspector.
        hunter.distanceToTarget = distanceToTarget;
        hunter.distanceToAttackRange = distanceToTarget - hunter.attackRadius;

        // --- LÓGICA DE MOVIMIENTO Y ESTADO ---
        // Si está dentro del radio de ataque...
        if (distanceToTarget < hunter.attackRadius)
        {
            hunter.SetDebugInfo(Color.red, "Atacando");
            hunter.currentHunterState = HunterState.Attacking;
        }
        else // Si no, sigue cazando.
        {
            hunter.SetDebugInfo(Color.magenta, "Cazando");
            hunter.currentHunterState = HunterState.Hunting;
        }

        // Aplica la fuerza de "Pursuit" para predecir y perseguir al boid.
        hunter.ApplyForce(Pursuit(_targetBoid, hunter));
        Debug.DrawLine(hunter.transform.position, _targetBoid.transform.position, hunter.currentHunterState == HunterState.Attacking ? Color.red : Color.magenta);

        // --- LÓGICA DE TRANSICIÓN ---
        // Si se acaba el tiempo o pierde de vista al boid, vuelve a patrullar.
        if (_huntingTimer > MAX_HUNTING_TIME || distanceToTarget > hunter.sightRadius)
        {
            hunter.ChangeState(new PatrolState());
            return;
        }

        // Gasta energía rápidamente.
        hunter.energy -= 10 * Time.deltaTime;
        if (hunter.energy <= 0)
        {
            hunter.ChangeState(new IdleState()); // Si se agota, descansa.
        }
    }

    // Se ejecuta al salir del estado de caza.
    public override void Exit(Agent agent)
    {
        Hunter hunter = (Hunter)agent;
        // Resetea las variables de depuración.
        hunter.distanceToTarget = 0;
        hunter.distanceToAttackRange = 0;
    }

    // --- MÉTODO DE AYUDA: PURSUIT ---
    private Vector3 Pursuit(Agent target, Agent agent)
    {
        // Calcula cuánto tiempo tardará en llegar al objetivo.
        float distance = Vector3.Distance(agent.transform.position, target.transform.position);
        float timeToTarget = distance / agent.maxSpeed;

        // Predice la posición futura del objetivo basándose en su velocidad actual.
        Vector3 futurePosition = target.transform.position + (target.velocity * timeToTarget);

        // Dibuja una esfera cian en la posición futura predicha.
        DebugHelper.DrawCircle(futurePosition, 1f, Color.cyan);

        // Usa "Seek" (una versión simple de Arrive) para ir a esa posición futura.
        Vector3 desired = (futurePosition - agent.transform.position).normalized * agent.maxSpeed;
        return desired - agent.velocity;
    }
}