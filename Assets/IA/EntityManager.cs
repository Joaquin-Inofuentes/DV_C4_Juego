using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Gestor Singleton para registrar y acceder a todas las entidades importantes en la escena.
/// Proporciona un punto de acceso central a listas de agentes, comida y waypoints.
/// </summary>
public class EntityManager : MonoBehaviour
{
    // --- Singleton Pattern ---
    public static EntityManager Instance { get; private set; }

    // --- Listas de Entidades ---
    [Header("Listas de Entidades")]
    public List<Boid> boids = new List<Boid>();
    public List<GameObject> foodItems = new List<GameObject>();
    public Hunter hunter;

    [Header("Puntos de Interés")]
    // Puedes tener múltiples rutas de patrulla. Por ahora, una es suficiente.
    public List<Transform> patrolWaypoints = new List<Transform>();


    private void Awake()
    {
        // Implementación del patrón Singleton
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    // --- Métodos de Registro / Desregistro ---

    public void RegisterBoid(Boid boid)
    {
        if (!boids.Contains(boid))
        {
            boids.Add(boid);
            Debug.Log($"Boid '{boid.name}' registrado. Total: {boids.Count}");
        }
    }

    public void UnregisterBoid(Boid boid)
    {
        if (boids.Contains(boid))
        {
            boids.Remove(boid);
            Debug.Log($"Boid '{boid.name}' desregistrado. Restantes: {boids.Count}");
        }
    }

    public void RegisterFood(GameObject food)
    {
        if (!foodItems.Contains(food))
        {
            foodItems.Add(food);
            Debug.Log($"Comida '{food.name}' registrada. Total: {foodItems.Count}");
        }
    }

    public void UnregisterFood(GameObject food)
    {
        if (foodItems.Contains(food))
        {
            foodItems.Remove(food);
            Debug.Log($"Comida '{food.name}' desregistrada. Restantes: {foodItems.Count}");
        }
    }

    public void RegisterHunter(Hunter hunterInstance)
    {
        if (hunter == null)
        {
            hunter = hunterInstance;
            Debug.Log($"Cazador '{hunter.name}' registrado.");
        }
    }
}