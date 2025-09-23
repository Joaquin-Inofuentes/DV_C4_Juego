using UnityEngine;

public class Food : MonoBehaviour
{
    public static int foodEaten = 0;

    private void OnEnable()
    {
        if (EntityManager.Instance != null)
        {
            EntityManager.Instance.RegisterFood(this.gameObject);
        }
    }

    private void OnDisable()
    {
        // OnDisable se llama automáticamente cuando se destruye el objeto,
        // por lo que el OnDestroy que tenías es redundante.
        if (EntityManager.Instance != null)
        {
            EntityManager.Instance.UnregisterFood(this.gameObject);
        }
    }

    /// <summary>
    /// Método llamado por un Boid cuando consume este objeto.
    /// </summary>
    public void Consume()
    {
        foodEaten++;
        Debug.Log($"<color=yellow>COMIDA CONSUMIDA:</color> Total = {foodEaten}");

        // --- NUEVA LÍNEA CRÍTICA ---
        // Notifica al BoidSpawner que debe crear un nuevo boid.
        if (BoidSpawner.Instance != null)
        {
            BoidSpawner.Instance.SpawnBoid();
        }

        // Destruye el objeto de comida.
        Destroy(gameObject);
    }
}