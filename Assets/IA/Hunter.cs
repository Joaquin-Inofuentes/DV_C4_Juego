using UnityEngine;

public class Hunter : Agent
{
    [Header("Parámetros de la FSM")]
    public float energy = 100f;
    public float maxEnergy = 100f;

    [Header("Parámetros de Detección y Ataque")]
    public float sightRadius = 25f;
    public float attackRadius = 15f; // Aumentado para que tenga más rango de disparo

    // --- NUEVO: Parámetros de Disparo ---
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

    private FSMState currentState;

    // --- MÉTODO DE DISPARO ---
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

        // Instancia el proyectil en la posición del cazador.
        GameObject projectileGO = Instantiate(projectilePrefab, transform.position + direction * 2f, Quaternion.LookRotation(direction));
        // Lanza el proyectil.
        projectileGO.GetComponent<Projectile>().Launch(direction, projectileSpeed);
    }

    // ... (el resto del script OnEnable, Update, ChangeState, etc., se mantiene igual) ...
    #region Omitted Code
    private void OnEnable() { if (EntityManager.Instance != null) { EntityManager.Instance.RegisterHunter(this); } ChangeState(new PatrolState()); }
    private void OnDisable() { if (EntityManager.Instance != null) { EntityManager.Instance.UnregisterHunter(this); } }
    protected override void Update() { if (currentState != null) { currentState.Execute(this); } base.Update(); }
    public void ChangeState(FSMState newState) { if (currentState != null) { currentState.Exit(this); } currentState = newState; if (currentState != null) { currentState.Enter(this); } }
    public void SetDebugInfo(Color color, string status) { SetDebugColor(color); debugStatusText = status; }
    protected override void OnDrawGizmos() { base.OnDrawGizmos(); DebugHelper.DrawCircle(transform.position, sightRadius, Color.yellow); DebugHelper.DrawCircle(transform.position, attackRadius, Color.red); }
    #endregion
}

// El enum se mantiene igual
public enum HunterState { Patrolling, Hunting, Attacking, Resting }