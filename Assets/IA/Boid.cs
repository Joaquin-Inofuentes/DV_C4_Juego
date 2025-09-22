using UnityEngine;
using System.Linq;
using System.Collections.Generic;

// Boid hereda de Agent, por lo que tiene acceso a 'velocity', 'maxSpeed', 'ApplyForce', etc.
public class Boid : Agent
{
    // --- VARIABLES PÚBLICAS ---

    [Header("Parámetros de Detección")]
    public float foodDetectionRadius = 15f;
    public float hunterDetectionRadius = 20f;
    public float flockmateDetectionRadius = 10f;

    [Header("Parámetros de Flocking")]
    [Tooltip("Peso de cohesión (0–1).")]
    [Range(0f, 1f)] public float cohesionWeight = 0.4f;

    [Tooltip("Peso de alineamiento (0–1).")]
    [Range(0f, 1f)] public float alignmentWeight = 0.2f;

    [Tooltip("Peso de separación (0–1).")]
    [Range(0f, 1f)] public float separationWeight = 0.3f; // ojo, 1.5 no entra en el rango 0–1


    [Header("Parámetros de Wander")]
    [Tooltip("Distancia a la que se proyecta el círculo de Wander.")]
    public float wanderDistance = 10f;
    [Tooltip("Radio del círculo de Wander. Un radio mayor produce giros más amplios.")]
    public float wanderRadius = 5f;
    [Tooltip("Magnitud del desplazamiento aleatorio por segundo. Un valor mayor produce giros más erráticos.")]
    public float wanderJitter = 40f;

    // --- VARIABLE PRIVADA PARA "MEMORIA" DE WANDER ---
    // Almacena el ángulo actual en el círculo de Wander para que persista entre frames.
    private float wanderAngle = 0f;

    // --- MÉTODOS DE UNITY ---

    private void OnEnable()
    {
        if (EntityManager.Instance != null)
        {
            EntityManager.Instance.RegisterBoid(this);
        }
        velocity = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f)).normalized;
    }

    private void OnDisable()
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

    // --- LÓGICA PRINCIPAL (ÁRBOL DE DECISIÓN) ---

    private void ExecuteDecisionTree()
    {
        Hunter hunter = EntityManager.Instance.hunter;
        GameObject closestFood = FindClosestInList(EntityManager.Instance.foodItems, foodDetectionRadius);
        var flockmates = EntityManager.Instance.boids.Where(b => b != this && b.gameObject.activeInHierarchy && Vector3.Distance(transform.position, b.transform.position) < flockmateDetectionRadius).ToList();

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

    // --- COMPORTAMIENTOS (ACCIONES) ---

    private void GoToFood(GameObject food)
    {
        debugStatusText = "Comer";
        SetDebugColor(Color.green);
        Debug.DrawLine(transform.position, food.transform.position, Color.green);
        ApplyForce(Arrive(food.transform.position));
    }

    private void EvadeHunter(Hunter hunter)
    {
        debugStatusText = "Huir";
        SetDebugColor(Color.red);
        Debug.DrawLine(transform.position, hunter.transform.position, Color.red);
        Vector3 desired = (transform.position - hunter.transform.position).normalized * maxSpeed;
        Vector3 steeringForce = desired - velocity;
        ApplyForce(steeringForce);
    }

    private void ApplyFlocking(List<Boid> flockmates)
    {
        debugStatusText = "Flock";
        SetDebugColor(Color.blue);
        Vector3 cohesionForce = CalculateCohesion(flockmates);
        Vector3 alignmentForce = CalculateAlignment(flockmates);
        Vector3 separationForce = CalculateSeparation(flockmates);
        ApplyForce(cohesionForce * cohesionWeight);
        ApplyForce(alignmentForce * alignmentWeight);
        ApplyForce(separationForce * separationWeight);
    }

    private void Wander()
    {
        debugStatusText = "Wander";
        SetDebugColor(Color.gray);

        wanderAngle += Random.Range(-1f, 1f) * wanderJitter * Time.deltaTime;

        Vector3 circleCenter = transform.position + velocity.normalized * wanderDistance;

        Vector3 offset = new Vector3(
            Mathf.Cos(wanderAngle * Mathf.Deg2Rad),
            0,
            Mathf.Sin(wanderAngle * Mathf.Deg2Rad)
        ) * wanderRadius;

        Vector3 target = circleCenter + offset;

        DebugHelper.DrawCircle(circleCenter, wanderRadius, Color.gray);
        Debug.DrawLine(transform.position, target, Color.white);
        DrawSphere(target, 0.5f, Color.white);

        Vector3 desired = (target - transform.position).normalized * maxSpeed;
        ApplyForce(desired - velocity);
    }

    // --- CÁLCULOS DE FLOCKING ---

    private Vector3 CalculateCohesion(List<Boid> flockmates)
    {
        Vector3 centerOfMass = Vector3.zero;
        foreach (var mate in flockmates) { centerOfMass += mate.transform.position; }
        centerOfMass /= flockmates.Count;
        DrawSphere(centerOfMass, 0.5f, Color.blue);
        Debug.DrawLine(transform.position, centerOfMass, Color.blue);
        return Arrive(centerOfMass);
    }

    private Vector3 CalculateAlignment(List<Boid> flockmates)
    {
        Vector3 averageVelocity = Vector3.zero;
        foreach (var mate in flockmates) { averageVelocity += mate.velocity; }
        averageVelocity /= flockmates.Count;
        Vector3 alignmentLineEnd = transform.position + averageVelocity.normalized * 5f;
        Debug.DrawLine(transform.position, alignmentLineEnd, Color.white);
        DrawSphere(alignmentLineEnd, 0.2f, Color.white);
        Vector3 desiredVelocity = averageVelocity.normalized * maxSpeed;
        return desiredVelocity - velocity;
    }

    private Vector3 CalculateSeparation(List<Boid> flockmates)
    {
        Vector3 separationForce = Vector3.zero;
        int neighborsCount = 0;
        foreach (var mate in flockmates)
        {
            float distance = Vector3.Distance(transform.position, mate.transform.position);
            if (distance > 0 && distance < flockmateDetectionRadius / 2)
            {
                Vector3 repulsion = (transform.position - mate.transform.position).normalized / distance;
                separationForce += repulsion;
                neighborsCount++;
                Debug.DrawLine(mate.transform.position, transform.position, Color.red);
            }
        }
        if (neighborsCount > 0)
        {
            separationForce /= neighborsCount;
            Vector3 desiredVelocity = separationForce.normalized * maxSpeed;
            return desiredVelocity - velocity;
        }
        return Vector3.zero;
    }

    // --- MÉTODOS DE AYUDA ---

    private Vector3 Arrive(Vector3 target)
    {
        Vector3 desired = target - transform.position;
        float distance = desired.magnitude;
        float slowingRadius = 10f;
        if (distance < slowingRadius)
        {
            desired = desired.normalized * maxSpeed * (distance / slowingRadius);
        }
        else
        {
            desired = desired.normalized * maxSpeed;
        }
        return desired - velocity;
    }

    private GameObject FindClosestInList(List<GameObject> list, float radius)
    {
        return list
            .Where(item => item != null && item.activeInHierarchy && Vector3.Distance(transform.position, item.transform.position) < radius)
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

    private void DrawSphere(Vector3 position, float radius, Color color)
    {
        DebugHelper.DrawCircle(position, radius, color);
    }
}