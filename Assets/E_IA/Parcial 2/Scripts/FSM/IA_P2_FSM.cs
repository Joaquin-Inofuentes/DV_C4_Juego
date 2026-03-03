using CustomInspector;
using System.Collections.Generic;
using UnityEngine;

// (El Enum sigue fuera de la clase, lo cual es correcto)
public enum AgentState
{
    Patrolling,
    Chasing,
    Searching,
    ReturningToPatrol
}

public class IA_P2_FSM : MonoBehaviour
{
    [Button(nameof(ToggleState))]
    [Button(nameof(DibujarRutaPatrulla))]
    public IA_P2_AgentIA agent; // El "movedor" (ahora también los "ojos")

    [Header("Datos de Estados")]
    public List<Transform> patrolWaypoints;
    public GameObject target;
    public Vector3 lastKnownPosition;

    [Header("Configuración FSM")]
    [Tooltip("Estado inicial. Muestra el estado actual y se puede cambiar en Play Mode para forzar un estado.")]
    public AgentState currentStateEnum = AgentState.Patrolling;

    private AgentState _previousStateTracker;

    // Referencias a las instancias de estado (esto está bien)
    public IA_P2_INT_gentState _patrolState;
    public IA_P2_INT_gentState _chaseState;
    public IA_P2_INT_gentState _searchingState;
    public IA_P2_INT_gentState _returningState;

    public IA_P2_INT_gentState _currentState;

    // Escucha llamados de perseguir enemigo
    public IA_P2_FOV NotificacionDeEnemigoVisible;

    void OnEnable()
    {
        Debug.Log("Empezo a inicializarse");
        // Instanciamos TODOS los estados
        _patrolState = new IA_P2_ST_PatrolState();
        _chaseState = new IA_P2_ST_ChaseState();
        _searchingState = new IA_P2_ST_SearchingState();
        _returningState = new IA_P2_ST_ReturningToPatrolState();

        if (target == null) currentStateEnum = AgentState.Patrolling;

        switch (currentStateEnum) // <-- Lee el valor del Inspector
        {
            case AgentState.Patrolling:
                _currentState = _patrolState;
                break;
            case AgentState.Chasing:
                _currentState = _chaseState;
                break;
            default:
                _currentState = _patrolState;
                break;
        }

        // [MODIFICADO] Sincronizamos el tracker con el estado inicial
        _previousStateTracker = currentStateEnum;

        // Entrar al estado inicial
        if (_currentState != null)
        {
            _currentState.Enter(this);
        }
        Debug.Log("Se inicializo correctamente");
        NotificacionDeEnemigoVisible.OnTargetDetected += PerseguirEnemigo;
        IA_P2_BusEvent_Manager.OnEnemyFound += PerseguirEnemigo;
        NotificacionDeEnemigoVisible.OnTargetLost += LoPerdiDeVision;
    }

    public void LoPerdiDeVision(GameObject objetivoPerdido)
    {
        // Solo entramos en modo búsqueda si el objeto que se perdió 
        // es EXACTAMENTE el que estábamos persiguiendo.
        if (target != null && objetivoPerdido == target)
        {
            // Guardamos la última posición antes de pasar a Searching
            lastKnownPosition = target.transform.position;
            TransitionTo(AgentState.Searching);
        }
    }

    public void PerseguirEnemigo(GameObject enemigo)
    {
        if (enemigo == null) return;

        // Seguridad: No perseguirse a sí mismo
        if (enemigo == this.gameObject) return;

        target = enemigo;
        lastKnownPosition = enemigo.transform.position;

        if (currentStateEnum != AgentState.Chasing)
        {
            TransitionTo(AgentState.Chasing);
        }
    }

    void Update()
    {
        // [MODIFICADO] Comprobación de "Controlador" (Debug)
        // ¿El usuario cambió el enum en el Inspector?
        if (currentStateEnum != _previousStateTracker)
        {
            // Sí, el usuario forzó un cambio.
            Debug.LogWarning("CAMBIO MANUAL: Forzando transición a " + currentStateEnum.ToString());
            TransitionTo(currentStateEnum);
        }

        // Ejecución normal del estado
        if (_currentState != null)
        {
            _currentState.Execute(this);
        }
        else
        {
            Debug.LogWarning("IA_P2_FSM: No hay estado activo");
        }

        if (agent.currentPath == null || agent.currentPath.Count == 0)
        {
            //TransitionTo(AgentState.ReturningToPatrol);
            //Debug.Log("Intento de vovler a patrolling" + gameObject.name, gameObject);
        }
    }

    // [MÉTODO MODIFICADO]
    public void TransitionTo(AgentState stateKey)
    {
        IA_P2_INT_gentState newState = null;

        switch (stateKey)
        {
            case AgentState.Patrolling:
                newState = _patrolState;
                break;
            case AgentState.Chasing:
                newState = _chaseState;
                break;
            case AgentState.Searching:
                newState = _searchingState;
                break;
            case AgentState.ReturningToPatrol:
                newState = _returningState;
                break;
        }

        if (newState == null || newState == _currentState)
        {
            // Si el estado ya es el correcto, solo nos aseguramos
            // de que los enums de debug estén sincronizados
            currentStateEnum = stateKey;
            _previousStateTracker = stateKey;
            return;
        }

        if (_currentState != null)
        {
            _currentState.Exit(this);
        }

        //Debug.Log("IA_P2_FSM: Cambiando a " + stateKey.ToString());

        if (target != null && agent != null)
        {
            //ebug.DrawLine(agent.transform.position, target.transform.position, Color.blue, 3.0f);
        }

        _currentState = newState;
        _currentState.Enter(this);

        // [MODIFICADO] Sincroniza el "Visor" y el "Tracker" al nuevo estado
        currentStateEnum = stateKey;
        _previousStateTracker = stateKey;
    }

    public bool IsPlayerVisible()
    {
        if (target == null || agent == null)
            return false;

        // Como la lógica de FOV se quitó de IA_P2_AgentIA,
        // 'agent.CanSeeTarget(target)' ya no existe.
        // Por ahora, devolvemos 'false' para que compile.
        // Esto significa que el agente nunca "verá" al jugador automáticamente.
        return false;
    }

    // (El método ToggleState() sigue igual para debug)
    public void ToggleState()
    {
        if (_currentState == _patrolState)
        {
            TransitionTo(AgentState.Chasing);
        }
        else
        {
            TransitionTo(AgentState.Patrolling);
        }
    }


    public void DibujarRutaPatrulla()
    {
        if (patrolWaypoints == null || patrolWaypoints.Count < 2)
            return;

        for (int i = 0; i < patrolWaypoints.Count - 1; i++)
        {
            Debug.DrawLine(
                patrolWaypoints[i].position,
                patrolWaypoints[i + 1].position,
                Color.cyan,
                10f
            );
        }
    }

    public void SePerdioElObjetivo()
    {
        target = null;
    }

}