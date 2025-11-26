using TMPro;
using System.Collections.Generic;
using UnityEngine;
// --- NUEVO: Necesitamos esta directiva para usar el método .RemoveAll() de LINQ ---
using System.Linq;
using UnityEngine.SceneManagement;

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
        }
    }

    public void UnregisterBoid(Boid boid)
    {
        if (boids.Contains(boid))
        {
            boids.Remove(boid);
        }
    }

    public void RegisterFood(GameObject food)
    {
        if (!foodItems.Contains(food))
        {
            foodItems.Add(food);
        }
    }

    public void UnregisterFood(GameObject food)
    {
        if (foodItems.Contains(food))
        {
            foodItems.Remove(food);
        }
    }

    public void RegisterHunter(Hunter hunterInstance)
    {
        if (hunter == null)
        {
            hunter = hunterInstance;
        }
    }

    public void UnregisterHunter(Hunter hunterInstance)
    {
        if (hunter == hunterInstance)
        {
            hunter = null;
        }
    }

    [Header("UI Debug")]
    public TextMeshProUGUI debugText;

    // --- MÉTODO UPDATE MODIFICADO ---
    public void Update()
    {
        // --- NUEVO: Lógica de limpieza de listas ---
        // Esta es una medida de seguridad para eliminar cualquier referencia "fantasma"
        // que pueda haber quedado si un objeto fue destruido.
        CleanUpLists();

        // Si no quedan boids y el juego ya había comenzado con boids, reinicia la escena.
        if (boids.Count == 0)
        {
            Debug.Log("<color=orange>¡Todos los boids han sido eliminados! Reiniciando la escena...</color>");
            // Carga la escena activa actualmente por su nombre.
            UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
        }
        // Lógica para activar/desactivar el movimiento con la tecla que asignaste.
        if (Input.GetKeyDown(KeyCode.S)) // Nota: Cambiaste la tecla a 'S'
        {
            Agent.movementEnabled = !Agent.movementEnabled;

            if (Agent.movementEnabled)
            {
                Debug.Log("<color=lime>MOVIMIENTO HABILITADO</color> - Presiona 'S' para detener.", this);
            }
            else
            {
                Debug.Log("<color=red>MOVIMIENTO DESHABILITADO</color> - Presiona 'S' para reanudar.", this);
            }
        }

        // Lógica para actualizar el texto de la UI.
        if (debugText != null)
        {
            debugText.text = $"Comida Consumida: {Food.foodEaten}\n" +
                             $"Comidas restantes: {foodItems.Count}\n" +
                             $"Boids Restantes: {boids.Count}";
        }
    }

    /// <summary>
    /// Elimina todas las entradas nulas de las listas de entidades.
    /// </summary>
    private void CleanUpLists()
    {
        // Usa el método RemoveAll de LINQ para eliminar cualquier boid que sea 'null'.
        // La expresión "item => item == null" es una forma corta de decir:
        // "Para cada 'item' en la lista, si el 'item' es nulo, elimínalo".
        boids.RemoveAll(item => item == null);

        // Hace lo mismo para la lista de comida.
        foodItems.RemoveAll(item => item == null);
    }
}