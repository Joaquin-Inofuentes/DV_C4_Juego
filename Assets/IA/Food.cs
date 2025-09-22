using UnityEngine;

public class Food : MonoBehaviour
{
    private void Start()
    {
        EntityManager.Instance.RegisterFood(this.gameObject);
    }

    private void OnDestroy()
    {
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
        Debug.Log($"La comida '{name}' ha sido consumida.");
        Destroy(gameObject);
    }
}