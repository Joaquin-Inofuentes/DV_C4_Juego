using UnityEngine;
using System.Linq;

public class Boid : Agent
{
    [Header("Parámetros de Detección del Boid")]
    public float foodDetectionRadius = 15f;
    public float hunterDetectionRadius = 20f;
    public float flockmateDetectionRadius = 10f;

    private void Start()
    {
        EntityManager.Instance.RegisterBoid(this);
    }

    private void OnDestroy()
    {
        if (EntityManager.Instance != null)
        {
            EntityManager.Instance.UnregisterBoid(this);
        }
    }

    protected override void Update()
    {
        ExecuteDecisionTree();
        base.Update();
    }

    private void ExecuteDecisionTree()
    {
        Hunter hunter = EntityManager.Instance.hunter;
        GameObject closestFood = FindClosestInList(EntityManager.Instance.foodItems, foodDetectionRadius);
        var flockmates = EntityManager.Instance.boids.Where(b => b != this && Vector3.Distance(transform.position, b.transform.position) < flockmateDetectionRadius).ToList();

        if (hunter != null && Vector3.Distance(transform.position, hunter.transform.position) < hunterDetectionRadius)
        {
            EvadeHunter(hunter);
            return;
        }

        if (closestFood != null)
        {
            GoToFood(closestFood);
            return;
        }

        if (flockmates.Count > 0)
        {
            ApplyFlocking(flockmates);
            return;
        }

        Wander();
    }

    // --- Métodos de Comportamiento (Textos Simplificados) ---

    private void GoToFood(GameObject food)
    {
        debugStatusText = "Comer"; // <-- CAMBIO
        SetDebugColor(Color.green);
        Debug.DrawLine(transform.position, food.transform.position, Color.green, 0f, false);
    }

    private void EvadeHunter(Hunter hunter)
    {
        debugStatusText = "Huir"; // <-- CAMBIO
        SetDebugColor(Color.red);
        Vector3 threatPosition = hunter.transform.position + Vector3.up * 0.2f;
        Debug.DrawLine(transform.position, threatPosition, Color.red, 0f, false);
        Vector3 escapeDirection = (transform.position - hunter.transform.position).normalized;
        Debug.DrawLine(transform.position, transform.position + escapeDirection * 10f, Color.cyan, 0f, false);
    }

    private void ApplyFlocking(System.Collections.Generic.List<Boid> flockmates)
    {
        debugStatusText = "Flock"; // <-- CAMBIO
        SetDebugColor(Color.blue);
        foreach (var mate in flockmates)
        {
            Debug.DrawLine(transform.position, mate.transform.position, new Color(0, 0, 1, 0.5f));
        }
    }

    private void Wander()
    {
        debugStatusText = "Wander"; // <-- CAMBIO
        SetDebugColor(Color.gray);
    }

    private GameObject FindClosestInList(System.Collections.Generic.List<GameObject> list, float radius)
    {
        return list
            .Where(item => Vector3.Distance(transform.position, item.transform.position) < radius)
            .OrderBy(item => Vector3.Distance(transform.position, item.transform.position))
            .FirstOrDefault();
    }

    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();
        DebugHelper.DrawCircle(transform.position, foodDetectionRadius, Color.green);
        DebugHelper.DrawCircle(transform.position, hunterDetectionRadius, Color.red);
        DebugHelper.DrawCircle(transform.position, flockmateDetectionRadius, Color.blue);
    }
}