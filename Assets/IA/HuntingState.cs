using UnityEngine;

public class HuntingState : FSMState
{
    // Almacena una referencia al boid que se está persiguiendo.
    private Boid _targetBoid;
    // Temporizador para que el cazador "se aburra" y no persiga indefinidamente.
    private float _huntingTimer;
    private const float MAX_HUNTING_TIME = 8.0f;

    // --- NUEVO: Temporizador para la cadencia de fuego ---
    // Esta variable contará el tiempo que falta para el siguiente disparo.
    private float _shootTimer;

    // Constructor: se le debe pasar el boid objetivo al crear el estado.
    public HuntingState(Boid target)
    {
        _targetBoid = target;
    }

    // Se ejecuta al entrar en el estado de caza.
    public override void Enter(Agent agent)
    {
        _huntingTimer = 0f; // Reinicia el temporizador de caza.
        // Inicializa el temporizador de disparo a 0 para que pueda disparar inmediatamente
        // la primera vez que entre en rango.
        _shootTimer = 0f;
    }

    // Se ejecuta en cada frame mientras está cazando.
    public override void Execute(Agent agent)
    {
        Hunter hunter = (Hunter)agent;

        // Si el objetivo ya no existe, vuelve a patrullar.
        if (_targetBoid == null || !_targetBoid.gameObject.activeInHierarchy)
        {
            hunter.ChangeState(new PatrolState());
            return;
        }

        // Incrementa el temporizador de caza.
        _huntingTimer += Time.deltaTime;
        // --- LÓGICA DEL TEMPORIZADOR ---
        // El temporizador de disparo cuenta hacia abajo en cada frame.
        _shootTimer -= Time.deltaTime;

        float distanceToTarget = Vector3.Distance(hunter.transform.position, _targetBoid.transform.position);
        hunter.distanceToTarget = distanceToTarget;
        hunter.distanceToAttackRange = distanceToTarget - hunter.attackRadius;

        // Si está dentro del radio de ataque...
        if (distanceToTarget < hunter.attackRadius)
        {
            hunter.SetDebugInfo(Color.red, "Atacando");
            hunter.currentHunterState = HunterState.Attacking;

            // --- CONDICIÓN DE DISPARO CON TEMPORIZADOR ---
            // Si el temporizador de disparo está listo (ha llegado a cero o menos)...
            if (_shootTimer <= 0f)
            {
                // ...dispara al objetivo.
                hunter.Shoot(_targetBoid);
                // ...y reinicia el temporizador según la cadencia de fuego definida en el Hunter.
                // Si fireRate es 1, el temporizador se reinicia a 1 segundo.
                _shootTimer = 1f / hunter.fireRate;
            }
        }
        else // Si no, sigue persiguiendo.
        {
            hunter.SetDebugInfo(Color.magenta, "Cazando");
            hunter.currentHunterState = HunterState.Hunting;
        }

        // Aplica la fuerza de "Pursuit" para seguir persiguiendo al boid.
        hunter.ApplyForce(Pursuit(_targetBoid, hunter));
        Debug.DrawLine(hunter.transform.position, _targetBoid.transform.position, hunter.currentHunterState == HunterState.Attacking ? Color.red : Color.magenta);

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
            hunter.ChangeState(new IdleState());
        }
    }

    // Se ejecuta al salir del estado de caza.
    public override void Exit(Agent agent)
    {
        Hunter hunter = (Hunter)agent;
        hunter.distanceToTarget = 0;
        hunter.distanceToAttackRange = 0;
    }

    // Método de ayuda para predecir la posición del objetivo.
    private Vector3 Pursuit(Agent target, Agent agent)
    {
        float distance = Vector3.Distance(agent.transform.position, target.transform.position);
        // Usa la velocidad del proyectil para una predicción más precisa.
        float timeToTarget = distance / ((Hunter)agent).projectileSpeed;

        Vector3 futurePosition = target.transform.position + (target.velocity * timeToTarget);

        DebugHelper.DrawCircle(futurePosition, 1f, Color.cyan);

        Vector3 desired = (futurePosition - agent.transform.position).normalized * agent.maxSpeed;
        return desired - agent.velocity;
    }
}