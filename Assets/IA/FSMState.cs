/// <summary>
/// Clase base abstracta para todos los estados de la Máquina de Estados Finita (FSM).
/// Define los métodos que cada estado concreto debe implementar.
/// </summary>
public abstract class FSMState
{
    /// <summary>
    /// Se ejecuta una sola vez al entrar en este estado. Ideal para inicializaciones.
    /// </summary>
    public abstract void Enter(Agent agent);

    /// <summary>
    /// Se ejecuta en cada frame mientras el agente está en este estado. Contiene la lógica principal.
    /// </summary>
    public abstract void Execute(Agent agent);

    /// <summary>
    /// Se ejecuta una sola vez al salir de este estado. Ideal para limpieza.
    /// </summary>
    public abstract void Exit(Agent agent);
}