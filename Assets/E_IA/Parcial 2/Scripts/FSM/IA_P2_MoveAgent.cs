// --- IA_P2_MoveAgent.cs ---
// (Modificado para pasar 'this' como contexto)

using CustomInspector;
using System.Collections.Generic;
using UnityEngine;

public class IA_P2_MoveAgent : MonoBehaviour
{
    [Button(nameof(ToggleState))]
    public IA_P2_AgentIA agent; // El "movedor"

    [Header("Datos de Estados")]
    public List<Transform> patrolWaypoints; // Los waypoints VIVEN AQUÍ
    public GameObject target;               // El objetivo VIVE AQUÍ

    public IA_P2_INT_gentState _patrolState;
    public IA_P2_INT_gentState _chaseState;
    public IA_P2_INT_gentState _currentState;

    void OnEnable()
    {
        // Los estados ya no necesitan los datos en el constructor,
        // los tomarán del contexto cuando los necesiten.
        _patrolState = new IA_P2_ST_PatrolState();
        _chaseState = new IA_P2_ST_ChaseState(); // (No me diste este script, pero asumo que existe)

        _currentState = null;
        ToggleState(); // Empieza con el primero disponible
    }

    void Update()
    {
        if (_currentState != null)
        {
            // Pasa 'this' (el contexto) al estado
            _currentState.Execute(this);
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
            // Pasa 'this' (el contexto) al estado
            _currentState.Exit(this);
            //Debug.Log("IA_P2_MoveAgent: Salió de estado → " + _currentState.GetType().Name);
        }

        // Decidir siguiente estado
        // Esta lógica está bien, ya que 'target' y 'patrolWaypoints' son de esta clase.
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
            // Pasa 'this' (el contexto) al estado
            _currentState.Enter(this);
            //Debug.Log("IA_P2_MoveAgent: Entró en estado → " + _currentState.GetType().Name);
        }
    }
}