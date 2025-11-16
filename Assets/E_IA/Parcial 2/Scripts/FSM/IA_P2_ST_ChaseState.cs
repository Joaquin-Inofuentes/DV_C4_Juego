using UnityEngine;

public class IA_P2_ST_ChaseState : IA_P2_INT_gentState
{
    private GameObject _target;

    public IA_P2_ST_ChaseState(GameObject target)
    {
        _target = target;
    }

    public void Enter(IA_P2_AgentIA agent) { }

    public void Execute(IA_P2_AgentIA agent)
    {
        if (_target == null) return;
        agent.GoTo(_target.transform.position);
    }

    public void Exit(IA_P2_AgentIA agent)
    {
        agent.StopAgent();
    }
}
