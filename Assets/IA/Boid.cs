using UnityEngine;

/// <summary>
/// Representa un agente Boid (presa).
/// Utiliza un Árbol de Decisión para elegir su comportamiento en cada frame.
/// </summary>
public class Boid : Agent
{
    private void Start()
    {
        // Al iniciar, se registra en el EntityManager
        EntityManager.Instance.RegisterBoid(this);
    }

    private void OnDestroy()
    {
        // Al ser destruido, se da de baja del EntityManager
        if (EntityManager.Instance != null)
        {
            EntityManager.Instance.UnregisterBoid(this);
        }
    }

    protected override void Update()
    {
        ExecuteDecisionTree();

        // Llama al Update de la clase base para aplicar el movimiento
        base.Update();
    }

    /// <summary>
    /// Simula un Árbol de Decisión para determinar la acción del Boid.
    /// </summary>
    private void ExecuteDecisionTree()
    {
        // Aquí iría la lógica real de percepción. Por ahora, son solo placeholders.
        bool foodNearby = false; // Placeholder
        bool hunterNearby = false; // Placeholder
        bool flockmatesNearby = true; // Placeholder

        if (foodNearby)
        {
            GoToFood();
        }
        else if (hunterNearby)
        {
            EvadeHunter();
        }
        else if (flockmatesNearby)
        {
            ApplyFlocking();
        }
        else
        {
            Wander();
        }
    }

    // --- Métodos de Comportamiento (Steering Behaviors) ---

    private void GoToFood()
    {
        Debug.Log($"{name}: Decisión -> Ir a por comida (Arrive).");
        // Lógica de Arrive iría aquí.
        // Vector3 force = Arrive(targetFood.position);
        // ApplyForce(force);
    }

    private void EvadeHunter()
    {
        Debug.Log($"{name}: Decisión -> Huir del cazador (Evade).");
        // Lógica de Evade iría aquí.
        // Vector3 force = Evade(hunter.position, hunter.velocity);
        // ApplyForce(force);
    }

    private void ApplyFlocking()
    {
        Debug.Log($"{name}: Decisión -> Mantenerse con el grupo (Flocking).");
        // Lógica de Flocking (combinación de las 3 reglas) iría aquí.
        // Vector3 separation = Separation();
        // Vector3 alignment = Alignment();
        // Vector3 cohesion = Cohesion();
        // ApplyForce(separation + alignment + cohesion);
    }

    private void Wander()
    {
        Debug.Log($"{name}: Decisión -> Moverse aleatoriamente (Wander).");
        // Lógica de Wander iría aquí.
        // Vector3 force = Wander();
        // ApplyForce(force);
    }
}