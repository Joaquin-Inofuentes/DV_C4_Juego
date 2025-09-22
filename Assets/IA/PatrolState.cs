using UnityEngine;
using System.Linq;

public class PatrolState : FSMState
{
    private int _targetWaypointIndex = 0;

    public override void Enter(Agent agent)
    {
        Hunter hunter = (Hunter)agent;
        hunter.SetDebugInfo(Color.yellow, "WayPoints");
        // --- LÍNEA QUE CAUSABA EL ERROR SI EL ENUM NO EXISTE ---
        hunter.currentHunterState = HunterState.Patrolling;
        FindClosestWaypoint(hunter);
    }

    public override void Execute(Agent agent)
    {
        Hunter hunter = (Hunter)agent;
        var waypoints = EntityManager.Instance.patrolWaypoints;

        Boid closestBoid = FindClosestBoid(hunter);
        if (closestBoid != null)
        {
            hunter.ChangeState(new HuntingState(closestBoid));
            return;
        }

        if (waypoints.Count == 0) return;

        Vector3 targetPosition = waypoints[_targetWaypointIndex].position;
        float distanceToTarget = Vector3.Distance(hunter.transform.position, targetPosition);

        if (distanceToTarget > hunter.repathThresholdDistance)
        {
            FindClosestWaypoint(hunter);
            targetPosition = waypoints[_targetWaypointIndex].position;
        }

        Debug.DrawLine(hunter.transform.position, targetPosition, Color.yellow);
        DebugHelper.DrawCircle(targetPosition, hunter.waypointArrivalDistance, Color.cyan);

        if (distanceToTarget < hunter.waypointArrivalDistance)
        {
            _targetWaypointIndex = (_targetWaypointIndex + 1) % waypoints.Count;
        }

        hunter.energy -= 2 * Time.deltaTime;
        if (hunter.energy <= 0)
        {
            hunter.ChangeState(new IdleState());
        }
    }

    public override void Exit(Agent agent) { }

    private void FindClosestWaypoint(Hunter hunter)
    {
        var waypoints = EntityManager.Instance.patrolWaypoints;
        if (waypoints.Count == 0) return;

        float closestDistanceSqr = float.MaxValue;
        for (int i = 0; i < waypoints.Count; i++)
        {
            float distanceSqr = (hunter.transform.position - waypoints[i].position).sqrMagnitude;
            if (distanceSqr < closestDistanceSqr)
            {
                closestDistanceSqr = distanceSqr;
                _targetWaypointIndex = i;
            }
        }
    }

    private Boid FindClosestBoid(Hunter hunter)
    {
        return EntityManager.Instance.boids
            .Where(b => b != null && b.gameObject.activeInHierarchy)
            .Where(b => Vector3.Distance(hunter.transform.position, b.transform.position) < hunter.sightRadius)
            .OrderBy(b => Vector3.Distance(hunter.transform.position, b.transform.position))
            .FirstOrDefault();
    }
}