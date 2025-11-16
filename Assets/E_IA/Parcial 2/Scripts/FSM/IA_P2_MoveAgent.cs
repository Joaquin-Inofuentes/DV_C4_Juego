using CustomInspector;
using System.Collections.Generic;
using UnityEngine;

public class IA_P2_MoveAgent : MonoBehaviour
{
    [Button(nameof(ToggleState))]
    public IA_P2_AgentIA agent;

    public List<Transform> patrolWaypoints;
    public GameObject target;

    public IA_P2_INT_gentState _patrolState;
    public IA_P2_INT_gentState _chaseState;
    public IA_P2_INT_gentState _currentState;

    void OnEnable()
    {
        _patrolState = new IA_P2_ST_PatrolState();
        _chaseState = new IA_P2_ST_ChaseState();

        _currentState = null;
        ToggleState(); // Empieza con el primero disponible
    }

    void Update()
    {
        if (_currentState != null)
        {
            _currentState.Execute(agent);
        }
        else
        {
            Debug.LogWarning("IA_P2_MoveAgent: No hay estado activo");
        }
    }

    public void ToggleState()
    {
        if (_currentState != null)
        {
            _currentState.Exit(agent);
            Debug.Log("IA_P2_MoveAgent: Salió de estado → " + _currentState.GetType().Name);
        }

        // Decidir siguiente estado
        if (_currentState == _patrolState || _currentState == null)
        {
            if (target != null)
            {
                _currentState = _chaseState;
                Debug.Log("IA_P2_MoveAgent: Cambia a ChaseState");
            }
            else if (patrolWaypoints != null && patrolWaypoints.Count > 0)
            {
                _currentState = _patrolState;
                Debug.Log("IA_P2_MoveAgent: Cambia a PatrolState");
            }
            else
            {
                Debug.LogError("IA_P2_MoveAgent: No se asignó target ni waypoints.");
                _currentState = null;
                return;
            }
        }
        else if (_currentState == _chaseState)
        {
            if (patrolWaypoints != null && patrolWaypoints.Count > 0)
            {
                _currentState = _patrolState;
                Debug.Log("IA_P2_MoveAgent: Cambia a PatrolState");
            }
            else if (target != null)
            {
                _currentState = _chaseState;
                Debug.Log("IA_P2_MoveAgent: Cambia a ChaseState");
            }
            else
            {
                Debug.LogError("IA_P2_MoveAgent: No se asignó target ni waypoints.");
                _currentState = null;
                return;
            }
        }

        // Entrar en el nuevo estado
        if (_currentState != null)
        {
            _currentState.Enter(agent);
            Debug.Log("IA_P2_MoveAgent: Entró en estado → " + _currentState.GetType().Name);
        }
    }
}
