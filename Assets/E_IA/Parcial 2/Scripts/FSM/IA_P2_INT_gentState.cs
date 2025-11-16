// --- IA_P2_INT_gentState.cs ---
// (Esta es la interfaz que tus estados deben implementar)

public interface IA_P2_INT_gentState
{
    // Ahora reciben el "contexto" (MoveAgent) en lugar de solo el agente
    void Enter(IA_P2_FSM context);
    void Execute(IA_P2_FSM context);
    void Exit(IA_P2_FSM context);
}