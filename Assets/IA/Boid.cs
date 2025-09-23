using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class Boid : Agent
{
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
    // --- NUEVO: El ángulo de los "bigotes" laterales ---
    [Tooltip("El ángulo en grados de los sensores laterales para una evasión más suave.")]
    public float whiskerAngle = 20f;

    private float wanderAngle = 0f;

    private void OnEnable()
    {
        if (EntityManager.Instance != null) { EntityManager.Instance.RegisterBoid(this); }
        velocity = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f)).normalized;
    }

    private void OnDisable()
    {
        if (EntityManager.Instance != null) { EntityManager.Instance.UnregisterBoid(this); }
    }

    protected override void Update()
    {
        ExecuteDecisionTree();
        base.Update();
    }

    private void ExecuteDecisionTree()
    {
        if (AvoidWalls())
        {
            return;
        }

        Hunter hunter = EntityManager.Instance.hunter;
        GameObject closestFood = FindClosestInList(EntityManager.Instance.foodItems, foodDetectionRadius);
        var flockmates = EntityManager.Instance.boids.Where(b => b != this && b.gameObject.activeInHierarchy && Vector3.Distance(transform.position, b.transform.position) < flockmateDetectionRadius).ToList();

        if (hunter != null && Vector3.Distance(transform.position, hunter.transform.position) < hunterDetectionRadius) { EvadeHunter(hunter); return; }
        if (closestFood != null) { GoToFood(closestFood); return; }
        if (flockmates.Count > 0) { ApplyFlocking(flockmates); return; }
        Wander();
    }

    // --- MÉTODO DE EVASIÓN DE MUROS COMPLETAMENTE REESCRITO ---
    private bool AvoidWalls()
    {
        if (velocity.magnitude < 0.1f) return false;

        // --- CÁLCULO DE LOS "BIGOTES" (WHISKERS) ---
        Vector3 forwardDirection = velocity.normalized;
        // Rota la dirección hacia la izquierda para el bigote izquierdo.
        Vector3 leftWhiskerDirection = Quaternion.Euler(0, -whiskerAngle, 0) * forwardDirection;
        // Rota la dirección hacia la derecha para el bigote derecho.
        Vector3 rightWhiskerDirection = Quaternion.Euler(0, whiskerAngle, 0) * forwardDirection;

        bool wallDetected = false;
        Vector3 steeringForce = Vector3.zero;

        // 1. Comprobar el rayo central (peligro inminente)
        if (Physics.Raycast(transform.position, forwardDirection, out RaycastHit hitCenter, wallAvoidanceDistance, wallLayer))
        {
            debugStatusText = "Evadir Muro!";
            // Maniobra de pánico: usar la normal del muro para un escape seguro.
            Vector3 desiredVelocity = hitCenter.normal * maxSpeed;
            steeringForce = desiredVelocity - velocity;
            wallDetected = true;

            // Visualización: Rayo central en rojo, normal de escape en cian.
            Debug.DrawLine(transform.position, hitCenter.point, Color.red);
            Debug.DrawLine(hitCenter.point, hitCenter.point + hitCenter.normal * 5f, Color.cyan);
        }
        // 2. Si el centro está libre, comprobar los bigotes laterales
        else
        {
            bool leftHit = Physics.Raycast(transform.position, leftWhiskerDirection, out RaycastHit hitLeft, wallAvoidanceDistance, wallLayer);
            bool rightHit = Physics.Raycast(transform.position, rightWhiskerDirection, out RaycastHit hitRight, wallAvoidanceDistance, wallLayer);

            if (leftHit)
            {
                debugStatusText = "Evadir Muro";
                // Si el bigote izquierdo choca, la fuerza de dirección debe ser hacia la DERECHA.
                // Usamos la dirección del bigote derecho como una buena ruta de escape.
                Vector3 desiredVelocity = rightWhiskerDirection * maxSpeed;
                steeringForce += desiredVelocity - velocity;
                wallDetected = true;
                Debug.DrawLine(transform.position, hitLeft.point, Color.yellow); // Bigote de aviso en amarillo
            }

            if (rightHit)
            {
                debugStatusText = "Evadir Muro";
                // Si el bigote derecho choca, la fuerza de dirección debe ser hacia la IZQUIERDA.
                Vector3 desiredVelocity = leftWhiskerDirection * maxSpeed;
                steeringForce += desiredVelocity - velocity;
                wallDetected = true;
                Debug.DrawLine(transform.position, hitRight.point, Color.yellow); // Bigote de aviso en amarillo
            }
        }

        // 3. Aplicar la fuerza y dibujar los sensores restantes
        if (wallDetected)
        {
            ApplyForce(steeringForce * wallAvoidanceWeight);
            return true;
        }
        else
        {
            // Si no se detectó nada, dibuja todos los sensores en verde.
            Debug.DrawLine(transform.position, transform.position + forwardDirection * wallAvoidanceDistance, Color.green);
            Debug.DrawLine(transform.position, transform.position + leftWhiskerDirection * wallAvoidanceDistance, Color.green);
            Debug.DrawLine(transform.position, transform.position + rightWhiskerDirection * wallAvoidanceDistance, Color.green);
            return false;
        }
    }

    // --- El resto de los métodos se mantienen exactamente igual ---

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
        Vector3 offset = new Vector3(Mathf.Cos(wanderAngle * Mathf.Deg2Rad), 0, Mathf.Sin(wanderAngle * Mathf.Deg2Rad)) * wanderRadius;
        Vector3 target = circleCenter + offset;
        DebugHelper.DrawCircle(circleCenter, wanderRadius, Color.gray);
        Debug.DrawLine(transform.position, target, Color.white);
        DrawSphere(target, 0.5f, Color.white);
        Vector3 desired = (target - transform.position).normalized * maxSpeed;
        ApplyForce(desired - velocity);
    }

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
        //base.OnDrawGizmos();
        //DebugHelper.DrawCircle(transform.position, foodDetectionRadius, Color.green);
        DebugHelper.DrawCircle(transform.position, hunterDetectionRadius, Color.red);
        //DebugHelper.DrawCircle(transform.position, flockmateDetectionRadius, Color.blue);
    }

    private void DrawSphere(Vector3 position, float radius, Color color)
    {
        DebugHelper.DrawCircle(position, radius, color);
    }
}