using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Gestor Singleton para registrar y acceder a todas las entidades importantes en la escena.
/// Proporciona un punto de acceso central a listas de agentes, comida y waypoints.
/// </summary>
public class EntityManager : MonoBehaviour
{
    // --- Singleton Pattern ---
    public static EntityManager Instance
    {
        get
        {
            if (_instance == null)
                _instance = FindObjectOfType<EntityManager>();
            return _instance;
        }
        private set
        {
            _instance = value;
        }
    }
    private static EntityManager _instance;


    // --- Listas de Entidades ---
    [Header("Listas de Entidades")]
    public List<Boid> boids = new List<Boid>();
    public List<GameObject> foodItems = new List<GameObject>();
    public Hunter hunter;

    [Header("Puntos de Interés")]
    // Puedes tener múltiples rutas de patrulla. Por ahora, una es suficiente.
    public List<Transform> patrolWaypoints = new List<Transform>();


    public void Awake()
    {
        Instance = this;
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

    // --- MÉTODO QUE FALTABA Y CAUSABA EL ERROR ---
    public void UnregisterHunter(Hunter hunterInstance)
    {
        if (hunter == hunterInstance)
        {
            hunter = null;
        }
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            // Invierte el estado del interruptor global en la clase Agent.
            Agent.movementEnabled = !Agent.movementEnabled;

            if (Agent.movementEnabled)
            {
                Debug.Log("<color=lime>MOVIMIENTO HABILITADO</color> - Presiona 'R' para detener.", this);
            }
            else
            {
                Debug.Log("<color=red>MOVIMIENTO DESHABILITADO</color> - Presiona 'R' para reanudar.", this);
            }
        }
    }
}