using UnityEngine;

/// <summary>
/// Se encarga de generar Boids de forma aleatoria dentro de un área definida.
/// Puede ser llamado para crear un nuevo boid cuando sea necesario.
/// </summary>
public class BoidSpawner : MonoBehaviour
{
    // --- Singleton Pattern ---
    // Esto permite que otros scripts (como Food.cs) accedan fácilmente a este spawner.
    public static BoidSpawner Instance { get; private set; }

    [Header("Configuración del Spawner de Boids")]
    [Tooltip("El Prefab del Boid que se va a generar.")]
    public GameObject boidPrefab;

    [Tooltip("El Transform del objeto (un cubo simple) que define el área donde pueden aparecer los boids.")]
    public Transform spawnArea;

    [Tooltip("La posición fija en el eje Y donde aparecerán todos los boids.")]
    public float fixedYPosition = 0.5f;

    private void Update()
    {
        // Configuración del Singleton.
        if (Instance == null)
        {
            Instance = this;
        }
    }

    private void Start()
    {
        // Comprobación de seguridad para asegurarse de que todo está configurado.
        if (boidPrefab == null || spawnArea == null)
        {
            Debug.LogError("BoidSpawner no está configurado. Asigna el Boid Prefab y el Spawn Area.", this);
            this.enabled = false;
        }
    }

    /// <summary>
    /// Genera una única instancia del prefab de Boid en una posición aleatoria.
    /// Este método será llamado por el script de Food.
    /// </summary>
    public void SpawnBoid()
    {
        Debug.Log("Creacion de un boid hecha");
        // Obtiene los límites del área de la posición y escala del Transform.
        Vector3 center = spawnArea.position;
        Vector3 size = spawnArea.localScale;

        // Calcula una posición aleatoria dentro de esos límites.
        float randomX = Random.Range(center.x - size.x / 2, center.x + size.x / 2);
        float randomZ = Random.Range(center.z - size.z / 2, center.z + size.z / 2);

        // Crea el vector de la posición final.
        Vector3 spawnPosition = new Vector3(randomX, fixedYPosition, randomZ);

        // Instancia el prefab del boid.
        Instantiate(boidPrefab, spawnPosition, Quaternion.identity);
    }
}