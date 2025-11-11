using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class Boid : Agent
{
    // -------------------------------------------------------------------
    // --- PARÁMETROS PÚBLICOS (Ajustables en el Inspector de Unity) ---
    // -------------------------------------------------------------------

    [Header("Parámetros de Detección")]
    public float foodDetectionRadius = 15f;
    public float hunterDetectionRadius = 20f;
    public float flockmateDetectionRadius = 10f;

    [Header("Parámetros de Flocking")]
    public float cohesionWeight = 1.0f;
    public float alignmentWeight = 1.0f;
    public float separationWeight = 1.5f;

    [Header("Parámetros de Wander")]
    public float wanderDistance = 10f;
    public float wanderRadius = 5f;
    public float wanderJitter = 40f;

    [Header("Parámetros de Evasión de Muros")]
    public LayerMask wallLayer;
    public float wallAvoidanceDistance = 10f;
    public float wallAvoidanceWeight = 2.0f;
    public float whiskerAngle = 20f;

    [Header("Parámetros de Separación de Muros")]
    public float wallSeparationRadius = 3f;
    public float wallSeparationWeight = 1.8f;

    // -------------------------------------------------------------------
    // --- VARIABLES PRIVADAS ---
    // -------------------------------------------------------------------

    private float wanderAngle = 0f;

    // -------------------------------------------------------------------
    // --- MÉTODOS DE UNITY (Ciclo de Vida) ---
    // -------------------------------------------------------------------

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

    // -------------------------------------------------------------------
    // --- LÓGICA PRINCIPAL (El "Cerebro" del Boid) ---
    // -------------------------------------------------------------------

    private void ExecuteDecisionTree()
    {
        if (AvoidWalls())
        {
            return;
        }

        Vector3 wallSeparationForce = CalculateWallSeparation();
        Vector3 mainSteeringForce = Vector3.zero;

        Hunter hunter = EntityManager.Instance.hunter;
        GameObject closestFood = FindClosestInList(EntityManager.Instance.foodItems, foodDetectionRadius);
        var flockmates = EntityManager.Instance.boids.Where(b => b != this && b.gameObject.activeInHierarchy && Vector3.Distance(transform.position, b.transform.position) < flockmateDetectionRadius).ToList();

        if (hunter != null && Vector3.Distance(transform.position, hunter.transform.position) < hunterDetectionRadius)
        {
            mainSteeringForce = EvadeHunter(hunter);
        }
        else if (closestFood != null)
        {
            mainSteeringForce = GoToFood(closestFood);
        }
        else if (flockmates.Count > 0)
        {
            mainSteeringForce = ApplyFlocking(flockmates);
        }
        else
        {
            mainSteeringForce = Wander();
        }

        ApplyForce(mainSteeringForce);
        ApplyForce(wallSeparationForce * wallSeparationWeight);
    }

    // -------------------------------------------------------------------
    // --- COMPORTAMIENTOS DE EVASIÓN ---
    // -------------------------------------------------------------------

    private bool AvoidWalls()
    {
        if (velocity.magnitude < 0.1f) return false;

        Vector3 forwardDirection = velocity.normalized;
        Vector3 leftWhiskerDirection = Quaternion.Euler(0, -whiskerAngle, 0) * forwardDirection;
        Vector3 rightWhiskerDirection = Quaternion.Euler(0, whiskerAngle, 0) * forwardDirection;

        bool wallDetected = false;
        Vector3 steeringForce = Vector3.zero;

        if (Physics.Raycast(transform.position, forwardDirection, out RaycastHit hitCenter, wallAvoidanceDistance, wallLayer))
        {
            debugStatusText = "Evadir Muro!";
            Vector3 desiredVelocity = hitCenter.normal * maxSpeed;
            steeringForce = desiredVelocity - velocity;
            wallDetected = true;
            Debug.DrawLine(transform.position, hitCenter.point, Color.red);
            Debug.DrawLine(hitCenter.point, hitCenter.point + hitCenter.normal * 5f, Color.cyan);
        }
        else
        {
            bool leftHit = Physics.Raycast(transform.position, leftWhiskerDirection, out RaycastHit hitLeft, wallAvoidanceDistance, wallLayer);
            bool rightHit = Physics.Raycast(transform.position, rightWhiskerDirection, out RaycastHit hitRight, wallAvoidanceDistance, wallLayer);

            if (leftHit)
            {
                debugStatusText = "Evadir Muro";
                Vector3 desiredVelocity = rightWhiskerDirection * maxSpeed;
                steeringForce += desiredVelocity - velocity;
                wallDetected = true;
                Debug.DrawLine(transform.position, hitLeft.point, Color.yellow);
            }
            if (rightHit)
            {
                debugStatusText = "Evadir Muro";
                Vector3 desiredVelocity = leftWhiskerDirection * maxSpeed;
                steeringForce += desiredVelocity - velocity;
                wallDetected = true;
                Debug.DrawLine(transform.position, hitRight.point, Color.yellow);
            }
        }

        if (wallDetected)
        {
            ApplyForce(steeringForce * wallAvoidanceWeight);
            return true;
        }
        else
        {
            Debug.DrawLine(transform.position, transform.position + forwardDirection * wallAvoidanceDistance, Color.green);
            Debug.DrawLine(transform.position, transform.position + leftWhiskerDirection * wallAvoidanceDistance, Color.green);
            Debug.DrawLine(transform.position, transform.position + rightWhiskerDirection * wallAvoidanceDistance, Color.green);
            return false;
        }
    }

    private Vector3 CalculateWallSeparation()
    {
        Vector3 separationForce = Vector3.zero;
        Collider[] nearbyWalls = Physics.OverlapSphere(transform.position, wallSeparationRadius, wallLayer);
        foreach (var wallCollider in nearbyWalls)
        {
            Vector3 closestPoint = wallCollider.ClosestPoint(transform.position);
            Vector3 repulsion = transform.position - closestPoint;
            float distance = repulsion.magnitude;
            separationForce += repulsion.normalized / distance;
            Debug.DrawLine(closestPoint, transform.position, Color.magenta);
        }
        return separationForce;
    }

    // -------------------------------------------------------------------
    // --- COMPORTAMIENTOS DE ACCIÓN PRINCIPAL ---
    // -------------------------------------------------------------------

    private Vector3 GoToFood(GameObject food)
    {
        debugStatusText = "Comer";
        SetDebugColor(Color.green);
        Debug.DrawLine(transform.position, food.transform.position, Color.green);
        if (Vector3.Distance(transform.position, food.transform.position) < 1.5f)
        {
            food.GetComponent<Food>().Consume();
        }
        return Arrive(food.transform.position);
    }

    private Vector3 EvadeHunter(Hunter hunter)
    {
        debugStatusText = "Huir";
        SetDebugColor(Color.red);
        Debug.DrawLine(transform.position, hunter.transform.position, Color.red);
        Vector3 desired = (transform.position - hunter.transform.position).normalized * maxSpeed;
        return desired - velocity;
    }

    private Vector3 ApplyFlocking(List<Boid> flockmates)
    {
        debugStatusText = "Flock";
        SetDebugColor(Color.blue);
        Vector3 cohesionForce = CalculateCohesion(flockmates);
        Vector3 alignmentForce = CalculateAlignment(flockmates);
        Vector3 separationForce = CalculateSeparation(flockmates);
        return (cohesionForce * cohesionWeight) +
               (alignmentForce * alignmentWeight) +
               (separationForce * separationWeight);
    }

    private Vector3 Wander()
    {
        debugStatusText = "Wander";
        SetDebugColor(Color.gray);
        wanderAngle += Random.Range(-1f, 1f) * wanderJitter * Time.deltaTime;

        Vector3 directionBase = velocity.sqrMagnitude > 0.001f ? velocity.normalized : transform.forward;

        Vector3 circleCenter = transform.position + directionBase * wanderDistance;
        Vector3 offset = new Vector3(Mathf.Cos(wanderAngle * Mathf.Deg2Rad), 0, Mathf.Sin(wanderAngle * Mathf.Deg2Rad)) * wanderRadius;
        Vector3 target = circleCenter + offset;
        DebugHelper.DrawCircle(circleCenter, wanderRadius, Color.gray);
        Debug.DrawLine(transform.position, target, Color.white);
        DrawSphere(target, 0.5f, Color.white);
        Vector3 desired = (target - transform.position).normalized * maxSpeed;
        return desired - velocity;
    }

    // -------------------------------------------------------------------
    // --- MÉTODOS DE CÁLCULO PARA FLOCKING ---
    // -------------------------------------------------------------------

    private Vector3 CalculateCohesion(List<Boid> flockmates)
    {
        Vector3 centerOfMass = Vector3.zero;
        foreach (var mate in flockmates)
        {
            centerOfMass += mate.transform.position;
        }
        centerOfMass /= flockmates.Count;
        DrawSphere(centerOfMass, 0.5f, Color.blue);
        Debug.DrawLine(transform.position, centerOfMass, Color.blue);
        return Arrive(centerOfMass);
    }

    private Vector3 CalculateAlignment(List<Boid> flockmates)
    {
        Vector3 averageVelocity = Vector3.zero;
        foreach (var mate in flockmates)
        {
            averageVelocity += mate.velocity;
        }
        averageVelocity /= flockmates.Count;

        if (averageVelocity.sqrMagnitude < 0.001f)
        {
            return Vector3.zero;
        }

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
            if (distance > 0.001f && distance < flockmateDetectionRadius / 2)
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
            if (separationForce.sqrMagnitude > 0.001f)
            {
                Vector3 desiredVelocity = separationForce.normalized * maxSpeed;
                return desiredVelocity - velocity;
            }
        }
        return Vector3.zero;
    }

    // -------------------------------------------------------------------
    // --- MÉTODOS DE AYUDA Y DEPURACIÓN ---
    // -------------------------------------------------------------------

    private Vector3 Arrive(Vector3 target)
    {
        Vector3 desired = target - transform.position;
        float distance = desired.magnitude;

        if (distance < 0.1f)
        {
            return Vector3.zero;
        }

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
        //DebugHelper.DrawCircle(transform.position, foodDetectionRadius, Color.green);
        DebugHelper.DrawCircle(transform.position, hunterDetectionRadius, Color.red);
        //DebugHelper.DrawCircle(transform.position, flockmateDetectionRadius, Color.blue);
        DebugHelper.DrawCircle(transform.position, wallSeparationRadius, Color.magenta);
    }

    private void DrawSphere(Vector3 position, float radius, Color color)
    {
        DebugHelper.DrawCircle(position, radius, color);
    }
}