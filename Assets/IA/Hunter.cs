using UnityEngine;

/// <summary>
/// Controla el comportamiento del agente Cazador.
/// Hereda de la clase base 'Agent' para el movimiento.
/// Utiliza una Máquina de Estados Finita (FSM) para gestionar sus acciones,
/// como patrullar, cazar, atacar y descansar.
/// </summary>
public class Hunter : Agent
{
    // -------------------------------------------------------------------
    // --- PARÁMETROS PÚBLICOS (Ajustables en el Inspector de Unity) ---
    // -------------------------------------------------------------------

    [Header("Parámetros de la FSM")]
    public float energy = 100f;
    public float maxEnergy = 100f;

    [Header("Parámetros de Detección y Ataque")]
    public float sightRadius = 25f;
    public float attackRadius = 15f;

    [Tooltip("El Prefab del proyectil que disparará el cazador.")]
    public GameObject projectilePrefab;
    [Tooltip("Velocidad a la que se mueven los proyectiles.")]
    public float projectileSpeed = 50f;
    [Tooltip("Disparos por segundo.")]
    public float fireRate = 1f;

    [Header("Parámetros de Patrulla")]
    public float waypointArrivalDistance = 2.0f;
    [Range(0.5f, 1f)]
    public float dynamicRepathFactor = 0.8f;

    [Header("Debug Info (Read-Only)")]
    public HunterState currentHunterState;
    public float distanceToTarget;
    public int lastWaypointVisitedIndex = -1;
    public float distanceToAttackRange;

    // -------------------------------------------------------------------
    // --- VARIABLES PRIVADAS ---
    // -------------------------------------------------------------------

    // Almacena una referencia al estado actual de la FSM.
    private FSMState currentState;

    // -------------------------------------------------------------------
    // --- MÉTODOS DE UNITY (Ciclo de Vida) ---
    // -------------------------------------------------------------------

    private void OnEnable()
    {
        if (EntityManager.Instance != null)
        {
            EntityManager.Instance.RegisterHunter(this);
        }
        // Al activarse, el estado inicial siempre será Patrullar.
        ChangeState(new PatrolState());
    }

    private void OnDisable()
    {
        if (EntityManager.Instance != null)
        {
            EntityManager.Instance.UnregisterHunter(this);
        }
    }

    protected override void Update()
    {
        // Si hay un estado activo, delega la lógica de comportamiento a ese estado.
        if (currentState != null)
        {
            currentState.Execute(this);
        }
        // Llama al Update de la clase base (Agent) para aplicar el movimiento físico.
        base.Update();
    }

    protected override void OnDrawGizmos()
    {
        // Llama al OnDrawGizmos de la clase base para dibujar la etiqueta de texto.
        base.OnDrawGizmos();
        // Dibuja los círculos de detección para depuración.
        //DebugHelper.DrawCircle(transform.position, sightRadius, Color.yellow);
        DebugHelper.DrawCircle(transform.position, attackRadius, Color.red);
    }

    // -------------------------------------------------------------------
    // --- MÉTODOS PÚBLICOS (Control de la FSM y Acciones) ---
    // -------------------------------------------------------------------

    /// <summary>
    /// Realiza la transición de un estado a otro de forma segura.
    /// </summary>
    public void ChangeState(FSMState newState)
    {
        // Ejecuta la lógica de salida del estado actual si existe.
        if (currentState != null)
        {
            currentState.Exit(this);
        }

        // Cambia al nuevo estado.
        currentState = newState;

        // Ejecuta la lógica de entrada del nuevo estado si existe.
        if (currentState != null)
        {
            currentState.Enter(this);
        }
    }

    /// <summary>
    /// Instancia y lanza un proyectil hacia la posición futura predicha de un objetivo.
    /// </summary>
    public void Shoot(Agent target)
    {
        if (projectilePrefab == null)
        {
            Debug.LogError("¡El cazador no tiene un Prefab de proyectil asignado!", this);
            return;
        }

        // Predice la posición futura del objetivo (lógica de Pursuit).
        float distance = Vector3.Distance(transform.position, target.transform.position);
        float timeToTarget = distance / projectileSpeed;
        Vector3 futurePosition = target.transform.position + (target.velocity * timeToTarget);

        // Calcula la dirección desde el cazador hasta esa posición futura.
        Vector3 direction = (futurePosition - transform.position).normalized;

        // Instancia el proyectil en la posición del cazador (un poco adelantado para no chocar consigo mismo).
        GameObject projectileGO = Instantiate(projectilePrefab, transform.position + direction * 2f, Quaternion.LookRotation(direction));

        // Lanza el proyectil usando su propio script.
        projectileGO.GetComponent<Projectile>().Launch(direction, projectileSpeed);
    }

    /// <summary>
    /// Un método de ayuda para que los estados puedan actualizar la información de depuración.
    /// </summary>
    public void SetDebugInfo(Color color, string status)
    {
        SetDebugColor(color);
        debugStatusText = status;
    }
}

// -------------------------------------------------------------------
// --- ENUMERACIÓN DE ESTADOS ---
// -------------------------------------------------------------------

/// <summary>
/// Define los posibles estados de alto nivel del Cazador para una fácil visualización en el Inspector.
/// </summary>
public enum HunterState
{
    Patrolling,
    Hunting,
    Attacking,
    Resting
}