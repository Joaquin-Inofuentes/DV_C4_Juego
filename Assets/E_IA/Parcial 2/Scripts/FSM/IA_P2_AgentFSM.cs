using UnityEngine;

public class IA_P2_AgentFSM : MonoBehaviour
{
    public IA_P2_AgentIA agent;

    private IA_P2_INT_gentState currentState;

    public void SetState(IA_P2_INT_gentState newState)
    {
        if (currentState != null)
            currentState.Exit(agent);

        currentState = newState;

        if (currentState != null)
            currentState.Enter(agent);
    }

    void Update()
    {
        if (currentState != null)
            currentState.Execute(agent);
    }

    // Método simple para alternar entre dos estados
    public void ToggleState(IA_P2_INT_gentState stateA, IA_P2_INT_gentState stateB)
    {
        SetState(currentState == stateA ? stateB : stateA);
    }
}
