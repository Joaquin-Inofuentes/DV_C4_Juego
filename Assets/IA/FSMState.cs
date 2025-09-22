/// <summary>
/// Interfaz/Clase base abstracta para todos los estados de la FSM.
/// Define el contrato que cada estado debe cumplir.
/// </summary>
public abstract class FSMState
{
    /// <summary>
    /// Se ejecuta una sola vez al entrar en este estado.
    /// </summary>
    public abstract void Enter(Agent agent);

    /// <summary>
    /// Se ejecuta en cada frame mientras el agente está en este estado.
    /// </summary>
    public abstract void Execute(Agent agent);

    /// <summary>
    /// Se ejecuta una sola vez al salir de este estado.
    /// </summary>
    public abstract void Exit(Agent agent);
}