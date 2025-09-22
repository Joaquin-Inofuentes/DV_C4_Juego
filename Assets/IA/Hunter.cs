using UnityEngine;

/// <summary>
/// Representa al NPC Cazador.
/// Su comportamiento es controlado por una Máquina de Estados Finita (FSM).
/// </summary>
public class Hunter : Agent
{
    [Header("Parámetros de la FSM")]
    public float energy = 100f;
    public float maxEnergy = 100f;

    private FSMState currentState;

    private void Start()
    {
        EntityManager.Instance.RegisterHunter(this);
        // El estado inicial del cazador es Patrullar.
        ChangeState(new PatrolState());
    }

    protected override void Update()
    {
        // Delega la lógica de comportamiento al estado actual.
        if (currentState != null)
        {
            currentState.Execute(this);
        }

        // Llama al Update de la clase base para aplicar el movimiento.
        base.Update();
    }

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

        // Ejecuta la lógica de entrada del nuevo estado.
        if (currentState != null)
        {
            currentState.Enter(this);
        }
    }
}