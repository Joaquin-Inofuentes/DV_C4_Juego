using UnityEngine;

public class IA_P2_ST_ChaseState : IA_P2_INT_gentState
{

    public void Enter(IA_P2_AgentIA agent) { }

    public void Execute(IA_P2_AgentIA agent)
    {
        if (agent.targetObject == null) return;
        agent.GoTo(agent.targetObject.transform.position);
    }

    public void Exit(IA_P2_AgentIA agent)
    {
        agent.StopAgent();
    }
}
