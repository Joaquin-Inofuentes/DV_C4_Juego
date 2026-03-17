using UnityEngine;

/// <summary>
/// Se encarga de generar comida (Food) de forma aleatoria dentro de un área definida por un Transform.
/// Mantiene una cantidad máxima de comida en la escena.
/// </summary>
public class FoodSpawner : MonoBehaviour
{
    [Header("Configuración del Spawner")]
    [Tooltip("El Prefab del objeto de comida que se va a generar.")]
    public GameObject foodPrefab;

    // --- CAMBIO IMPORTANTE: Ahora usamos un Transform en lugar de un BoxCollider ---
    [Tooltip("El Transform del objeto (un cubo simple) que define el área donde puede aparecer la comida.")]
    public Transform spawnArea;

    [Header("Parámetros de Generación")]
    [Tooltip("Cuánta comida se generará al inicio del juego.")]
    public int initialFoodCount = 10;

    [Tooltip("El spawner intentará mantener esta cantidad de comida en la escena.")]
    public int maxFoodCount = 15;

    [Tooltip("La posición fija en el eje Y donde aparecerá toda la comida.")]
    public float fixedYPosition = 0.5f;

    // Se ejecuta una vez al inicio del juego.
    private void Start()
    {
        // Comprobación de seguridad para asegurarse de que todo está configurado en el Inspector.
        if (foodPrefab == null || spawnArea == null)
        {
            Debug.LogError("FoodSpawner no está configurado correctamente. Asigna el Food Prefab y el Spawn Area en el Inspector.", this);
            this.enabled = false;
            return;
        }

        // Genera la cantidad inicial de comida.
        for (int i = 0; i < initialFoodCount; i++)
        {
            SpawnFood();
        }
    }

    // Se ejecuta en cada frame.
    private void Update()
    {
        //Debug.Log(EntityManager.Instance.foodItems.Count + " | " + maxFoodCount);
        // Comprueba constantemente si la cantidad de comida en la escena es menor que el máximo permitido.
        if (EntityManager.Instance.foodItems.Count < maxFoodCount)
        {
            // Si hay menos comida de la deseada, genera una nueva.
            SpawnFood();
        }
    }

    /// <summary>
    /// Genera una única instancia del prefab de comida en una posición aleatoria dentro del spawnArea.
    /// </summary>
    void SpawnFood()
    {
        // --- LÓGICA CORREGIDA: Usa la posición y escala del Transform ---

        // 1. Obtiene el centro del área de la posición del Transform.
        Vector3 center = spawnArea.position;
        // 2. Obtiene el tamaño del área de la escala del Transform.
        //    (Esto asume que el mesh base del cubo es de tamaño 1x1x1).
        Vector3 size = spawnArea.localScale;

        // 3. Calcula los límites mínimos y máximos en los ejes X y Z.
        float minX = center.x - size.x / 2;
        float maxX = center.x + size.x / 2;
        float minZ = center.z - size.z / 2;
        float maxZ = center.z + size.z / 2;

        // 4. Calcula una posición aleatoria dentro de esos límites.
        float randomX = Random.Range(minX, maxX);
        float randomZ = Random.Range(minZ, maxZ);

        // 5. Crea el vector de la posición final usando las coordenadas aleatorias y la Y fija.
        Vector3 spawnPosition = new Vector3(randomX, fixedYPosition, randomZ);

        // 6. Instancia (crea) el prefab de comida en la posición calculada, sin rotación.
        Instantiate(foodPrefab, spawnPosition, Quaternion.identity);
    }
}